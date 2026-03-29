<template>
  <div class="card fade-in">
    <h2>本轮得分</h2>
    <div class="round-info">第 {{ currentRound }} 轮</div>

    <div class="score-changes">
      <div
        v-for="change in scoreChanges"
        :key="change.playerId"
        class="score-change"
        :class="{ positive: change.change > 0 }"
      >
        <strong>{{ change.name }}</strong>
        <span class="score-value">
          {{ change.change > 0 ? `+${change.change}` : change.change }} 分
        </span>
        <div v-if="change.extra" class="extra-hint">
          (含投票奖励 +{{ change.extra }})
        </div>
      </div>
    </div>

    <h3>排行榜</h3>
    <Leaderboard :players="sortedPlayers" />

    <button class="btn btn-primary" @click="$emit('next-round')">下一轮</button>
    <button class="btn btn-secondary" @click="$emit('end-game')">结束游戏</button>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import Leaderboard from './Leaderboard.vue'

const props = defineProps({
  currentRound: {
    type: Number,
    default: 1
  },
  scoreChanges: {
    type: Array,
    default: () => []
  },
  players: {
    type: Array,
    default: () => []
  }
})

defineEmits(['next-round', 'end-game'])

const sortedPlayers = computed(() => {
  return [...props.players].sort((a, b) => (b.score || 0) - (a.score || 0))
})
</script>

<style scoped>
.score-changes {
  margin: 16px 0;
}

.score-change {
  text-align: center;
  padding: 16px;
  background: #f5f5f5;
  border-radius: 10px;
  margin: 12px 0;
}

.score-change.positive {
  background: #e8f8f5;
}

.score-value {
  font-size: 20px;
  font-weight: bold;
  color: #11998e;
  margin-left: 8px;
}

.extra-hint {
  font-size: 12px;
  color: #666;
  margin-top: 4px;
}
</style>
