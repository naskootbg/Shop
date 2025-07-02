self.onmessage = (e) => {
  const { products, filters } = e.data;

  const {
    searchQuery,
    selectedCategory,
    discountPercent,
    minPrice,
    maxPrice,
    freeShipping,
    giftIncluded
  } = filters;

  const result = products.filter(p => {
    const name = p.product_name.toLowerCase();
    const matchesSearch = !searchQuery || name.includes(searchQuery.toLowerCase());
    const matchesCategory = !selectedCategory || p.category === selectedCategory;
    const discount = Math.round(((p.price_vat - p.price_discounted) / p.price_vat) * 100);
    const matchesDiscount = discount >= discountPercent;
    const matchesMinPrice = !minPrice || p.price_discounted >= minPrice;
    const matchesMaxPrice = !maxPrice || p.price_discounted <= maxPrice;
    const matchesShipping = !freeShipping || p.free_shipping === '1';
    const matchesGift = !giftIncluded || p.gift_included === '1';

    return matchesSearch && matchesCategory && matchesDiscount &&
      matchesMinPrice && matchesMaxPrice &&
      matchesShipping && matchesGift;
  });

  postMessage(result);
};
