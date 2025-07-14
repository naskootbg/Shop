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
    const name = (p.product_name || '').toLowerCase();
    const category = (p.category || '').toLowerCase();

    const matchesSearch = !searchQuery || name.includes(searchQuery.toLowerCase());
    const matchesCategory = !selectedCategory || category.includes(selectedCategory.toLowerCase());

    const vat = parseFloat(p.price_vat || 0);
    const discounted = parseFloat(p.price_discounted || 0);
    const discount = vat > 0 && discounted > 0 && discounted < vat
      ? Math.round(((vat - discounted) / vat) * 100)
      : 0;

    const matchesDiscount = discount >= discountPercent;
    const matchesMinPrice = !minPrice || discounted >= minPrice;
    const matchesMaxPrice = !maxPrice || discounted <= maxPrice;
    const matchesShipping = !freeShipping || p.free_shipping === '1';
    const matchesGift = !giftIncluded || p.gift_included === '1';

    return matchesSearch &&
      matchesCategory &&
      matchesDiscount &&
      matchesMinPrice &&
      matchesMaxPrice &&
      matchesShipping &&
      matchesGift;
  });

  postMessage(result);
};
