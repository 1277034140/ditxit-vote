import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { getDb } from '@/config/firebase.js'
import { ref as dbRef, set, get, update, runTransaction, onValue, remove } from 'firebase/database'
import { generateRoomCode as genCode } from '@/utils/roomCode.js'

export const GAME_STATUS = {
  WAITING: 'waiting',
  VOTING: 'voting',
  REVEAL_INPUT: 'reveal_input',
  REVEAL_VIEW: 'reveal_view',
  SCORING: 'scoring',
  RESULT: 'result',
  GAME_OVER: 'game_over'
}

export const useGameStore = defineStore('game', () => {
  const db = getDb()

  // State
  const roomCode = ref(null)
  const roomInfo = ref(null)
  const players = ref({})
  const unsubscribe = ref(null)
  const isInitialized = ref(false)
  const error = ref(null)

  // Getters
  const playerArray = computed(() => {
    return Object.entries(players.value).map(([id, data]) => ({
      id,
      ...data
    }))
  })

  const playerCount = computed(() => playerArray.value.length)

  const storyteller = computed(() => {
    if (!roomInfo.value || roomInfo.value.storytellerIndex === undefined) return null
    const index = roomInfo.value.storytellerIndex
    return playerArray.value[index] || null
  })

  const isStoryteller = (playerId) => {
    return storyteller.value?.id === playerId
  }

  const votedCount = computed(() => {
    const st = storyteller.value
    if (!st) return 0
    return playerArray.value.filter(p => p.id !== st.id && p.hasVoted).length
  })

  const totalVoters = computed(() => {
    return playerCount.value - 1
  })

  const allVoted = computed(() => {
    return votedCount.value === totalVoters.value
  })

  const scoreChanges = computed(() => {
    return roomInfo.value?.lastScoreChanges || []
  })

  // Generate room code
  function generateRoomCode() {
    return genCode()
  }

  // Create room
  async function createRoom(hostPlayerId, hostName, providedRoomCode) {
    let code = providedRoomCode

    if (code) {
      const existing = await get(dbRef(db, `rooms/${code}/info`))
      if (existing.exists()) {
        code = null
      }
    }

    if (!code) {
      code = generateRoomCode()
    }

    const roomData = {
      info: {
        createdAt: Date.now(),
        currentRound: 1,
        status: GAME_STATUS.WAITING,
        storytellerIndex: 0,
        cardOwners: null,
        storytellerCardId: null,
        cardCount: 0,
        scoreCalculated: false,
        lastScoreChanges: null
      },
      players: {
        [hostPlayerId]: {
          name: hostName,
          score: 0,
          cardId: null,
          hasVoted: false,
          votedFor: null,
          isHost: true
        }
      }
    }

    try {
      await set(dbRef(db, `rooms/${code}`), roomData)
      roomCode.value = code
      return code
    } catch (e) {
      error.value = e.message
      throw e
    }
  }

  // Join room
  async function joinRoom(code, playerId, playerName) {
    const roomSnapshot = await get(dbRef(db, `rooms/${code}/info`))

    if (!roomSnapshot.exists()) {
      throw new Error('房间不存在')
    }

    const info = roomSnapshot.val()
    if (info.status !== GAME_STATUS.WAITING) {
      throw new Error('游戏已开始，无法加入')
    }

    const playersSnapshot = await get(dbRef(db, `rooms/${code}/players`))
    const existingPlayers = playersSnapshot.val() || {}
    const names = Object.values(existingPlayers).map(p => p.name.toLowerCase())

    if (names.includes(playerName.toLowerCase())) {
      throw new Error('这个名字已被使用')
    }

    await set(dbRef(db, `rooms/${code}/players/${playerId}`), {
      name: playerName,
      score: 0,
      cardId: null,
      hasVoted: false,
      votedFor: null,
      isHost: false
    })

    roomCode.value = code
  }

  // Watch room
  function watchRoom() {
    if (unsubscribe.value) {
      unsubscribe.value()
    }

    const roomPath = dbRef(db, `rooms/${roomCode.value}`)
    unsubscribe.value = onValue(roomPath, (snapshot) => {
      if (!snapshot.exists()) {
        roomInfo.value = null
        players.value = {}
        error.value = '房间已关闭'
        return
      }

      const data = snapshot.val()
      roomInfo.value = data.info || {}
      players.value = data.players || {}
      isInitialized.value = true
    })
  }

  // Stop watching
  function unwatchRoom() {
    if (unsubscribe.value) {
      unsubscribe.value()
      unsubscribe.value = null
    }
    roomCode.value = null
    roomInfo.value = null
    players.value = {}
    isInitialized.value = false
  }

  // Start game
  async function startGame() {
    const ids = playerArray.value.map(p => p.id)
    const cardCount = ids.length

    const updates = {}
    updates[`rooms/${roomCode.value}/info/status`] = GAME_STATUS.VOTING
    updates[`rooms/${roomCode.value}/info/cardCount`] = cardCount
    updates[`rooms/${roomCode.value}/info/scoreCalculated`] = false

    const storytellerIndex = Math.floor(Math.random() * ids.length)

    ids.forEach((id) => {
      // 洗牌后编号由每位玩家在投票页自行选择，不在开局写入
      updates[`rooms/${roomCode.value}/players/${id}/cardId`] = null
      updates[`rooms/${roomCode.value}/players/${id}/hasVoted`] = false
      updates[`rooms/${roomCode.value}/players/${id}/votedFor`] = null
    })

    updates[`rooms/${roomCode.value}/info/storytellerIndex`] = storytellerIndex

    await update(dbRef(db), updates)
  }

  // 所有人投票时同步提交自己出的牌编号；非讲述者另传 voteFor（投给哪张牌，不能等于自己的 myCardId）
  async function submitVote(playerId, myCardId, voteFor) {
    const playerList = Object.values(players.value)
    const st = storyteller.value

    const updates = {}

    if (myCardId != null) {
      updates[`rooms/${roomCode.value}/players/${playerId}/cardId`] = myCardId
    }

    if (st && playerId !== st.id) {
      if (voteFor != null && Number(voteFor) === Number(myCardId)) {
        throw new Error('不能投自己出的牌')
      }
      updates[`rooms/${roomCode.value}/players/${playerId}/hasVoted`] = true
      if (voteFor != null) {
        updates[`rooms/${roomCode.value}/players/${playerId}/votedFor`] = voteFor
      }

      const nonStList = playerList.filter(p => p.id !== st.id)
      const allVoted = nonStList.every(p => p.id === playerId ? true : p.hasVoted)

      if (allVoted) {
        const cardOwners = {}
        playerList.forEach(p => {
          if (p.cardId != null) {
            cardOwners[String(p.cardId)] = p.id
          }
        })
        const stCardId = st.cardId != null ? String(st.cardId) : null
        updates[`rooms/${roomCode.value}/info/cardOwners`] = cardOwners
        updates[`rooms/${roomCode.value}/info/storytellerCardId`] = stCardId ? parseInt(stCardId) : null
        updates[`rooms/${roomCode.value}/info/status`] = GAME_STATUS.REVEAL_INPUT
      }
    }

    await update(dbRef(db), updates)
  }

  // Reveal card owners (host only)
  async function revealCardOwners(cardOwners) {
    const st = storyteller.value
    const storytellerCardId = st ? parseInt(Object.keys(cardOwners).find(cid => cardOwners[cid] === st.id)) : null

    await update(dbRef(db, `rooms/${roomCode.value}/info`), {
      cardOwners,
      storytellerCardId,
      status: GAME_STATUS.REVEAL_VIEW
    })
  }

  // Calculate and show scores (host only)
  async function calculateScores() {
    const infoRef = dbRef(db, `rooms/${roomCode.value}/info`)

    // Use runTransaction for atomic operation
    try {
      const result = await runTransaction(infoRef, (current) => {
        if (!current) return current
        if (current.status !== GAME_STATUS.REVEAL_VIEW) return current
        if (current.scoreCalculated) return current
        current.scoreCalculated = true
        return current
      })

      if (!result.committed || !result.snapshot.exists()) {
        return false
      }

      const freshSnapshot = await get(dbRef(db, `rooms/${roomCode.value}`))
      const data = freshSnapshot.val()
      const stIndex = data.info.storytellerIndex
      const stCardId = data.info.storytellerCardId
      const cardOwners = data.info.cardOwners || {}
      const playerList = Object.entries(data.players).map(([id, p]) => ({ id, ...p }))
      const st = playerList[stIndex]

      const stCardNum = parseInt(stCardId)
      const nonStCount = playerList.length - 1

      let correctGuessers = 0
      const voterMap = {}

      playerList.forEach(p => {
        if (p.id === st.id) return
        if (parseInt(p.votedFor) === stCardNum) {
          correctGuessers++
          voterMap[p.id] = true
        }
      })

      let scoreChanges = []

      if (correctGuessers === 0 || correctGuessers === nonStCount) {
        playerList.forEach(p => {
          scoreChanges.push({
            playerId: p.id,
            name: p.name,
            change: p.id !== st.id ? 2 : 0,
            extra: 0
          })
        })
      } else {
        playerList.forEach(p => {
          let change = 0
          if (p.id === st.id || voterMap[p.id]) {
            change = 3
          }
          scoreChanges.push({
            playerId: p.id,
            name: p.name,
            change,
            extra: 0
          })
        })
      }

      // Extra points for receiving votes
      playerList.forEach(p => {
        const myCardId = Object.keys(cardOwners).find(cid => parseInt(cardOwners[cid]) === parseInt(p.id.replace(/-/g, '').slice(0, 8)))
        // Find by name instead
        const myCardIdByName = Object.keys(cardOwners).find(cid => {
          const ownerPlayer = playerList.find(pl => pl.id === cardOwners[cid])
          return ownerPlayer && ownerPlayer.name === p.name
        })
        if (!myCardIdByName) return
        if (parseInt(myCardIdByName) === stCardNum) return

        const myCardNum = parseInt(myCardIdByName)
        let extraPoints = 0
        playerList.forEach(voter => {
          if (voter.id === p.id) return
          if (parseInt(voter.votedFor) === myCardNum) {
            extraPoints++
          }
        })
        if (extraPoints > 0) {
          const changeObj = scoreChanges.find(c => c.name === p.name)
          if (changeObj) {
            changeObj.change += extraPoints
            changeObj.extra = extraPoints
          }
        }
      })

      // Update scores
      const scoreUpdates = {}
      scoreChanges.forEach(sc => {
        const currentPlayer = playerList.find(p => p.name === sc.name)
        if (currentPlayer) {
          scoreUpdates[`rooms/${roomCode.value}/players/${currentPlayer.id}/score`] =
            (currentPlayer.score || 0) + sc.change
        }
      })

      await update(dbRef(db), {
        ...scoreUpdates,
        [`rooms/${roomCode.value}/info/status`]: GAME_STATUS.RESULT,
        [`rooms/${roomCode.value}/info/lastScoreChanges`]: scoreChanges
      })

      return true
    } catch (e) {
      error.value = e.message
      return false
    }
  }

  // Next round
  async function nextRound() {
    const newRound = (roomInfo.value?.currentRound || 1) + 1
    const ids = playerArray.value.map(p => p.id)
    const stIndex = roomInfo.value?.storytellerIndex || 0
    const nextStIndex = (stIndex + 1) % ids.length

    const updates = {}
    updates[`rooms/${roomCode.value}/info/currentRound`] = newRound
    updates[`rooms/${roomCode.value}/info/status`] = GAME_STATUS.VOTING
    updates[`rooms/${roomCode.value}/info/scoreCalculated`] = false
    updates[`rooms/${roomCode.value}/info/cardOwners`] = null
    updates[`rooms/${roomCode.value}/info/storytellerCardId`] = null
    updates[`rooms/${roomCode.value}/info/lastScoreChanges`] = null
    updates[`rooms/${roomCode.value}/info/storytellerIndex`] = nextStIndex

    ids.forEach((id) => {
      updates[`rooms/${roomCode.value}/players/${id}/cardId`] = null
      updates[`rooms/${roomCode.value}/players/${id}/hasVoted`] = false
      updates[`rooms/${roomCode.value}/players/${id}/votedFor`] = null
    })

    await update(dbRef(db), updates)
  }

  // End game
  async function endGame() {
    await update(dbRef(db, `rooms/${roomCode.value}/info`), {
      status: GAME_STATUS.GAME_OVER
    })
  }

  // Delete room
  async function deleteRoom() {
    if (roomCode.value) {
      await remove(dbRef(db, `rooms/${roomCode.value}`))
    }
    unwatchRoom()
  }

  // Clear error
  function clearError() {
    error.value = null
  }

  return {
    // State
    roomCode,
    roomInfo,
    players,
    isInitialized,
    error,
    // Getters
    playerArray,
    playerCount,
    storyteller,
    isStoryteller,
    votedCount,
    totalVoters,
    allVoted,
    scoreChanges,
    // Actions
    generateRoomCode,
    createRoom,
    joinRoom,
    watchRoom,
    unwatchRoom,
    startGame,
    submitVote,
    revealCardOwners,
    calculateScores,
    nextRound,
    endGame,
    deleteRoom,
    clearError
  }
})
