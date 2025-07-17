<template>
  <div v-if="product" class="product-page">

    <h1 class="title">{{ product.product_name }}</h1>

    <div class="content-box">

      <img :src="localImage(product)"
           :alt="product.product_name"
           @error="onImageError($event, product.product_pic)"
           class="product-img"
           loading="lazy" />
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
        <button class="modal-close" @click="showModal = false">×</button>
        <img :src="localImage(product)"
             @error="onImageError($event, product.product_pic)"
             class="modal-image" />
        <h2>{{ product.product_name }}</h2>
        <p v-if="product.product_desc" v-html="formattedDescription(product)"></p>

        <div class="modal-price">
          <span class="new">{{ product.price_discounted }} лв</span>
          <span class="old" v-if="product.price_discounted !== product.price_vat">{{ product.price_vat }} лв</span>
          <span v-if="discount(product) > 0" class="badge">-{{ discount(product) }}%</span>
        </div>

        <a :href="'https:' + product.product_aff_link" target="_blank" class="buy-btn">Купи</a>
      </div>

    </div>
    <Subscribe v-if="userStore.isReady" :product="product" />
  </div>
</template>

<script setup>
  import { useRoute } from 'vue-router'
  import { useOrderStore } from '@/stores/useOrderStore'
  import { useUserStore } from '@/stores/useUserStore'
  import { computed, ref } from 'vue'
  import { useHead } from '@vueuse/head'
  import Subscribe from '@/components/Subscribe.vue'

  const route = useRoute()
  const store = useOrderStore()
  const userStore = useUserStore()
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

  function slugify2(str) {
    return (str || '')
      .toLowerCase()
      .replace(/\s+/g, '-')                    // Replace spaces with dashes
      .replace(/[^a-zа-яё0-9\-]+/giu, '')      // Allow Cyrillic + Latin + digits + dash
      .replace(/--+/g, '-')                    // Replace multiple dashes with one
      .replace(/^-+|-+$/g, '');                // Trim dashes from start/end
  }
  function localImage(product) {
    const category = slugify2(product.category || '');
    const name = slugify2(product.product_name || '');
    const maxName = name.length > 60 ? name.substring(0, 60) : name;
    const maxCat = category.length > 40 ? category.substring(0, 40) : category;
    const filename = `${maxName}-${product.product_code}.webp`;
   // console.log(`/images/${maxCat}/${filename}`);
    return `/images/${maxCat}/${filename}`;
  }

  function onImageError(event, fallbackUrl) {
    event.target.src = httpFix(fallbackUrl);
  }

  function formattedDescription(product) {
    const desc = product.product_desc?.trim() || ''
    const link = `<a href="https:${product.product_aff_link}" target="_blank" style="color: #2e7d32; text-decoration: underline;">Научи повече</a>`

    // If it ends in three dots (with optional space), append link
    if (desc.endsWith('...') || desc.endsWith('…')) {
      return desc + ' ' + link
    }

    return desc
  }
  function discount(p) {
    const vat = parseFloat(p.price_vat || 0)
    const discounted = parseFloat(p.price_discounted || 0)
    if (vat === 0 || discounted === 0 || discounted >= vat) return 0
    return Math.round(((vat - discounted) / vat) * 100)
  }
  function httpFix(text){
    return text.replace("http:","https:");
  }
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
    z-index: 50;
  }

  .modal {
    position: relative; /* <-- add this */
    background: white;
    padding: 2rem;
    border-radius: 12px;
    text-align: center;
    width: 74%;
  }

  .modal-close {
    position: absolute;
    top: 8px;
    right: 12px;
    background: transparent;
    border: none;
    font-size: 28px;
    font-weight: bold;
    color: #333; /* <- Ensure it's visible */
    cursor: pointer;
    z-index: 10;
  }

    .modal-close:hover {
      color: red;
    }


  .modal-image {
    width: 100%;
    max-height: 300px;
    object-fit: contain;
    margin-bottom: 1rem;
  }

  .modal-price {
    margin-top: 1rem;
    font-size: 18px;
  }

    .modal-price .new {
      font-weight: bold;
      color: #111;
    }

    .modal-price .old {
      margin-left: 10px;
      text-decoration: line-through;
      color: #888;
    }

  .badge {
    margin-left: 10px;
    background: red;
    color: white;
    padding: 2px 6px;
    font-size: 12px;
    border-radius: 4px;
  }

  .buy-btn {
    display: inline-block;
    margin-top: 1rem;
    padding: 8px 16px;
    background: #2e7d32;
    color: white;
    border-radius: 6px;
    text-decoration: none;
    font-weight: bold;
  }

</style>
