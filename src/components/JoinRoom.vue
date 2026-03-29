<template>
  <div class="card fade-in">
    <h2>加入房间</h2>
    <div v-if="error" class="error-msg">{{ error }}</div>
    <input
      v-model="joinCode"
      type="text"
      placeholder="输入房间码"
      maxlength="6"
      class="code-input"
      @input="joinCode = joinCode.toUpperCase()"
    />
    <input
      v-model="playerName"
      type="text"
      placeholder="输入你的名字"
      maxlength="20"
      @keyup.enter="handleJoin"
    />
    <button class="btn btn-primary" @click="handleJoin" :disabled="!canJoin">加入</button>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  initialCode: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['join'])

const joinCode = ref(props.initialCode)
const playerName = ref('')
const error = ref('')

const canJoin = computed(() => {
  return joinCode.value.length === 6 && playerName.value.trim().length > 0
})

function handleJoin() {
  if (!canJoin.value) {
    if (joinCode.value.length !== 6) {
      error.value = '请输入6位房间码'
    } else if (!playerName.value.trim()) {
      error.value = '请输入名字'
    }
    return
  }
  error.value = ''
  emit('join', {
    code: joinCode.value,
    name: playerName.value.trim()
  })
}
</script>

<style scoped>
.code-input {
  text-transform: uppercase;
  text-align: center;
  font-size: 24px;
  letter-spacing: 4px;
}
</style>
