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
  }),

  getters: {
    categories(state) {
      return [...new Set(state.allProducts.map(p => p.category).filter(Boolean))].sort();
    },
    maxAvailablePrice(state) {
      return Math.ceil(
        Math.max(...state.allProducts.map(p => parseFloat(p.price_discounted || 0)))
      );
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

        this.allProducts = Array.from(uniqueMap.values());
        this.feedLoaded = true;
      } catch (err) {
        console.error('Failed to load feed:', err);
      }
    },

    async applyFilters() {
      if (!this.feedLoaded) {
        await this.fetchFeed();
      }

      const searchLower = this.searchQuery.toLowerCase();
      const categoryLower = this.selectedCategory.toLowerCase();
      const discountMin = this.discountPercent || 0;

      // First: filter
      this.filteredProducts = this.allProducts.filter(p => {
        const name = (p.product_name || '').toLowerCase();
        const category = (p.category || '').toLowerCase();
        const discount = p._discount || 0;

        if (discountMin && discount < discountMin) return false;
        if (categoryLower && !category.includes(categoryLower)) return false;
        if (searchLower && !name.includes(searchLower)) return false;
        if (this.maxPrice && p.price_discounted > this.maxPrice) return false;
        if (this.freeShipping && p.free_shipping !== '1') return false;
        if (this.giftIncluded && p.gift_included !== '1') return false;

        return true;
      });

      // Then: sort
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
    }
,

    nextPage() {
      if (this.currentPage < this.totalPages) this.currentPage++;
    },
    prevPage() {
      if (this.currentPage > 1) this.currentPage--;
    },
  },
});
