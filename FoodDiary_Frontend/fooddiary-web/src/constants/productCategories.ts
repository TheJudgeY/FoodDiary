export const PRODUCT_CATEGORIES = {
  0: "Fruits",
  1: "Vegetables",
  2: "Grains",
  3: "Proteins",
  4: "Dairy",
  5: "Nuts & Seeds",
  6: "Beverages",
  7: "Snacks",
  8: "Condiments",
  9: "Supplements",
  10: "Other"
} as const;

export const PRODUCT_CATEGORIES_ARRAY = [
  { value: '', label: 'All Categories' },
  { value: 0, label: 'Fruits' },
  { value: 1, label: 'Vegetables' },
  { value: 2, label: 'Grains' },
  { value: 3, label: 'Proteins' },
  { value: 4, label: 'Dairy' },
  { value: 5, label: 'Nuts & Seeds' },
  { value: 6, label: 'Beverages' },
  { value: 7, label: 'Snacks' },
  { value: 8, label: 'Condiments' },
  { value: 9, label: 'Supplements' },
  { value: 10, label: 'Other' }
] as const;

export type ProductCategoryValue = keyof typeof PRODUCT_CATEGORIES;
