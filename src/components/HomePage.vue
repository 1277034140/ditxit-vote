<template>
  <div class="card fade-in">
    <h2>选择操作</h2>
    <button class="btn btn-primary" @click="$emit('create')">创建房间</button>
    <button class="btn btn-secondary" @click="$emit('join')">加入房间</button>
    <button class="btn btn-danger" @click="handleClearData">清空房间缓存</button>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { getDb } from '@/config/firebase.js'
import { ref as dbRef, remove } from 'firebase/database'

const emit = defineEmits(['create', 'join'])

const clearing = ref(false)

async function handleClearData() {
  if (!confirm('确定清空所有房间数据？此操作不可恢复！')) return
  clearing.value = true
  try {
    const db = getDb()
    if (db) {
      await remove(dbRef(db, 'rooms'))
      alert('已清空所有房间缓存')
    } else {
      alert('Firebase 未初始化')
    }
  } catch (e) {
    alert('清空失败: ' + e.message)
  } finally {
    clearing.value = false
  }
}
</script>
