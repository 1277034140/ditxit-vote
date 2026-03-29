<template>
  <div class="leaderboard">
    <div
      v-for="(player, index) in sortedPlayers"
      :key="player.id"
      class="leader-item"
    >
      <div class="rank" :class="`rank-${index + 1}`">{{ index + 1 }}</div>
      <div class="leader-name">
        {{ player.name }}
        <span v-if="player.isHost" class="badge badge-host">主持</span>
      </div>
      <div class="leader-score">{{ player.score || 0 }}</div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  players: {
    type: Array,
    default: () => []
  }
})

const sortedPlayers = computed(() => {
  return [...props.players].sort((a, b) => (b.score || 0) - (a.score || 0))
})
</script>
