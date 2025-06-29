<script setup>
const referral = window.location.hostname;
const year = new Date().getFullYear();

</script>

<template>
    <div>© {{ year }} {{ referral }} </div>
</template>

<style>
div {
    background-color: aliceblue;
    padding: .2rem;
    margin: auto;
}
</style>