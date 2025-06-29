<script setup>
  import { ref } from "vue";
  import { useUserStore } from '@/stores/useUserStore';
  import { useOrderStore } from '@/stores/useOrderStore';


  const userStore = useUserStore();
  const orderStore = useOrderStore();

  orderStore.Reset();
</script>

<template>
 
</template>

<style scoped>

</style>
