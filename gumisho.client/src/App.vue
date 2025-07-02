<script setup>
  import AppNavigation from './components/AppNavigation.vue';
  import AppFooter from './components/AppFooter.vue';
  import { useUserStore } from '@/stores/useUserStore';
  import { useOrderStore } from '@/stores/useOrderStore';
  import { ref, onMounted } from 'vue';




  const isLoading = ref(true);
  const userStore = useUserStore();
  const orderStore = useOrderStore();

  onMounted(async () => {
    //await userStore.onEnter();

    isLoading.value = false;


  });

</script>

<template>

  <nav>
    <AppNavigation />
  </nav>
  <div class="holy-grail-grid">
    <div v-if="isLoading" class="container">
      <span aria-busy="true" class="item">Зареждам Данни...</span>
    </div>
    <RouterView v-else />

  </div>
  <footer>
    <AppFooter />
  </footer>
</template>


<style scoped>

  nav {
    max-width: 100%;
    margin: auto;
    padding: 0.25rem 0.5rem;
    display: flex;
    justify-content: space-between;
    align-items: center;
    background: white;
  }


  header {
    line-height: 1.5;
  }

  .logo {
    display: block;
    margin: 0 auto 2rem;
  }
  @media (min-width: 768px) {
nav{
    display: flex;
    flex-direction: row;
}
  }
    @media (min-width: 1024px) {
      header {
        display: flex;
        place-items: center;
        padding-right: calc(var(--section-gap) / 2);
      }


      .item {
        padding: 20px;
        width: 10em;
      }

      .container {
        font: 1.2em sans-serif;
        height: 200px;
        display: flex;
        align-items: center;
        justify-content: center;
      }
    }
</style>
