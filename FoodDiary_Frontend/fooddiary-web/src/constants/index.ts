export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5001';
export const API_TIMEOUT = 10000;
export const API_ENDPOINTS = {
  AUTH: {
    REGISTER: '/api/auth/register',
    LOGIN: '/api/auth/login',
    CONFIRM_EMAIL: '/api/auth/confirm-email',
    RESEND_EMAIL_CONFIRMATION: '/api/auth/resend-email-confirmation',
    EMAIL_CONFIRMATION_TOKEN: '/api/auth/email-confirmation-token',
  },
  USER: {
    ME: '/api/users/me',
    UPDATE_PROFILE: '/api/users/me',
    BODY_METRICS: '/api/users/body-metrics',
    FITNESS_GOAL: '/api/users/fitness-goal',
    MACRONUTRIENT_GOALS: '/api/users/macronutrient-goals',
  },
  FOOD_ENTRIES: {
    LIST: '/api/food-entries',
    CREATE: '/api/food-entries',
    GET: (id: string) => `/api/food-entries/${id}`,
    UPDATE: (id: string) => `/api/food-entries/${id}`,
    DELETE: (id: string) => `/api/food-entries/${id}`,
    ADD_RECIPE: '/api/food-entries/add-recipe',
  },
  PRODUCTS: {
    LIST: '/api/products',
    CREATE: '/api/products',
    GET: (id: string) => `/api/products/${id}`,
    UPDATE: (id: string) => `/api/products/${id}`,
    DELETE: (id: string) => `/api/products/${id}`,
    UPLOAD_IMAGE: (id: string) => `/api/products/${id}/image`,
    GET_IMAGE: (id: string) => `/api/products/${id}/image`,
  },
  RECIPES: {
    LIST: '/api/recipes',
    CREATE: '/api/recipes',
    GET: (id: string) => `/api/recipes/${id}`,
    UPDATE: (id: string) => `/api/recipes/${id}`,
    DELETE: (id: string) => `/api/recipes/${id}`,
    UPLOAD_IMAGE: (id: string) => `/api/recipes/${id}/image`,
    GET_IMAGE: (id: string) => `/api/recipes/${id}/image`,
  },
  ANALYTICS: {
    DAILY: '/api/analytics/daily',
    TRENDS: '/api/analytics/trends',
    RECOMMENDATIONS: '/api/analytics/recommendations',
  },
  NOTIFICATIONS: {
    LIST: '/api/notifications',
    PREFERENCES: '/api/notifications/preferences',
    TEST: '/api/notifications/test',
  },
} as const;
export const VALIDATION_RULES = {
  EMAIL: {
    required: 'Email is required',
    pattern: {
      value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
      message: 'Invalid email address',
    },
  },
  PASSWORD: {
    required: 'Password is required',
    minLength: {
      value: 8,
      message: 'Password must be at least 8 characters',
    },
    pattern: {
      value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/,
      message: 'Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character',
    },
  },
  NAME: {
    required: 'Name is required',
    minLength: {
      value: 2,
      message: 'Name must be at least 2 characters',
    },
    maxLength: {
      value: 50,
      message: 'Name must be less than 50 characters',
    },
  },
  HEIGHT: {
    required: 'Height is required',
    min: {
      value: 100,
      message: 'Height must be at least 100 cm',
    },
    max: {
      value: 250,
      message: 'Height must be less than 250 cm',
    },
  },
  WEIGHT: {
    required: 'Weight is required',
    min: {
      value: 30,
      message: 'Weight must be at least 30 kg',
    },
    max: {
      value: 300,
      message: 'Weight must be less than 300 kg',
    },
  },
  AGE: {
    required: 'Age is required',
    min: {
      value: 13,
      message: 'Age must be at least 13',
    },
    max: {
      value: 120,
      message: 'Age must be less than 120',
    },
  },
  QUANTITY: {
    required: 'Quantity is required',
    min: {
      value: 0.1,
      message: 'Quantity must be at least 0.1',
    },
    max: {
      value: 10000,
      message: 'Quantity must be less than 10000',
    },
  },
  CALORIES: {
    required: 'Calories are required',
    min: {
      value: 0,
      message: 'Calories must be at least 0',
    },
    max: {
      value: 10000,
      message: 'Calories must be less than 10000',
    },
  },
  PROTEINS: {
    required: 'Proteins are required',
    min: {
      value: 0,
      message: 'Proteins must be at least 0',
    },
    max: {
      value: 1000,
      message: 'Proteins must be less than 1000',
    },
  },
  FATS: {
    required: 'Fats are required',
    min: {
      value: 0,
      message: 'Fats must be at least 0',
    },
    max: {
      value: 1000,
      message: 'Fats must be less than 1000',
    },
  },
  CARBOHYDRATES: {
    required: 'Carbohydrates are required',
    min: {
      value: 0,
      message: 'Carbohydrates must be at least 0',
    },
    max: {
      value: 1000,
      message: 'Carbohydrates must be less than 1000',
    },
  },
} as const;
export const UI_CONSTANTS = {
  BREAKPOINTS: {
    MOBILE: 320,
    TABLET: 768,
    DESKTOP: 1024,
    LARGE_DESKTOP: 1280,
  },
  SPACING: {
    XS: '0.5rem', 
    SM: '1rem',   
    MD: '1.5rem', 
    LG: '2rem',   
    XL: '3rem',   
    XXL: '4rem',  
  },
  BORDER_RADIUS: {
    SM: '0.375rem',  
    MD: '0.5rem',    
    LG: '0.75rem',   
    XL: '1rem',      
    FULL: '9999px',
  },
  SHADOWS: {
    SM: '0 1px 2px 0 rgb(0 0 0 / 0.05)',
    MD: '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
    LG: '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
    XL: '0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1)',
  },
  TRANSITIONS: {
    FAST: '150ms ease-in-out',
    NORMAL: '200ms ease-in-out',
    SLOW: '300ms ease-in-out',
  },
  Z_INDEX: {
    DROPDOWN: 1000,
    STICKY: 1020,
    FIXED: 1030,
    MODAL_BACKDROP: 1040,
    MODAL: 1050,
    POPOVER: 1060,
    TOOLTIP: 1070,
  },
} as const;
export const STORAGE_KEYS = {
  AUTH_TOKEN: 'fooddiary_auth_token',
  USER_DATA: 'fooddiary_user_data',
  THEME: 'fooddiary_theme',
  NOTIFICATIONS: 'fooddiary_notifications',
  PREFERENCES: 'fooddiary_preferences',
} as const;
export const DEFAULT_VALUES = {
  USER: {
    HEIGHT_CM: 170,
    WEIGHT_KG: 70,
    AGE: 25,
    DAILY_CALORIE_GOAL: 2000,
    DAILY_PROTEIN_GOAL: 150,
    DAILY_FAT_GOAL: 65,
    DAILY_CARBOHYDRATE_GOAL: 250,
  },
  FOOD_ENTRY: {
    QUANTITY: 100,
  },
  RECIPE: {
    SERVINGS: 4,
    PREPARATION_TIME: 15,
    COOKING_TIME: 30,
  },
  PAGINATION: {
    PAGE_SIZE: 20,
    MAX_PAGE_SIZE: 100,
  },
} as const;
export const ERROR_MESSAGES = {
  NETWORK_ERROR: 'Network error. Please check your connection and try again.',
  UNAUTHORIZED: 'You are not authorized to perform this action.',
  FORBIDDEN: 'Access denied. You don\'t have permission to perform this action.',
  NOT_FOUND: 'The requested resource was not found.',
  SERVER_ERROR: 'An unexpected error occurred. Please try again later.',
  VALIDATION_ERROR: 'Please check your input and try again.',
  TOKEN_EXPIRED: 'Your session has expired. Please log in again.',
  EMAIL_NOT_CONFIRMED: 'Please confirm your email address before logging in.',
  INVALID_CREDENTIALS: 'Invalid email or password.',
  EMAIL_ALREADY_EXISTS: 'An account with this email already exists.',
} as const;
export const SUCCESS_MESSAGES = {
  LOGIN: 'Successfully logged in!',
  REGISTER: 'Registration successful! Please check your email to confirm your account.',
  LOGOUT: 'Successfully logged out!',
  PROFILE_UPDATED: 'Profile updated successfully!',
  FOOD_ENTRY_CREATED: 'Food entry added successfully!',
  FOOD_ENTRY_UPDATED: 'Food entry updated successfully!',
  FOOD_ENTRY_DELETED: 'Food entry deleted successfully!',
  PRODUCT_CREATED: 'Product created successfully!',
  PRODUCT_UPDATED: 'Product updated successfully!',
  PRODUCT_DELETED: 'Product deleted successfully!',
  RECIPE_CREATED: 'Recipe created successfully!',
  RECIPE_UPDATED: 'Recipe updated successfully!',
  RECIPE_DELETED: 'Recipe deleted successfully!',
  EMAIL_CONFIRMED: 'Email confirmed successfully!',
  PASSWORD_CHANGED: 'Password changed successfully!',
} as const;
export const CHART_COLORS = {
  PRIMARY: '#0ea5e9',
  SECONDARY: '#d946ef',
  SUCCESS: '#22c55e',
  WARNING: '#f59e0b',
  ERROR: '#ef4444',
  SURFACE: '#737373',
  CALORIES: '#ef4444',
  PROTEINS: '#22c55e',
  FATS: '#f59e0b',
  CARBOHYDRATES: '#0ea5e9',
} as const;
export const MEAL_TIMES = {
  BREAKFAST: '08:00',
  LUNCH: '12:00',
  DINNER: '18:00',
  SNACK: '15:00',
} as const;
export const MEAL_TYPES = {
  0: "Breakfast",
  1: "Lunch",
  2: "Dinner",
  3: "Snack"
} as const;
export const MEAL_TYPES_ARRAY = [
  { value: 0, label: 'Breakfast', displayName: 'Breakfast' },
  { value: 1, label: 'Lunch', displayName: 'Lunch' },
  { value: 2, label: 'Dinner', displayName: 'Dinner' },
  { value: 3, label: 'Snack', displayName: 'Snack' }
] as const;
