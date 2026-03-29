import { initializeApp } from 'firebase/app'
import { getDatabase } from 'firebase/database'

const firebaseConfig = {
  apiKey: "AIzaSyC-1aiEF1yjcbg_jESVpXlM_Jdn7k4r79U",
  authDomain: "xagameproject.firebaseapp.com",
  projectId: "xagameproject",
  storageBucket: "xagameproject.firebasestorage.app",
  databaseURL: "https://xagameproject-default-rtdb.asia-southeast1.firebasedatabase.app",
  messagingSenderId: "1018819296928",
  appId: "1:1018819296928:web:b1d0dc3dc4a8c51445d3c5",
  measurementId: "G-L4VPZECQCD"
}

let app = null
let database = null
let initError = null

export function initFirebase() {
  if (initError) throw initError
  if (!app) {
    try {
      app = initializeApp(firebaseConfig)
      database = getDatabase(app)
    } catch (e) {
      initError = e
      throw e
    }
  }
  return { app, database }
}

export function getDb() {
  if (!database) {
    try {
      initFirebase()
    } catch (e) {
      console.error('Firebase 未初始化:', e.message)
      return null
    }
  }
  return database
}

export { app }
