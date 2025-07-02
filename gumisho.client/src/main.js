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
  async ({ app, router, isClient }) => {
    const pinia = createPinia()
    app.use(pinia)

    if (isClient) {
      const { useOrderStore } = await import('@/stores/useOrderStore')
      await useOrderStore().fetchFeed()
    }
  }
)
