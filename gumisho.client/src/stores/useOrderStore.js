import { defineStore } from 'pinia';

export const useOrderStore = defineStore('orderStore', {
  state: () => ({
    allProducts: [],
    filteredProducts: [],
    currentPage: 1,
    perPage: 24,
    searchQuery: '',
    selectedCategory: '',
    discountPercent: 0,
    maxPrice: null,
    freeShipping: false,
    giftIncluded: false,
    feedLoaded: false,
    sortOrder: '',
    priceChangedManually: false,
  }),

  getters: {
    haveFilteredProducts: (s) =>
      Array.isArray(s.filteredProducts) && s.filteredProducts.length > 0,

    haveProductsWithTag: (s) =>
      !!s.searchQuery &&
      s.allProducts.some((p) =>
        p.product_name.toLowerCase().includes(s.searchQuery.toLowerCase())
      ),

    haveProductsInCategory: (s) =>
      !!s.selectedCategory &&
      s.allProducts.some((p) => p.category === s.selectedCategory),

    categories(state) {
      return [...new Set(state.allProducts.map(p => p.category).filter(Boolean))].sort();
    },

    maxPriceAll: (state) => {
      if (!state.allProducts.length) return 0;
      return Math.max(...state.allProducts.map(p => parseFloat(p.price_discounted || '0')));
    },

    maxPriceFiltered: (state) => {
      if (!state.filteredProducts.length) return 0;
      return Math.max(...state.filteredProducts.map(p => parseFloat(p.price_discounted || '0')));
    },

    

    suggestions(state) {
      return [...new Set(state.allProducts.map(p => p.product_name))].slice(0, 100);
    },

    totalPages(state) {
      return Math.ceil(state.filteredProducts.length / state.perPage) || 1;
    },

    paginatedProducts(state) {
      const start = (state.currentPage - 1) * state.perPage;
      return state.filteredProducts.slice(start, start + state.perPage);
    },
  },

  actions: {
    async fetchFeed(url = '/feeds.json') {
      if (this.feedLoaded) return;  // Only fetch once per session

      try {
        const res = await fetch(url);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);

        const data = await res.json();
        const rawProducts = data.products?.product || [];

        const uniqueMap = new Map();
        for (const p of rawProducts) {
          if (!p.product_code || uniqueMap.has(p.product_code)) continue;

          const vat = parseFloat(p.price_vat || 0);
          const discounted = parseFloat(p.price_discounted || 0);
          const discount = vat > 0 && discounted > 0 && discounted < vat
            ? Math.round(((vat - discounted) / vat) * 100)
            : 0;

          uniqueMap.set(p.product_code, { ...p, _discount: discount });
        }

        this.allProducts = Array.from(uniqueMap.values())
          .sort((a, b) => parseFloat(a.price_discounted || 0) - parseFloat(b.price_discounted || 0));

        this.feedLoaded = true;
      } catch (err) {
        console.error('Failed to load feed:', err);
      }
    },

    setPriceChangedManually(value) {
      this.priceChangedManually = value;
    },

    resetPriceChangedManually() {
      this.priceChangedManually = false;
    },

    async applyFilters() {
      if (!this.feedLoaded) {
        await this.fetchFeed();
      }

      const searchLower = (this.searchQuery || '').trim().toLowerCase();
      const categoryLower = (this.selectedCategory || '').toLowerCase();
      const discountMin = parseInt(this.discountPercent) || 0;

      this.filteredProducts = this.allProducts.filter(p => {
        const name = (p.product_name || '').toLowerCase();
        const category = (p.category || '').toLowerCase();
        const discount = parseFloat(p._discount) || 0;

        // Skip if discount doesn't match
        if (discountMin > 0 && discount < discountMin) return false;

        // Skip if category filter is set and doesn't match
        if (categoryLower && !category.includes(categoryLower)) return false;

        // Skip search unless it's at least 3 characters
        if (searchLower.length >= 3 && !name.includes(searchLower)) return false;

        // Price filter
        if (this.maxPrice != null && p.price_discounted > this.maxPrice) return false;

        // Free shipping
        if (this.freeShipping && p.free_shipping !== '1') return false;

        // Gift included
        if (this.giftIncluded && p.gift_included !== '1') return false;

        return true;
      });

      // Sort
      switch (this.sortOrder) {
        case 'price_asc':
          this.filteredProducts.sort((a, b) => a.price_discounted - b.price_discounted);
          break;
        case 'price_desc':
          this.filteredProducts.sort((a, b) => b.price_discounted - a.price_discounted);
          break;
        case 'discount_asc':
          this.filteredProducts.sort((a, b) => (a._discount || 0) - (b._discount || 0));
          break;
        case 'discount_desc':
          this.filteredProducts.sort((a, b) => (b._discount || 0) - (a._discount || 0));
          break;
      }

      this.currentPage = 1;

      if (!this.priceChangedManually) {
        this.maxPrice = this.maxPriceFiltered;
      }
    }
,

    onFilterChange() {
      this.resetPriceChangedManually();
      this.applyFilters();
    },

    nextPage() {
      if (this.currentPage < this.totalPages) this.currentPage++;
    },
    prevPage() {
      if (this.currentPage > 1) this.currentPage--;
    },
  },
});
