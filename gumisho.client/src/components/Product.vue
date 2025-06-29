<template>
  <div class="catalog">
    <!-- Filters -->
    <div class="filters">
      <input v-model="store.searchQuery"
             @input="store.applyFilters"
             placeholder="Search products..."
             list="suggestions"
             class="input" />
      <datalist id="suggestions">
        <option v-for="s in store.suggestions" :key="s" :value="s" />
      </datalist>

      <select v-model="store.selectedCategory" @change="store.applyFilters" class="select">
        <option value="">All Categories</option>
        <option v-for="cat in store.categories" :key="cat">{{ cat }}</option>
      </select>

      <input type="range" min="0" max="100" v-model="store.discountPercent" @input="store.applyFilters" />
      <label>Min Discount: {{ store.discountPercent }}%</label>

      <input type="number" v-model.number="store.minPrice" @change="store.applyFilters" placeholder="Min Price" class="input" />
      <input type="number" v-model.number="store.maxPrice" @change="store.applyFilters" placeholder="Max Price" class="input" />

      <label><input type="checkbox" v-model="store.freeShipping" @change="store.applyFilters" /> Free Shipping</label>
      <label><input type="checkbox" v-model="store.giftIncluded" @change="store.applyFilters" /> Gift Included</label>
    </div>

    <!-- Product Grid -->
    <div class="grid">
      <div v-for="product in store.paginatedProducts" :key="product.product_code" class="card" @click="openModal(product)">
        <template v-if="discount(product) > 0">
          <img :src="product.product_pic" alt="" class="image" />
          <div class="discount-badge">-{{ discount(product) }}%</div>
          <h3 class="title">{{ product.product_name }}</h3>
          <p class="price">{{ product.price_discounted }} лв</p>
          <small class="price-old">{{ product.price_vat }} лв</small>
        </template>
      </div>
    </div>

    <!-- Pagination -->
    <div class="pagination">
      <button @click="store.prevPage" :disabled="store.currentPage === 1">◀</button>
      <span>Page {{ store.currentPage }} / {{ store.totalPages }}</span>
      <button @click="store.nextPage" :disabled="store.currentPage === store.totalPages">▶</button>
    </div>
  </div>

  <!-- Modal -->
  <div v-if="selectedProduct" class="modal-overlay" @click.self="closeModal">
    <div class="modal">
      <button class="modal-close" @click="closeModal">×</button>
      <img :src="selectedProduct.product_pic" alt="" class="modal-image" />
      <h2>{{ selectedProduct.product_name }}</h2>
      <p v-if="selectedProduct.product_desc" v-html="selectedProduct.product_desc"></p>

      <div class="modal-price">
        <span class="new">{{ selectedProduct.price_discounted }} лв</span>
        <span class="old" v-if="selectedProduct.price_discounted !== selectedProduct.price_vat">{{ selectedProduct.price_vat }} лв</span>
        <span v-if="discount(selectedProduct) > 0" class="badge">-{{ discount(selectedProduct) }}%</span>
      </div>

      <a :href="'https:' + selectedProduct.product_aff_link" target="_blank" class="buy-btn">Купи</a>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useOrderStore } from '@/stores/useOrderStore'

const store = useOrderStore()
const selectedProduct = ref(null)

onMounted(() => {
  store.fetchFeed()
})

function openModal(product) {
  selectedProduct.value = product
}
function closeModal() {
  selectedProduct.value = null
}
function discount(p) {
  const vat = parseFloat(p.price_vat || 0)
  const discounted = parseFloat(p.price_discounted || 0)
  if (vat === 0 || discounted === 0 || discounted >= vat) return 0
  return Math.round(((vat - discounted) / vat) * 100)
}
</script>



<style scoped>
  .catalog {
    padding: 1rem;
  }

  .filters {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    margin-bottom: 1rem;
    align-items: center;
  }

  .input,
  .select {
    padding: 6px 10px;
    font-size: 14px;
    border-radius: 4px;
    border: 1px solid #ccc;
  }

  .grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 1rem;
  }

  .card {
    position: relative;
    border: 1px solid #ccc;
    padding: 10px;
    border-radius: 12px;
    background: #fff;
    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
    display: flex;
    flex-direction: column;
    align-items: center;
  }

  .image {
    width: 100%;
    height: 140px;
    object-fit: contain;
    margin-bottom: 0.5rem;
  }

  .title {
    font-size: 14px;
    font-weight: bold;
    margin-bottom: 4px;
    text-align: center;
  }

  .price {
    color: #111;
    font-weight: bold;
    font-size: 16px;
  }

  .price-old {
    text-decoration: line-through;
    color: #888;
    font-size: 12px;
  }

  .discount-badge {
    position: absolute;
    top: 8px;
    right: 8px;
    background: red;
    color: white;
    padding: 2px 6px;
    font-size: 12px;
    border-radius: 6px;
  }

  .pagination {
    margin-top: 1rem;
    display: flex;
    gap: 1rem;
    justify-content: center;
    align-items: center;
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
    background: #fff;
    max-width: 600px;
    width: 90%;
    padding: 1.5rem;
    border-radius: 12px;
    position: relative;
    max-height: 90vh;
    overflow-y: auto;
  }

  .modal-close {
    position: absolute;
    top: 8px;
    right: 12px;
    background: transparent;
    border: none;
    font-size: 28px;
    cursor: pointer;
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
