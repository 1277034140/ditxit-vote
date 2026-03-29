<template>
  <div class="card fade-in">
    <h2>{{ title }}</h2>
    <div class="status-bar">
      {{ message }}<span class="waiting-dots"></span>
    </div>
    <VoteProgress :voters="voters" />
  </div>
</template>

<script setup>
import VoteProgress from './VoteProgress.vue'

defineProps({
  title: {
    type: String,
    default: '等待中'
  },
  message: {
    type: String,
    default: '其他玩家'
  },
  voters: {
    type: Array,
    default: () => []
  }
})
</script>
