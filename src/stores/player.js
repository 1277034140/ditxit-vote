import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const usePlayerStore = defineStore('player', () => {
  // State
  const playerId = ref(null)
  const playerName = ref(null)
  const isHost = ref(false)

  // Getters
  const isLoggedIn = computed(() => !!playerId.value && !!playerName.value)

  // Actions
  function setPlayer(id, name, host = false) {
    playerId.value = id
    playerName.value = name
    isHost.value = host
  }

  function clearPlayer() {
    playerId.value = null
    playerName.value = null
    isHost.value = false
  }

  function generatePlayerId() {
    return `p_${Date.now()}_${Math.random().toString(36).slice(2, 11)}`
  }

  return {
    playerId,
    playerName,
    isHost,
    isLoggedIn,
    setPlayer,
    clearPlayer,
    generatePlayerId
  }
})
