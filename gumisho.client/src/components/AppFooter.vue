<script setup>
const referral = window.location.hostname;
const year = new Date().getFullYear();

</script>

<template>
    <div>© {{ year }} {{ referral }} </div>
</template>

<style>
  *,
  *::before,
  *::after {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
  }

  div {
    background: white;
  
    padding: .2rem;
    margin: auto;
}
</style>
