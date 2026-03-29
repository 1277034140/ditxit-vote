<template>
  <div class="card fade-in">
    <h2>揭示卡牌归属</h2>
    <div class="round-info">第 {{ currentRound }} 轮</div>
    <p class="hint-text">请为每张卡牌选择对应的玩家</p>

    <div class="card-owner-grid">
      <div
        v-for="cardId in cardIds"
        :key="cardId"
        class="select-wrapper"
      >
        <label>卡牌 {{ cardId }}</label>
        <select
          :value="cardOwners[cardId] || ''"
          @change="handleSelect(cardId, $event.target.value)"
        >
          <option value="">选择玩家</option>
          <option
            v-for="player in players"
            :key="player.id"
            :value="player.id"
          >
            {{ player.name }}
          </option>
        </select>
      </div>
    </div>

    <button
      class="btn btn-success"
      @click="handleReveal"
      :disabled="!isComplete"
    >
      确认揭示
    </button>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'

const props = defineProps({
  currentRound: {
    type: Number,
    default: 1
  },
  cardCount: {
    type: Number,
    default: 6
  },
  players: {
    type: Array,
    default: () => []
  },
  storytellerId: {
    type: String,
    default: null
  }
})

const emit = defineEmits(['reveal'])

const cardOwners = ref({})
const cardIds = computed(() =>
  Array.from({ length: props.cardCount }, (_, i) => i + 1)
)

const isComplete = computed(() => {
  return cardIds.value.every(id => !!cardOwners.value[id])
})

// Pre-fill all card owners from players' cardIds
watch(() => props.players, (playerList) => {
  if (!playerList || playerList.length === 0) return

  const owners = {}
  playerList.forEach(p => {
    if (p.cardId != null) {
      owners[String(p.cardId)] = p.id
    }
  })
  cardOwners.value = owners
}, { immediate: true })

function handleSelect(cardId, playerId) {
  cardOwners.value[cardId] = playerId
}

function handleReveal() {
  if (!isComplete.value) return
  emit('reveal', { ...cardOwners.value })
}
</script>

<style scoped>
.hint-text {
  text-align: center;
  color: #666;
  margin-bottom: 16px;
}

.card-owner-grid {
  margin: 16px 0;
}
</style>
