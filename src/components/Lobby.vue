<template>
  <div class="card fade-in">
    <h2>游戏大厅</h2>
    <div class="round-info">房间码: {{ roomCode }}</div>
    <div class="status-bar">{{ playerCount }} 位玩家</div>
    <PlayerList
      :players="players"
      :storytellerId="storytellerId"
      :isStoryteller="false"
    />
    <button
      v-if="isHost"
      class="btn btn-success"
      @click="$emit('start')"
      :disabled="playerCount < 3"
    >
      {{ playerCount < 3 ? `开始游戏 (需${3 - playerCount}人)` : '开始游戏' }}
    </button>
    <button v-else class="btn btn-secondary" disabled>等待主持人开始游戏...</button>
  </div>
</template>

<script setup>
import PlayerList from './PlayerList.vue'

defineProps({
  roomCode: {
    type: String,
    required: true
  },
  players: {
    type: Array,
    default: () => []
  },
  isHost: {
    type: Boolean,
    default: false
  },
  storytellerId: {
    type: String,
    default: null
  }
})

defineEmits(['start'])
</script>
