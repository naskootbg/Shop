<template>
  <div class="cart-container">
    <h2 class="cart-title">🛒 Продукти в количката</h2>

    <div v-if="productsInCard.length === 0">
      <p>Количката е празна.</p>
    </div>

    <div v-else>
      <div class="product-list">
        <div v-for="product in productsInCard"
             :key="product.product_code"
             class="product-card">
          <img :src="product.product_pic"
               alt="Product"
               class="product-image" />
          <div class="product-info">
            <h3 class="product-name">{{ product.product_name }}</h3>
            <p class="product-desc" v-html="product.product_desc"></p>
            <p class="product-price">
              {{ Number(product.price_discounted).toFixed(2) }} {{ product.currency.replace("leva","лв") }}
            </p>
          </div>
          <div class="product-actions">
            <button @click="removeItem(product.product_code)"
                    class="remove-button"
                    title="Премахни">
              🗑️
            </button>
            <a :href="fixAffLink(product.product_aff_link)"
               target="_blank"
               class="buy-button">
              Купи сега
            </a>
          </div>
        </div>
      </div>

      <div class="clear-container">
        <button @click="clearCard" class="clear-button">
          Изчисти количката
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { computed } from 'vue';
  import { useUserStore } from '@/stores/useUserStore';
  import { useOrderStore } from '@/stores/useOrderStore';

  const userStore = useUserStore();
  const orderStore = useOrderStore();

  const productsInCard = computed(() =>
    userStore.card?.productCodes?.map(code =>
      orderStore.allProducts.find(p => p.product_code === code)
    ).filter(Boolean)
  );

  function fixAffLink(link) {
    return link.startsWith('//') ? 'https:' + link : link;
  }

  function removeItem(code) {
    userStore.RemoveItem(code);
  }

  function clearCard() {
    userStore.ClearCard();
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

  div {
    background: white;
  }
  .cart-container {
    padding: 16px;
  }

  .cart-title {
    font-size: 1.25rem;
    font-weight: bold;
    margin-bottom: 16px;
  }

  .product-list {
    display: flex;
    flex-direction: column;
    gap: 16px;
   
  }

  .product-card {
    display: flex;
    align-items: flex-start;
    border: 1px solid #ddd;
    padding: 16px;
    border-radius: 12px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
    width: 90%;
  }

  .product-image {
    width: 96px;
    height: 96px;
    object-fit: contain;
    margin-right: 16px;
  }

  .product-info {
    flex: 1;
  }

  .product-name {
    font-weight: 600;
    margin-bottom: 8px;
  }

  .product-desc {
    font-size: 0.875rem;
    color: #666;
    max-height: 3.2em;
    overflow: hidden;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    margin-bottom: 8px;
  }

  .product-price {
    font-weight: bold;
    color: #15803d; /* green */
  }

  .product-actions {
    display: flex;
    flex-direction: column;
    align-items: flex-end;
    gap: 8px;
    margin-left: 16px;
  }

  .remove-button {
    color: #dc2626; /* red */
    background: none;
    border: none;
    font-size: 1.2rem;
    cursor: pointer;
  }

    .remove-button:hover {
      color: #b91c1c;
    }

  .buy-button {
    background-color: #2563eb; /* blue */
    color: white;
    padding: 6px 12px;
    border-radius: 6px;
    text-decoration: none;
    font-size: 0.875rem;
  }

    .buy-button:hover {
      background-color: #1e40af;
    }

  .clear-container {
    text-align: right;
    margin-top: 24px;
  }

  .clear-button {
    background-color: #dc2626; /* red */
    color: white;
    padding: 10px 16px;
    border: none;
    border-radius: 8px;
    cursor: pointer;
  }

    .clear-button:hover {
      background-color: #b91c1c;
    }
</style>
