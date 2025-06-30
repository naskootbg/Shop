<template>
  <div v-if="product" class="product-page">
    <h1 class="title">{{ product.product_name }}</h1>

    <div class="content-box">
      <img :src="product.product_pic" class="product-img" />

      <div class="description" v-html="product.product_desc"></div>
    </div>

    <div class="bottom-box">
      <div class="price">
        <strong class="discounted">{{ product.price_discounted }} лв</strong>
        <span v-if="product.price_vat !== product.price_discounted" class="old">
          {{ product.price_vat }} лв
        </span>
      </div>

      <button class="buy-btn" @click="showModal = true">Купи</button>
    </div>

    <div v-if="showModal" class="modal-overlay" @click.self="showModal = false">
      <div class="modal">
        <h2>Добавено в количката 🎉</h2>
        <p>Имейл за поръчка ще бъде изпратен по-късно с връзка към този продукт.</p>
        <button class="modal-close" @click="showModal = false">Затвори</button>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { useRoute } from 'vue-router'
  import { useOrderStore } from '@/stores/useOrderStore'
  import { computed, ref } from 'vue'
  import { useHead } from '@vueuse/head'

  const route = useRoute()
  const store = useOrderStore()
  const showModal = ref(false)

  const product = computed(() =>
    store.allProducts.find(p => p.product_code === route.params.id)
  )

  useHead({
    title: product.value.product_name,
    meta: [
      { property: 'og:title', content: product.value.product_name },
      { property: 'og:image', content: product.value.product_pic },
      { property: 'og:description', content: product.value.product_desc || '' },
      { property: 'og:url', content: window.location.href },
      { property: 'og:type', content: 'product' }
    ]
  })
</script>

<style scoped>
  *,
  *::before,
  *::after {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
  }
  div{
    background: white;
  }
  .product-page {
    padding: 1.5rem;
    max-width: 1000px;
    margin: auto;
  }

  .title {
    font-size: 24px;
    font-weight: bold;
    margin-bottom: 1.5rem;
    text-align: center;
  }

  .content-box {
    display: flex;
    flex-direction: row;
    gap: 2rem;
    flex-wrap: wrap;
    align-items: flex-start;
  }

  .product-img {
    flex: 1;
    max-width: 400px;
    width: 100%;
    object-fit: contain;
    border: 1px solid #eee;
    border-radius: 8px;
  }

  .description {
    flex: 2;
    font-size: 15px;
    line-height: 1.6;
    white-space: pre-wrap;
  }

  .bottom-box {
    margin-top: 2rem;
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
    gap: 1rem;
    flex-wrap: wrap;
  }

  .price {
    font-size: 20px;
    display: flex;
    align-items: center;
    gap: 1rem;
  }

  .discounted {
    color: #2e7d32;
    font-weight: bold;
  }

  .old {
    text-decoration: line-through;
    color: #888;
    font-size: 16px;
  }

  .buy-btn {
    padding: 10px 18px;
    background: #2e7d32;
    color: white;
    font-weight: bold;
    border: none;
    border-radius: 8px;
    cursor: pointer;
  }

    .buy-btn:hover {
      background: #256d2b;
    }

  .modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.7);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
  }

  .modal {
    background: white;
    padding: 2rem;
    border-radius: 12px;
    text-align: center;
    max-width: 400px;
  }

  .modal-close {
    margin-top: 1rem;
    padding: 8px 16px;
    background: #aaa;
    color: white;
    border: none;
    border-radius: 6px;
    cursor: pointer;
  }
</style>
