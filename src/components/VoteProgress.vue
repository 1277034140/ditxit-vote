<template>
  <div class="vote-progress">
    <div class="vote-dots">
      <div
        v-for="player in voters"
        :key="player.id"
        class="vote-dot"
        :class="{ voted: player.hasVoted }"
        :title="player.name"
      ></div>
    </div>
    <span class="vote-count">{{ votedCount }}/{{ totalCount }}</span>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  voters: {
    type: Array,
    default: () => []
  }
})

const votedCount = computed(() => props.voters.filter(p => p.hasVoted).length)
const totalCount = computed(() => props.voters.length)
</script>

<style scoped>
.vote-progress {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 8px;
  margin: 12px 0;
}

.vote-dots {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.vote-dot {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background: #e0e0e0;
  transition: background 0.3s;
}

.vote-dot.voted {
  background: #38ef7d;
}

.vote-count {
  font-size: 14px;
  color: #666;
  margin-left: 4px;
}
</style>
