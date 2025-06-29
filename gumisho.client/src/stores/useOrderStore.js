// stores/useOrderStore.js
import { defineStore } from 'pinia'

function getDiscount(p) {
  const vat = parseFloat(p.price_vat || 0)
  const discounted = parseFloat(p.price_discounted || 0)
  return vat > 0 && discounted > 0 && discounted < vat
    ? Math.round(((vat - discounted) / vat) * 100)
    : 0
}

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
    async fetchFeed(url = '/feeds.json') {
      try {
        const res = await fetch(url)
        if (!res.ok) throw new Error(`HTTP ${res.status}`)
        const data = await res.json()
        this.allProducts = data.products?.product || []
        this.applyFilters()
      } catch (err) {
        console.error('Failed to load feed:', err)
      }
    },

    applyFilters() {
      this.currentPage = 1

      this.filteredProducts = this.allProducts.filter(p => {
        const name = p.product_name?.toLowerCase() || ''
        const price = parseFloat(p.price_discounted || 0)
        const discount = getDiscount(p)

        return (
          discount >= this.discountPercent &&
          name.includes(this.searchQuery.toLowerCase()) &&
          (!this.selectedCategory || p.category === this.selectedCategory) &&
          (!this.minPrice || price >= this.minPrice) &&
          (!this.maxPrice || price <= this.maxPrice) &&
          (!this.freeShipping || p.free_shipping === '1') &&
          (!this.giftIncluded || p.gift_included === '1')
        )
      })
    },

    nextPage() {
      if (this.currentPage < this.totalPages) this.currentPage++
    },

    prevPage() {
      if (this.currentPage > 1) this.currentPage--
    },
  },
})
