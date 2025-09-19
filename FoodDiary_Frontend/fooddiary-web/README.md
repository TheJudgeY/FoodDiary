# 🍽️ FoodDiary Frontend – React Application

A modern React + TypeScript frontend for comprehensive nutrition tracking, built with Vite and featuring a responsive UI that works across desktop, tablet, and mobile devices.

---

## 🚀 Quick Start

### Prerequisites

* **Node.js 18+** – [Download](https://nodejs.org/)
* **npm** or **yarn** package manager
* **FoodDiary API** running at `http://localhost:5001`

### 1) Install Dependencies

```bash
npm install
# or
# yarn
```

### 2) Environment Configuration

```bash
# Copy environment template
cp .env.example .env.local

# Then edit .env.local and update VITE_API_BASE_URL if needed
```

**Environment Variables** (`.env.local`):

```dotenv
VITE_API_BASE_URL=http://localhost:5001
```

### 3) Development Server

```bash
npm run dev
```

Application will be available at: [http://localhost:5173](http://localhost:5173)

---

## 🏗️ Technology Stack

### Core Technologies

* **React 18** – UI library with hooks and functional components
* **TypeScript** – Type‑safe JavaScript development
* **Vite** – Fast build tool and development server
* **React Router** – Client‑side routing

### State Management

* **Zustand** – Lightweight client state
* **React Query** – Server state management and caching

### Styling & UI

* **Tailwind CSS** – Utility‑first CSS framework
* **lucide-react** – Icon library
* **Recharts** – Data visualization and charts
* **react-toastify** – Toast notifications

---

## 📱 Features

### Authentication

* **User Registration** – Create new accounts
* **User Login** – Secure authentication
* **Protected Routes** – Route‑based access control
* **Session Management** – Persistent login state

### Food Tracking

* **Food Entry Management** – Add, edit, delete entries
* **Product Database** – Search and browse products
* **Recipe Integration** – Add recipes to diary
* **Nutrition Calculation** – Real‑time macro & micro tracking

### Analytics & Insights

* **Daily Analysis** – Real‑time goal tracking
* **Trend Analysis** – Weekly and monthly trends
* **Goal Adherence** – Progress with visual indicators
* **Personalized Recommendations** – AI‑driven advice
* **Interactive Charts** – Recharts visualizations

---

## 🔧 Available Scripts

```bash
# Development
npm run dev              # Start development server
npm run build            # Build for production
npm run preview          # Preview production build

# Code Quality
npm run lint             # Run ESLint
npm run lint:fix         # Fix ESLint issues
npm run type-check       # TypeScript type checking

# Mobile Development
npm run build:mobile     # Build web assets for mobile
npm run sync:mobile      # Sync with Capacitor project
```

---

## ⚙️ Configuration

### Environment Variables

Create `.env.local` with the following variables:

```dotenv
# API Configuration
VITE_API_BASE_URL=http://localhost:5001

# Optional: API timeout (default: 10000ms)
VITE_API_TIMEOUT=10000
```

### Mobile Configuration

For mobile development, create `.env.mobile`:

```dotenv
# Mobile API Configuration (Android emulator)
VITE_API_BASE_URL=http://10.0.2.2:5001
```

---

## 📚 Additional Resources

* [React Docs](https://react.dev/learn)
* [TypeScript Handbook](https://www.typescriptlang.org/docs/)
* [Vite Guide](https://vitejs.dev/guide/)
* [Tailwind CSS Docs](https://tailwindcss.com/docs)

---

## 🤝 Contributing

### Code Quality Standards

* ✅ **Zero ESLint errors/warnings**
* ✅ **Zero TypeScript compilation errors**
* ✅ **Component documentation**
* ✅ **Robust error handling**
* ✅ **Performance optimization**

---

## 📄 License

This project is licensed under the MIT License – see the [LICENSE](../../LICENSE) file for details.
