<script setup>
  import { ref, watch } from 'vue'
  import { useOrderStore } from '@/stores/useOrderStore'
  

  const orderStore = useOrderStore();
  const selectedCategory = ref(orderStore.selectedCategory);
  const searchQuery = ref(orderStore.searchQuery);
  const discountPercent = ref(orderStore.discountPercent);
  const maxPrice = ref(orderStore.maxPrice);
  const freeShipping = ref(orderStore.freeShipping);
  const showPrice = ref(false);
 async function clearCat() {
    orderStore.selectedCategory = "";
   await orderStore.applyFilters();
  }
  async function clearQ() {
    orderStore.searchQuery = "";
    await orderStore.applyFilters();
  }
  async function clearD() {
    orderStore.discountPercent = 0;
    await orderStore.applyFilters();
  }
  async function clearM() {
    orderStore.maxPrice = orderStore.maxPriceAll;
    await orderStore.applyFilters();
    showPrice.value = false;
   
  }
  async function clearS() {
    orderStore.freeShipping = 0;
    await orderStore.applyFilters();
 

  }
  watch(() => orderStore.maxPrice, (newVal, oldVal) => {
    if (newVal !== oldVal) {
      showPrice.value = true
    }
    if (newVal == 0) {
      orderStore.maxPrice = orderStore.maxPriceAll;
    }
  })

  function slugify(text) {
    return text
      .toString()
      .toLowerCase()
      .trim()
      .replace(/[\s,]+/g, '-')       // spaces and commas → dashes
      .replace(/[^\w\-а-яА-Я]+/g, '') // remove special characters except Cyrillic
      .replace(/\-\-+/g, '-');        // multiple dashes → one
  }
</script>

<template>
  <div>
    <span v-if="orderStore.selectedCategory.length > 0">
      <RouterLink :to="`/category/${slugify(orderStore.selectedCategory)}`"
                  class="link">
        {{ orderStore.selectedCategory }}
      </RouterLink>
      <span @click="clearCat">❌</span>
    </span>

    <span v-if="orderStore.searchQuery.length > 0">
      <RouterLink :to="`/tag/${orderStore.searchQuery}`"
                  class="link">
        🔍{{ orderStore.searchQuery }}
      </RouterLink>
      <span @click="clearQ">❌</span>
    </span>

    <span v-if="Number(orderStore.discountPercent) > 0">Мин {{orderStore.discountPercent}}% <span @click="clearD">❌</span></span>
    <span v-if="showPrice">Макс {{orderStore.maxPrice}}лв <span @click="clearM">❌</span></span>
    <span v-if="orderStore.freeShipping >0">Безплатна доставка <span @click="clearS">❌</span></span>
  </div>
</template>
<style scoped>
  div{
    display: flex;
    gap: 1rem;
    background: white;
    font-size: .7rem;
  }
  .link {
    text-decoration: none;
 
  }
</style>
