<template>
  <div class="card fade-in">
    <h2>投票时间</h2>
    <div class="round-info">第 {{ currentRound }} 轮</div>

    <!-- 讲述者：只选自己的卡牌编号 -->
    <div v-if="isStoryteller" class="storyteller-view">
      <div class="status-bar">请选择洗牌后，你出的那张牌对应的编号</div>
      <p class="pick-hint">
        大家出完牌并洗牌后，桌上卡牌为 1～{{ cardCount }} 号，请点选与你出的那张牌一致的编号。
      </p>
      <CardGrid
        v-model="myCardId"
        :cardCount="cardCount"
        :disabled="hasSubmittedMyCard"
      />
      <button
        class="btn btn-primary"
        :disabled="!myCardId || hasSubmittedMyCard"
        @click="handleSubmitMyCard"
      >
        {{ hasSubmittedMyCard ? '已提交' : '确认我的卡牌编号' }}
      </button>
    </div>

    <!-- 其他玩家：先选卡牌编号，再投票 -->
    <div v-else class="voter-view">
      <div class="status-bar">请先选择你出的牌对应的编号，再投票</div>
      <p class="pick-hint">
        洗牌后桌上为 1～{{ cardCount }} 号，上方点选<strong>你出的那张牌</strong>对应的编号；下方再选你认为<strong>讲述者</strong>出的那张牌的编号。
      </p>

      <div class="card-select-section">
        <div class="section-label">我出的牌（桌上编号）：</div>
        <CardGrid
          v-model="myCardId"
          :cardCount="cardCount"
          :disabled="hasSubmittedMyCard"
        />
        <button
          class="btn btn-outline"
          :disabled="!myCardId || hasSubmittedMyCard"
          @click="handleSubmitMyCard"
        >
          {{ hasSubmittedMyCard ? '已提交' : '确认我的卡牌' }}
        </button>
      </div>

      <div v-if="hasSubmittedMyCard" class="vote-section">
        <div class="section-label">投票（你认为讲述者出的牌）：</div>
        <p class="vote-hint">不能投自己出的那张牌（与上方编号相同）</p>
        <div v-if="voteError" class="error-msg">{{ voteError }}</div>
        <CardGrid
          v-model="selectedCard"
          :cardCount="cardCount"
          :excludeIds="voteExcludeIds"
          :disabled="hasVoted"
          @select="handleVoteSelect"
        />
        <button
          class="btn btn-primary"
          @click="handleSubmitVote"
          :disabled="!selectedCard || hasVoted || voteInvalid"
        >
          {{ hasVoted ? '已投票' : '确认投票' }}
        </button>
      </div>
    </div>

    <!-- 投票进度（不含讲述者） -->
    <VoteProgress :voters="voters" />
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import CardGrid from './CardGrid.vue'
import VoteProgress from './VoteProgress.vue'

const props = defineProps({
  currentRound: { type: Number, default: 1 },
  cardCount: { type: Number, default: 6 },
  myCardId: { type: Number, default: null },
  hasVoted: { type: Boolean, default: false },
  isStoryteller: { type: Boolean, default: false },
  voters: { type: Array, default: () => [] }
})

const emit = defineEmits(['submit-vote'])

const selectedCard = ref(null)
const myCardId = ref(props.myCardId ?? null)
/** 仅由本机点击「确认我的卡牌」锁定，不用 props.cardId 推断（避免旧数据一进页就「已提交」） */
const hasSubmittedMyCard = ref(false)
const voteError = ref('')

/** 投票区禁用「自己出的牌」对应编号，不能投自己 */
const voteExcludeIds = computed(() => {
  if (myCardId.value == null || myCardId.value === '') return []
  return [Number(myCardId.value)]
})

const voteInvalid = computed(() => {
  if (selectedCard.value == null || myCardId.value == null) return false
  return Number(selectedCard.value) === Number(myCardId.value)
})

watch(() => props.myCardId, (id) => {
  myCardId.value = id ?? null
})

onMounted(() => {
  // 仅讲述者：若已同步到服务器（例如刷新页面），视为已确认过编号
  if (props.isStoryteller && props.myCardId != null) {
    hasSubmittedMyCard.value = true
  }
})

watch([myCardId, selectedCard], () => {
  if (
    selectedCard.value != null &&
    myCardId.value != null &&
    Number(selectedCard.value) === Number(myCardId.value)
  ) {
    selectedCard.value = null
  }
  voteError.value = ''
})

function handleVoteSelect(cardId) {
  selectedCard.value = cardId
  voteError.value = ''
}

function handleSubmitMyCard() {
  if (!myCardId.value) return
  hasSubmittedMyCard.value = true
  if (!props.isStoryteller) return
  emit('submit-vote', { myCardId: myCardId.value, voteFor: null })
}

function handleSubmitVote() {
  if (!selectedCard.value) return
  if (Number(selectedCard.value) === Number(myCardId.value)) {
    voteError.value = '不能投自己出的牌，请另选一个编号'
    return
  }
  voteError.value = ''
  emit('submit-vote', { myCardId: myCardId.value, voteFor: selectedCard.value })
}
</script>

<style scoped>
.storyteller-view, .voter-view {
  padding: 16px 0;
}

.pick-hint {
  text-align: center;
  color: #666;
  font-size: 14px;
  line-height: 1.5;
  margin: 8px 0 16px;
  padding: 0 8px;
}

.card-select-section, .vote-section {
  margin: 16px 0;
}

.section-label {
  font-weight: bold;
  color: #fff;
  margin-bottom: 8px;
  font-size: 15px;
}

.vote-section {
  margin-top: 24px;
  padding-top: 20px;
  border-top: 1px solid rgba(255,255,255,0.1);
}

.vote-hint {
  text-align: center;
  color: #888;
  font-size: 13px;
  margin-bottom: 10px;
}
</style>
