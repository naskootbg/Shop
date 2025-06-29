<script setup>
 
</script>

<template>
  <h1>Успещна регистация. Може да влезете в системата.</h1>   
  <router-link v-slot="{ isActive }" :to="{ name: 'login' }">
                    <button type="button" class="primary">
                        ВХОД
                    </button>
                </router-link>
     
</template>

<style scoped>
 
</style>
