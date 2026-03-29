import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import { initFirebase } from './config/firebase.js'
import './assets/styles/main.css'

try {
  initFirebase()
} catch (e) {
  console.warn('Firebase 初始化失败，应用将以离线模式运行:', e.message)
}

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.mount('#app')
