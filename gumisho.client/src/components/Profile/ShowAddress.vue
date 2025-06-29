<script setup>
  import { ref } from 'vue';
  import { ShowAdres } from '@/api/adress';

  
  const showOne = await ShowAdres();

</script>
<template>
</template>
<style scoped>
</style>
