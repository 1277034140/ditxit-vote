<template>
  <div class="card fade-in">
    <h2>卡牌归属</h2>
    <div class="round-info">第 {{ currentRound }} 轮</div>

    <div class="card-owners-display">
      <div
        v-for="(ownerId, cardId) in sortedCardOwners"
        :key="cardId"
        class="owner-item"
        :class="{ storyteller: isStorytellerCard(cardId) }"
      >
        <span class="card-label">卡牌 {{ cardId }}</span>
        <span class="arrow">→</span>
        <span class="owner-name">
          {{ getOwnerName(ownerId) }}
          <span v-if="isStorytellerCard(cardId)" class="badge badge-storyteller">讲述者</span>
        </span>
      </div>
    </div>

    <!-- 主持人显示"进入计分"按钮 -->
    <button
      v-if="isHost"
      class="btn btn-success"
      @click="$emit('calculate')"
    >
      进入计分
    </button>
    <!-- 非主持人显示等待信息 -->
    <div v-else class="status-bar waiting-message">
      等待主持人计分<span class="waiting-dots"></span>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  currentRound: {
    type: Number,
    default: 1
  },
  cardOwners: {
    type: Object,
    default: () => ({})
  },
  storytellerCardId: {
    type: Number,
    default: null
  },
  players: {
    type: Array,
    default: () => []
  },
  isHost: {
    type: Boolean,
    default: false
  }
})

defineEmits(['calculate'])

const sortedCardOwners = computed(() => {
  const entries = Object.entries(props.cardOwners)
  entries.sort((a, b) => parseInt(a[0]) - parseInt(b[0]))
  return Object.fromEntries(entries)
})

function getOwnerName(ownerId) {
  const player = props.players.find(p => p.id === ownerId)
  return player ? player.name : '未知'
}

function isStorytellerCard(cardId) {
  return parseInt(cardId) === props.storytellerCardId
}
</script>

<style scoped>
.card-owners-display {
  margin: 16px 0;
}

.owner-item {
  display: flex;
  align-items: center;
  padding: 12px;
  margin: 8px 0;
  background: #e8f5e9;
  border-radius: 8px;
}

.owner-item.storyteller {
  background: #fff3cd;
}

.card-label {
  font-weight: bold;
  font-size: 18px;
  min-width: 80px;
}

.arrow {
  margin: 0 12px;
  color: #666;
}

.owner-name {
  flex: 1;
}

.waiting-message {
  margin-top: 16px;
}
</style>
