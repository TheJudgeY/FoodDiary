import { 
  Flame, 
  Beef, 
  Droplets, 
  Wheat,
  BarChart3,
  Target,
  TrendingUp,
  TrendingDown,
  Activity,
  Scale,
  Info,
  Lightbulb,
  Calendar,
  Plus,
  Edit,
  Trash2,
  ChevronLeft,
  ChevronRight,
  Home,
  Package,
  ChefHat,
  User,
  Settings,
  Apple
} from 'lucide-react';


export const NUTRITION_ICONS = {
  calories: {
    icon: Flame,
    color: 'text-red-600',
    bgColor: 'bg-red-100',
    borderColor: 'border-red-200',
    progressColor: 'bg-red-500',
    bgProgressColor: 'bg-red-50'
  },
  protein: {
    icon: Beef,
    color: 'text-green-600',
    bgColor: 'bg-green-100',
    borderColor: 'border-green-200',
    progressColor: 'bg-green-500',
    bgProgressColor: 'bg-green-50'
  },
  fat: {
    icon: Droplets,
    color: 'text-amber-600',
    bgColor: 'bg-amber-100',
    borderColor: 'border-amber-200',
    progressColor: 'bg-amber-500',
    bgProgressColor: 'bg-amber-50'
  },
  carbs: {
    icon: Wheat,
    color: 'text-sky-600',
    bgColor: 'bg-sky-100',
    borderColor: 'border-sky-200',
    progressColor: 'bg-sky-500',
    bgProgressColor: 'bg-sky-50'
  }
} as const;


export const UI_ICONS = {
  barChart: BarChart3,
  target: Target,
  trendingUp: TrendingUp,
  trendingDown: TrendingDown,
  activity: Activity,
  scale: Scale,
  info: Info,
  lightbulb: Lightbulb,
  calendar: Calendar,
  plus: Plus,
  edit: Edit,
  trash: Trash2,
  chevronLeft: ChevronLeft,
  chevronRight: ChevronRight,
  home: Home,
  package: Package,
  chefHat: ChefHat,
  user: User,
  settings: Settings,
  apple: Apple
} as const;


export const getNutritionIcon = (type: keyof typeof NUTRITION_ICONS) => {
  return NUTRITION_ICONS[type];
};


export const getNutritionIconComponent = (type: keyof typeof NUTRITION_ICONS) => {
  return NUTRITION_ICONS[type].icon;
};
