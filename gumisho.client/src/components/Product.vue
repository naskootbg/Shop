<template>
  <div class="catalog">
    <div class="layout">
      <!-- Filters -->
      <div class="filters">
        <input v-model="store.searchQuery"
               @input="store.applyFilters"
               placeholder="🔍 Търсене..."
               list="suggestions"
               class="input" />
        <datalist id="suggestions">
          <option v-for="s in store.suggestions" :key="s" :value="s" />
        </datalist>
        <label for="selectedCategory">
          Категория:
          <select v-model="store.selectedCategory" @change="store.applyFilters" class="select" name="selectedCategory" id="selectedCategory">
            <option value="">Категории</option>
            <option v-for="cat in store.categories" :key="cat">{{ cat }}</option>
          </select>
        </label>
        <label for="discountPercent">
          Мин. Отстъпка: {{ store.discountPercent }}%
          <input type="range"
                 min="0"
                 max="100"
                 v-model.number="store.discountPercent"
                 @input="store.applyFilters" name="discountPercent" id="discountPercent" class="input-range" />

        </label>
        <label for="maxPrice">
          Макс. Цена до: {{ store.maxPrice }} лв
          <input type="range"
                 :min="0"
                 :max="store.maxAvailablePrice"
                 v-model.number="store.maxPrice"
                 @input="store.applyFilters"
                 class="input-range"
                 id="maxPrice" />
        </label>

        <label>
          Сортиране:
          <select v-model="store.sortOrder" @change="store.applyFilters" class="select">
            <option value="">Без сортиране</option>
            <option value="price_asc">Цена ↑</option>
            <option value="price_desc">Цена ↓</option>
            <option value="discount_asc">Отстъпка ↑</option>
            <option value="discount_desc">Отстъпка ↓</option>
          </select>
        </label>


        <label><input type="checkbox" v-model="store.freeShipping" @change="store.applyFilters" /> Безплатна доставка</label>
      </div>

      <!-- Product Grid -->
      <div class="content">
        <div class="grid">
          <div v-for="product in store.paginatedProducts"
               :key="product.product_code"
               class="card">
            <router-link :to="`/${encodeURIComponent(product.product_code)}/${slugify(product.product_name)}`"
                         class="router-link">
             
              <img :src="localImage(product)"
                   :alt="product.product_name"
                   @error="onImageError($event, product.product_pic)"
                   class="image"
                   loading="lazy" />
              <div class="discount-badge">-{{ discount(product) }}%</div>
              <h3 class="title">{{ product.product_name }}</h3>
              <p class="price">{{ Number(product.price_discounted).toFixed(2) }} лв</p>
              <small class="price-old" v-if="product.price_discounted !== product.price_vat">
                {{ Number(product.price_vat).toFixed(2) }} лв
              </small>
            </router-link>

            <!-- Sticky Footer -->
            <div class="card-footer">
              <button @click="openModal(product)">🛒 Добави</button>
            </div>
          </div>
        </div>

        <!-- Pagination -->
        <div class="pagination">
          <button @click="store.prevPage" :disabled="store.currentPage === 1">◀</button>
          <span>Page {{ store.currentPage }} / {{ store.totalPages }}</span>
          <button @click="store.nextPage" :disabled="store.currentPage === store.totalPages">▶</button>
        </div>
      </div>
    </div>

    <!-- Modal -->
    <div v-if="selectedProduct" class="modal-overlay" @click.self="closeModal">
      <div class="modal">
        <button class="modal-close" @click="closeModal">×</button>
        <img :src="localImage(selectedProduct)"
             @error="onImageError($event, selectedProduct.product_pic)"
             class="modal-image" />
        <h2>{{ selectedProduct.product_name }}</h2>
        <p v-if="selectedProduct.product_desc" v-html="formattedDescription(selectedProduct)"></p>

        <div class="modal-price">
          <span class="new">{{ selectedProduct.price_discounted }} лв</span>
          <span class="old" v-if="selectedProduct.price_discounted !== selectedProduct.price_vat">{{ selectedProduct.price_vat }} лв</span>
          <span v-if="discount(selectedProduct) > 0" class="badge">-{{ discount(selectedProduct) }}%</span>
        </div>

        <a :href="'https:' + selectedProduct.product_aff_link" target="_blank" class="buy-btn">Купи</a>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, watch, computed } from 'vue'
  import { useOrderStore } from '@/stores/useOrderStore'
  import { useUserStore } from '@/stores/useUserStore'
  import { slugify } from '@/api/slugify.js'
  import { useRoute } from 'vue-router'
  import { useHead } from '@vueuse/head'

  const route = useRoute()
  const store = useOrderStore()
  const userStore = useUserStore()
  const selectedProduct = ref(null)

  const pageTitle = computed(() => {
    if (route.params.value) return `Продукти с отстъпка над ${route.params.value}%`
    if (route.params.category) return `Категория: Промоции на ${route.params.category}`
    if (route.params.search) return `Резултати за промоции на: ${route.params.search}`
    if (route.params.tag) return `Търсене на: ${route.params.tag} на промоция`
    return 'Продуктите с най-добри цени в България'
  })

  const pageDescription = computed(() => `Голям избор на продукти в промоции. Ежеседмично събираме информация от над 50 големи онлайн магазини. ${pageTitle.value}`)

  function updateHead() {
    useHead({
      title: pageTitle.value,
      meta: [
        { name: 'description', content: pageDescription.value },
        { property: 'og:title', content: pageTitle.value },
        { property: 'og:description', content: pageDescription.value },
        { property: 'og:url', content: window.location.href },
      ],
    })
  }
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
    return `/images/${maxCat}/${filename}`;
  }

  function onImageError(event, fallbackUrl) {
    event.target.src = httpFix(fallbackUrl);
  }
  
  let lastRoute = ''

  watch(
    () => route.fullPath,
    async (newVal) => {
      if (newVal === lastRoute) return
      lastRoute = newVal
      updateHead()
      await loadProducts()
    },
    { immediate: true }
  )

  function httpFix(text) {
    return text.replace('http:', 'https:')
  }

  function formattedDescription(product) {
    const desc = product.product_desc?.trim() || ''
    const link = `<a href="https:${product.product_aff_link}" target="_blank" style="color: #2e7d32; text-decoration: underline;">Научи повече</a>`
    if (desc.endsWith('...') || desc.endsWith('…')) return desc + ' ' + link
    return desc
  }

  function discount(p) {
    const vat = parseFloat(p.price_vat || 0)
    const discounted = parseFloat(p.price_discounted || 0)
    return vat > 0 && discounted > 0 && discounted < vat
      ? Math.round(((vat - discounted) / vat) * 100)
      : 0
  }

  function openModal(product) {
    selectedProduct.value = product;
    userStore.AddItem(product.product_code);
  }
  function closeModal() {
    selectedProduct.value = null
  }

  async function loadProducts() {
    // Parse route params into filters
    const discountParam = route.params.value ? parseInt(route.params.value) : 0;
    const categoryParam = route.params.category || '';
    const tagParam = route.params.tag || '';

    // Set store filters from route
    store.discountPercent = discountParam;
    store.selectedCategory = categoryParam;
    store.searchQuery = tagParam;

    // Load feed if not loaded, then apply filters
    await store.applyFilters();
    if (!store.maxPrice) {
      store.maxPrice = store.maxAvailablePrice;
      store.applyFilters(); // apply again if necessary
    }
  }


  //onMounted(loadProducts)
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

  .catalog {
    padding: 1rem;
  }

  .layout {
    display: flex;
    gap: 1rem;
    align-items: flex-start; /* 👈 key */
  }

  /* Sidebar Filters */
  .filters {
    flex: 0 0 240px;
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    padding-right: 1rem;
    border-right: 1px solid #ddd;
    background: white;
  }

  .input,
  .select {
    padding: 6px 10px;
    font-size: 14px;
    border-radius: 4px;
    border: 1px solid #ccc;
  }

  .content {
    flex: 1;
    display: flex;
    flex-direction: column;
  }

  /* Products Grid */
  .grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
    gap: 1rem;
  }

  .card {
    position: relative;
    border: 1px solid #ccc;
    border-radius: 12px;
    background: #fff;
    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    overflow: hidden;
  }

  .router-link {
    display: flex;
    flex-direction: column;
    align-items: center;
    text-decoration: none;
    color: inherit;
    padding: 10px;
    flex-grow: 1;
  }

  .image {
    width: 100%;
    height: 140px;
    object-fit: contain;
    margin-bottom: 0.5rem;
    background-color: white;
  }

  .card-footer {
    padding: 0.5rem 1rem;
    border-top: 1px solid #eee;
    text-align: center;
    background: #fafafa;
  }

    .card-footer button {
      padding: 6px 12px;
      font-size: 14px;
      background-color: #2e7d32;
      color: white;
      border: none;
      border-radius: 6px;
      cursor: pointer;
    }

      .card-footer button:hover {
        background-color: #25682a;
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

  /* Modal */
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
    font-size: .7rem;
    width: 74%;
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

  /* Mobile: Stack filters above grid */
  @media (max-width: 768px) {
    .layout {
      flex-direction: column;
    }
    .input-range {
      width: 100%;
    }
    .filters {
      flex-direction: row;
      flex-wrap: wrap;
      border-right: none;
      border-bottom: 1px solid #ddd;
      padding-right: 0;
      margin-bottom: 1rem;
    }

      .filters input,
      .filters select {
        flex: 1 1 45%;
      }
    .grid {
      grid-template-columns: repeat(3, 1fr);
    }
  }

  label {
    font-size: .8rem;
  }
</style>
