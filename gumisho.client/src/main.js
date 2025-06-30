// main.js
import App from './App.vue'
import { ViteSSG } from 'vite-ssg'
import { createPinia } from 'pinia'
import router from './config/router'
import '@picocss/pico'
import '@/styles/reset.css'

export const createApp = ViteSSG(
  App,
  { routes: router.options.routes }, // ViteSSG needs routes here
  async ({ app, router, isClient }) => {
    const pinia = createPinia()
    app.use(pinia)
    app.use(router)

    if (isClient) {
      // ✅ Load feed only in browser (avoid during prerender)
      const { useOrderStore } = await import('@/stores/useOrderStore')
      await useOrderStore().fetchFeed()
    }
  }
)
