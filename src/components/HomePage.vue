<template>
  <div class="card fade-in">
    <h2>选择操作</h2>
    <button class="btn btn-primary" @click="$emit('create')">创建房间</button>
    <button class="btn btn-secondary" @click="$emit('join')">加入房间</button>
  </div>
</template>

<script setup>
defineEmits(['create', 'join'])
</script>
