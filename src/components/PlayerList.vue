<template>
  <ul class="player-list">
    <li v-for="player in players" :key="player.id" class="player-item">
      <span class="player-name">
        {{ player.name }}
        <span v-if="player.isHost" class="badge badge-host">主持</span>
        <span v-if="isStoryteller && storytellerId === player.id" class="badge badge-storyteller">讲述者</span>
      </span>
      <span class="player-score">{{ player.score || 0 }} 分</span>
    </li>
  </ul>
</template>

<script setup>
defineProps({
  players: {
    type: Array,
    default: () => []
  },
  storytellerId: {
    type: String,
    default: null
  },
  isStoryteller: {
    type: Boolean,
    default: false
  }
})
</script>
