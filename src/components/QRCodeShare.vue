<template>
  <div class="qrcode-share">
    <div id="qrcode-container" class="qrcode-container"></div>
    <p class="share-hint">分享二维码或房间码给朋友</p>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import QRCode from 'qrcode'

const props = defineProps({
  roomCode: {
    type: String,
    required: true
  }
})

const container = ref(null)

function generateQR() {
  if (!container.value || !props.roomCode) return

  const joinUrl = `${window.location.origin}${window.location.pathname}?room=${props.roomCode}`

  container.value.innerHTML = ''

  QRCode.toCanvas(document.createElement('canvas'), joinUrl, {
    width: 200,
    margin: 2
  }, (err, canvas) => {
    if (!err) {
      canvas.style.borderRadius = '8px'
      container.value.appendChild(canvas)
    }
  })
}

onMounted(() => {
  generateQR()
})

watch(() => props.roomCode, () => {
  generateQR()
})
</script>

<style scoped>
.qrcode-container {
  display: flex;
  justify-content: center;
  margin: 20px 0;
}

.share-hint {
  text-align: center;
  color: #666;
  margin: 12px 0;
  font-size: 14px;
}
</style>
