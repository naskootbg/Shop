<script setup>
  import { ref } from "vue";
  import { useUserStore } from '@/stores/useUserStore';
  import { useOrderStore } from '@/stores/useOrderStore';


  const userStore = useUserStore();
  const orderStore = useOrderStore();


  async function Logout() {
    await userStore.LogOut();
    
  }
</script>

<template>
  <router-link to="/" class="card">
    <button @click="Logout()" type="button" class="danger">
      ИЗХОД
    </button>
  </router-link>
</template>

<style scoped>
  .danger {
    background-color: red;
  }
</style>
