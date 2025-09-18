# 🍽️ FoodDiary Frontend - React Application

A modern React + TypeScript frontend application for comprehensive nutrition tracking, built with Vite and featuring a responsive design that works across desktop, tablet, and mobile devices.

## 🚀 Quick Start

### Prerequisites
- **Node.js 18+** - [Download here](https://nodejs.org/)
- **npm** or **yarn** package manager
- **FoodDiary API** running on http://localhost:5001

### 1. Install Dependencies
\`\`\`bash
npm install
\`\`\`

### 2. Environment Configuration
\`\`\`bash
# Copy environment template
cp .env.example .env.local

# Edit configuration
# Update VITE_API_BASE_URL if needed
\`\`\`

**Environment Variables** (`.env.local`):
\`\`\`
VITE_API_BASE_URL=http://localhost:5001
\`\`\`

### 3. Development Server
\`\`\`bash
npm run dev
\`\`\`
Application will be available at: http://localhost:5173

## 🏗️ Technology Stack

### Core Technologies
- **React 18** - UI library with hooks and functional components
- **TypeScript** - Type-safe JavaScript development
- **Vite** - Fast build tool and development server
- **React Router** - Client-side routing

### State Management
- **Zustand** - Lightweight state management
- **React Query** - Server state management and caching

### Styling & UI
- **Tailwind CSS** - Utility-first CSS framework
- **Lucide React** - Beautiful icon library
- **Recharts** - Data visualization and charts
- **React Toastify** - Toast notifications

## 📱 Features

### Authentication
- **User Registration** - Create new accounts
- **User Login** - Secure authentication
- **Protected Routes** - Route-based access control
- **Session Management** - Persistent login state

### Food Tracking
- **Food Entry Management** - Add, edit, delete food entries
- **Product Database** - Search and browse food products
- **Recipe Integration** - Add recipes to food diary
- **Nutrition Calculation** - Real-time macro and micro tracking

### Analytics & Insights
- **Daily Analysis** - Real-time nutrition goal tracking
- **Trend Analysis** - Weekly and monthly nutrition trends
- **Goal Adherence** - Progress tracking with visual indicators
- **Personalized Recommendations** - AI-driven nutrition advice
- **Interactive Charts** - Data visualization with Recharts

## 🔧 Available Scripts

\`\`\`bash
# Development
npm run dev              # Start development server
npm run build            # Build for production
npm run preview          # Preview production build

# Code Quality
npm run lint             # Run ESLint
npm run lint:fix         # Fix ESLint issues
npm run type-check       # TypeScript type checking

# Mobile Development
npm run build:mobile     # Build for mobile
npm run sync:mobile      # Sync with Capacitor
\`\`\`

## 🔧 Configuration

### Environment Variables
Create `.env.local` file with the following variables:

\`\`\`
# API Configuration
VITE_API_BASE_URL=http://localhost:5001

# Optional: API timeout (default: 10000ms)
VITE_API_TIMEOUT=10000
\`\`\`

### Mobile Configuration
For mobile development, create `.env.mobile`:

\`\`\`
# Mobile API Configuration (Android emulator)
VITE_API_BASE_URL=http://10.0.2.2:5001
\`\`\`

## 📚 Additional Resources

### Documentation
- [React Documentation](https://reactjs.org/docs/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Vite Guide](https://vitejs.dev/guide/)
- [Tailwind CSS Docs](https://tailwindcss.com/docs)

## 🤝 Contributing

### Code Quality Standards
- ✅ **Zero ESLint errors/warnings**
- ✅ **Zero TypeScript compilation errors**
- ✅ **Component documentation**
- ✅ **Error handling**
- ✅ **Performance optimization**

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.

---

**Built with ❤️ using React, TypeScript, and Vite**
