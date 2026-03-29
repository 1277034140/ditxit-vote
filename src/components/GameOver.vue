<template>
  <div class="card fade-in">
    <h2>游戏结束!</h2>
    <div class="winner-section" v-if="winner">
      <div class="winner-name">{{ winner.name }}</div>
      <div class="winner-score">获得 {{ winner.score }} 分</div>
    </div>
    <Leaderboard :players="sortedPlayers" />
    <button class="btn btn-primary" @click="$emit('go-home')">返回首页</button>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import Leaderboard from './Leaderboard.vue'

const props = defineProps({
  players: {
    type: Array,
    default: () => []
  }
})

defineEmits(['go-home'])

const sortedPlayers = computed(() => {
  return [...props.players].sort((a, b) => (b.score || 0) - (a.score || 0))
})

const winner = computed(() => {
  if (sortedPlayers.value.length === 0) return null
  return sortedPlayers.value[0]
})
</script>

<style scoped>
.winner-section {
  text-align: center;
  padding: 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 12px;
  color: white;
  margin-bottom: 20px;
}

.winner-name {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 8px;
}

.winner-score {
  font-size: 18px;
}
</style>
