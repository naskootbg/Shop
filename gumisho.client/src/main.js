import { createApp } from 'vue';
import App from './App.vue';
import { createPinia } from 'pinia';
import router from './config/router';
import '@picocss/pico'; // ✅ Keep CSS import
import '@/styles/reset.css';
 
const pinia = createPinia();

createApp(App)
  .use(pinia)
  .use(router)
  .mount("#app");

  import { useOrderStore } from '@/stores/useOrderStore'
useOrderStore().fetchFeed()
