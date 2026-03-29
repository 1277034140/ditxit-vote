<template>
  <div class="card fade-in">
    <h2>创建房间</h2>
    <div v-if="error" class="error-msg">{{ error }}</div>
    <div class="room-code">{{ roomCode }}</div>
    <QRCodeShare :roomCode="roomCode" />
    <button class="btn btn-primary" @click="enterRoom">进入房间</button>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import QRCodeShare from './QRCodeShare.vue'

const props = defineProps({
  roomCode: {
    type: String,
    required: true
  }
})

const emit = defineEmits(['enter'])

const error = ref('')

function enterRoom() {
  const name = prompt('请输入你的名字:')
  if (!name || name.trim() === '') {
    return
  }
  emit('enter', name.trim())
}
</script>
