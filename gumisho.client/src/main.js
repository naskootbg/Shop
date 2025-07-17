// main.js
import App from './App.vue'
import { ViteSSG } from 'vite-ssg'
import { createPinia } from 'pinia'
import routes from './config/router' // Just the routes array, not a router instance!
import '@picocss/pico'
import '@/styles/reset.css'

export const createApp = ViteSSG(
  App,
  { routes }, // ← Pass routes only, NOT a router instance
  async ({ app, routes, isClient }) => {
    const pinia = createPinia()
    app.use(pinia)

    if (isClient) {
      const { useUserStore } = await import('@/stores/useUserStore')
      await useUserStore().checkSession()
    }
  }
)
