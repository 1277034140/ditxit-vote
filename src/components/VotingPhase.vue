<template>
  <div class="card fade-in">
    <h2>投票时间</h2>
    <div class="round-info">第 {{ currentRound }} 轮</div>

    <!-- 讲述者视角 -->
    <div v-if="isStoryteller" class="storyteller-view">
      <div class="status-bar">你是讲述者，请等待其他玩家投票</div>
      <div class="my-card-display">
        <p style="text-align: center; color: #666;">你的卡牌编号是:</p>
        <div class="room-code" style="margin: 12px 0;">{{ myCardId }}</div>
      </div>
    </div>

    <!-- 可投票玩家视角 -->
    <div v-else class="voter-view">
      <div class="status-bar">请选择你投票的卡牌</div>
      <CardGrid
        v-model="selectedCard"
        :cardCount="cardCount"
        :disabled="hasVoted"
        @select="handleCardSelect"
      />
      <button
        class="btn btn-primary"
        @click="handleSubmitVote"
        :disabled="!selectedCard || hasVoted"
      >
        {{ hasVoted ? '已投票' : '确认投票' }}
      </button>
    </div>

    <!-- 投票进度 -->
    <VoteProgress :voters="voters" />
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import CardGrid from './CardGrid.vue'
import VoteProgress from './VoteProgress.vue'

const props = defineProps({
  currentRound: {
    type: Number,
    default: 1
  },
  cardCount: {
    type: Number,
    default: 6
  },
  myCardId: {
    type: Number,
    default: null
  },
  hasVoted: {
    type: Boolean,
    default: false
  },
  isStoryteller: {
    type: Boolean,
    default: false
  },
  voters: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['submit-vote'])

const selectedCard = ref(null)

watch(() => props.hasVoted, (voted) => {
  if (voted) {
    selectedCard.value = null
  }
})

function handleCardSelect(cardId) {
  selectedCard.value = cardId
}

function handleSubmitVote() {
  if (!selectedCard.value) return
  emit('submit-vote', selectedCard.value)
}
</script>

<style scoped>
.storyteller-view {
  padding: 20px 0;
}

.my-card-display {
  margin: 20px 0;
}
</style>
