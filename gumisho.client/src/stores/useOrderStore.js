// stores/useOrderStore.js
import { defineStore } from 'pinia'
const worker = new Worker(new URL('@/config/filterWorker.js', import.meta.url), { type: 'module' });


//function getDiscount(p) {
//  const vat = parseFloat(p.price_vat || 0)
//  const discounted = parseFloat(p.price_discounted || 0)
//  return vat > 0 && discounted > 0 && discounted < vat
//    ? Math.round(((vat - discounted) / vat) * 100)
//    : 0
//}

export const useOrderStore = defineStore('orderStore', {
  state: () => ({
    allProducts: [],
    filteredProducts: [],
    currentPage: 1,
    perPage: 24,
    searchQuery: '',
    selectedCategory: '',
    discountPercent: 0,
    minPrice: null,
    maxPrice: null,
    freeShipping: false,
    giftIncluded: false,
  }),

  getters: {
    categories(state) {
      return [...new Set(state.allProducts.map(p => p.category).filter(Boolean))].sort()
    },
    suggestions(state) {
      return [...new Set((state.allProducts || []).map(p => p.product_name))].slice(0, 100)
    },
    totalPages(state) {
      return Math.ceil(state.filteredProducts.length / state.perPage) || 1
    },
    paginatedProducts(state) {
      const start = (state.currentPage - 1) * state.perPage
      return state.filteredProducts.slice(start, start + state.perPage)
    },
  },

  actions: {
    getFilters() {
      return {
        searchQuery: this.searchQuery,
        selectedCategory: this.selectedCategory,
        discountPercent: this.discountPercent,
        minPrice: this.minPrice,
        maxPrice: this.maxPrice,
        freeShipping: this.freeShipping,
        giftIncluded: this.giftIncluded,
      };
    },
    async fetchFeed(url = '/feeds.json') {
      try {
        const res = await fetch(url)
        if (!res.ok) throw new Error(`HTTP ${res.status}`)

        const data = await res.json()
        const rawProducts = data.products?.product || []

        // Remove duplicates based on product_code
        const uniqueMap = new Map()
        for (const p of rawProducts) {
          if (p.product_code && !uniqueMap.has(p.product_code)) {
            uniqueMap.set(p.product_code, p)
          }
        }

        this.allProducts = Array.from(uniqueMap.values())
        this.applyFilters()

      } catch (err) {
        console.error('Failed to load feed:', err)
      }
    },
    applyFilters() {
      this.loading = true;

      const safeProducts = this.allProducts.map(p => ({
        product_code: p.product_code,
        product_name: p.product_name,
        price_discounted: p.price_discounted,
        price_vat: p.price_vat,
        category: p.category,
        delivery: p.delivery,
        gift: p.gift,
        product_pic: p.product_pic,
        product_desc: p.product_desc || '',
        product_aff_link: p.product_aff_link || '',
        free_shipping: p.free_shipping,
        gift_included: p.gift_included,
      }));

      // ✅ Send safe data and filters to worker
      worker.postMessage({
        products: safeProducts,
        filters: this.getFilters()
      });

      worker.onmessage = (e) => {
        this.filteredProducts = e.data;
        this.loading = false;
      };
    }

,

    nextPage() {
      if (this.currentPage < this.totalPages) this.currentPage++
    },

    prevPage() {
      if (this.currentPage > 1) this.currentPage--
    },
  },
})
