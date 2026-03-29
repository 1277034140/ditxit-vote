<template>
  <div class="game-view">
    <h1>Dixit 投票系统</h1>

    <div class="container">
      <!-- 首页 -->
      <HomePage
        v-if="currentPage === 'home'"
        @create="handleShowCreate"
        @join="handleShowJoin"
      />

      <!-- 创建房间 -->
      <CreateRoom
        v-if="currentPage === 'create'"
        :roomCode="pendingRoomCode"
        @enter="handleEnterRoom"
      />

      <!-- 加入房间 -->
      <JoinRoom
        v-if="currentPage === 'join'"
        :initialCode="initialJoinCode"
        @join="handleJoinRoom"
      />

      <!-- 游戏大厅 -->
      <Lobby
        v-if="currentPage === 'lobby'"
        :roomCode="gameStore.roomCode"
        :players="gameStore.playerArray"
        :isHost="playerStore.isHost"
        :storytellerId="gameStore.storyteller?.id"
        @start="handleStartGame"
      />

      <!-- 投票阶段 -->
      <VotingPhase
        v-if="currentPage === 'voting'"
        :currentRound="gameStore.roomInfo?.currentRound"
        :cardCount="gameStore.roomInfo?.cardCount"
        :myCardId="myPlayerData?.cardId"
        :hasVoted="myPlayerData?.hasVoted"
        :isStoryteller="isCurrentPlayerStoryteller"
        :voters="nonStorytellerPlayers"
        @submit-vote="handleSubmitVote"
      />

      <!-- 等待投票结果 -->
      <WaitingPhase
        v-if="currentPage === 'waiting-vote'"
        title="等待投票"
        message="其他玩家正在投票"
        :voters="nonStorytellerPlayers"
      />

      <!-- 主持人揭示卡牌归属 -->
      <RevealPhase
        v-if="currentPage === 'reveal-input'"
        :currentRound="gameStore.roomInfo?.currentRound"
        :cardCount="gameStore.roomInfo?.cardCount"
        :players="gameStore.playerArray"
        :storytellerId="gameStore.storyteller?.id"
        @reveal="handleReveal"
      />

      <!-- 揭示结果（所有玩家） -->
      <RevealView
        v-if="currentPage === 'reveal-view'"
        :currentRound="gameStore.roomInfo?.currentRound"
        :cardOwners="gameStore.roomInfo?.cardOwners || {}"
        :storytellerCardId="gameStore.roomInfo?.storytellerCardId"
        :players="gameStore.playerArray"
        :isHost="playerStore.isHost"
        @calculate="handleCalculate"
      />

      <!-- 等待计分 -->
      <WaitingPhase
        v-if="currentPage === 'waiting-score'"
        title="等待计分"
        message="主持人正在计分"
        :voters="gameStore.playerArray"
      />

      <!-- 得分结果 -->
      <ScoreResult
        v-if="currentPage === 'result'"
        :currentRound="gameStore.roomInfo?.currentRound"
        :scoreChanges="gameStore.scoreChanges"
        :players="gameStore.playerArray"
        @next-round="handleNextRound"
        @end-game="handleEndGame"
      />

      <!-- 游戏结束 -->
      <GameOver
        v-if="currentPage === 'game-over'"
        :players="gameStore.playerArray"
        @go-home="handleGoHome"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useGameStore, GAME_STATUS } from '@/stores/game.js'
import { usePlayerStore } from '@/stores/player.js'

import HomePage from '@/components/HomePage.vue'
import CreateRoom from '@/components/CreateRoom.vue'
import JoinRoom from '@/components/JoinRoom.vue'
import Lobby from '@/components/Lobby.vue'
import VotingPhase from '@/components/VotingPhase.vue'
import WaitingPhase from '@/components/WaitingPhase.vue'
import RevealPhase from '@/components/RevealPhase.vue'
import RevealView from '@/components/RevealView.vue'
import ScoreResult from '@/components/ScoreResult.vue'
import GameOver from '@/components/GameOver.vue'

const gameStore = useGameStore()
const playerStore = usePlayerStore()

const currentPage = ref('home')
const pendingRoomCode = ref('')
const initialJoinCode = ref('')

// Computed
const myPlayerData = computed(() => {
  return gameStore.players[playerStore.playerId] || null
})

const isCurrentPlayerStoryteller = computed(() => {
  const st = gameStore.storyteller
  return st && st.id === playerStore.playerId
})

const nonStorytellerPlayers = computed(() => {
  const st = gameStore.storyteller
  return gameStore.playerArray.filter(p => p.id !== st?.id)
})

// Watch room status changes
watch(() => gameStore.roomInfo?.status, (status) => {
  if (!status) return
  handleStatusChange(status)
})

watch(() => [gameStore.votedCount, gameStore.totalVoters, gameStore.roomInfo?.status], ([voted, total, status]) => {
  if (status === GAME_STATUS.VOTING && voted > 0 && voted === total) {
    if (playerStore.isHost) {
      currentPage.value = 'reveal-input'
    } else {
      currentPage.value = 'reveal-view'
    }
  }
}, { deep: true })

// Handle status changes - this is the core state machine
function handleStatusChange(status) {
  switch (status) {
    case GAME_STATUS.WAITING:
      currentPage.value = 'lobby'
      break

    case GAME_STATUS.VOTING:
      if (isCurrentPlayerStoryteller.value) {
        currentPage.value = 'voting'
      } else if (myPlayerData.value?.hasVoted) {
        currentPage.value = 'waiting-vote'
      } else {
        currentPage.value = 'voting'
      }
      break

    case GAME_STATUS.REVEAL_INPUT:
      // Host sees reveal input, others wait
      if (playerStore.isHost) {
        currentPage.value = 'reveal-input'
      } else {
        currentPage.value = 'reveal-view'
      }
      break

    case GAME_STATUS.REVEAL_VIEW:
      currentPage.value = 'reveal-view'
      break

    case GAME_STATUS.SCORING:
      currentPage.value = 'waiting-score'
      break

    case GAME_STATUS.RESULT:
      currentPage.value = 'result'
      break

    case GAME_STATUS.GAME_OVER:
      currentPage.value = 'game-over'
      break
  }
}

// Handlers
function handleShowCreate() {
  pendingRoomCode.value = gameStore.generateRoomCode()
  currentPage.value = 'create'
}

function handleShowJoin() {
  initialJoinCode.value = ''
  currentPage.value = 'join'
}

async function handleEnterRoom(playerName) {
  try {
    const playerId = playerStore.generatePlayerId()
    await gameStore.createRoom(playerId, playerName, pendingRoomCode.value)
    playerStore.setPlayer(playerId, playerName, true)
    gameStore.watchRoom()
    currentPage.value = 'lobby'
  } catch (e) {
    alert('创建房间失败: ' + e.message)
  }
}

async function handleJoinRoom({ code, name }) {
  try {
    const playerId = playerStore.generatePlayerId()
    await gameStore.joinRoom(code, playerId, name)
    playerStore.setPlayer(playerId, name, false)
    gameStore.watchRoom()
    currentPage.value = 'lobby'
  } catch (e) {
    alert('加入房间失败: ' + e.message)
  }
}

async function handleStartGame() {
  try {
    await gameStore.startGame()
  } catch (e) {
    alert('开始游戏失败: ' + e.message)
  }
}

async function handleSubmitVote({ myCardId, voteFor }) {
  try {
    await gameStore.submitVote(playerStore.playerId, myCardId, voteFor)
    if (voteFor !== null && voteFor !== undefined) {
      currentPage.value = 'waiting-vote'
    }
  } catch (e) {
    alert('提交失败: ' + e.message)
  }
}

async function handleReveal(cardOwners) {
  try {
    await gameStore.revealCardOwners(cardOwners)
  } catch (e) {
    alert('揭示失败: ' + e.message)
  }
}

async function handleCalculate() {
  try {
    await gameStore.calculateScores()
  } catch (e) {
    alert('计分失败: ' + e.message)
  }
}

async function handleNextRound() {
  try {
    await gameStore.nextRound()
  } catch (e) {
    alert('下一轮失败: ' + e.message)
  }
}

async function handleEndGame() {
  try {
    await gameStore.endGame()
  } catch (e) {
    alert('结束游戏失败: ' + e.message)
  }
}

function handleGoHome() {
  gameStore.deleteRoom()
  playerStore.clearPlayer()
  currentPage.value = 'home'
  pendingRoomCode.value = ''
}

// Check URL params for room code
onMounted(() => {
  const urlParams = new URLSearchParams(window.location.search)
  const roomCode = urlParams.get('room')
  if (roomCode) {
    initialJoinCode.value = roomCode.toUpperCase()
    currentPage.value = 'join'
  }
})
</script>

<style scoped>
.game-view {
  min-height: 100vh;
}

.game-view h1 {
  text-align: center;
  color: white;
  font-size: 28px;
  margin-bottom: 24px;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}
</style>
