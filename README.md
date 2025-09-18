# 🍽️ FoodDiary - Complete Nutrition Tracking Application

A full-stack nutrition tracking application built with Clean Architecture principles, featuring comprehensive food logging, analytics, and mobile support.

## 🏗️ Architecture

### Backend (ASP.NET Core 9.0)
- **Clean Architecture**: Domain, Infrastructure, UseCases, and Web layers
- **CQRS Pattern**: MediatR for command/query separation
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **API Documentation**: Swagger/OpenAPI
- **Logging**: Serilog with structured logging

### Frontend (React + TypeScript)
- **React 18** with TypeScript for type safety
- **Vite** for fast development and building
- **Zustand** for state management
- **React Router** for client-side routing
- **Tailwind CSS** for responsive styling
- **Recharts** for data visualization
- **Date-fns** for date manipulation

### Mobile (Capacitor)
- **Capacitor** for native mobile app generation
- **Android** support with configured build environment
- **Responsive design** that works across all devices

## 🚀 Features

### Core Functionality
- ✅ **User Authentication** - Secure registration and login
- ✅ **Food Entry Management** - Add, edit, delete food entries
- ✅ **Product Database** - Comprehensive food product catalog
- ✅ **Recipe Management** - Create and manage custom recipes
- ✅ **Nutrition Tracking** - Track calories, macros, and micronutrients

### Advanced Analytics
- ✅ **Daily Analysis** - Real-time nutrition goal tracking
- ✅ **Trend Analysis** - Weekly and monthly nutrition trends
- ✅ **Goal Adherence** - Track progress toward nutrition goals
- ✅ **Personalized Recommendations** - AI-driven nutrition advice
- ✅ **Visual Charts** - Interactive nutrition data visualization

### User Experience
- ✅ **Responsive Design** - Works on desktop, tablet, and mobile
- ✅ **Dark/Light Themes** - User preference support
- ✅ **Real-time Updates** - Instant feedback and calculations
- ✅ **Clean Interface** - Intuitive and user-friendly design

## 📋 Prerequisites

### Required Software
- **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download)
- **Node.js 18+** - [Download here](https://nodejs.org/)
- **PostgreSQL 13+** - [Download here](https://www.postgresql.org/download/)
- **Git** - [Download here](https://git-scm.com/)

### Optional Software
- **Android Studio** - For mobile development
- **Docker** - For containerized deployment
- **Visual Studio Code** - Recommended editor

## 🛠️ Quick Setup

### 1. Clone Repository
\`\`\`bash
git clone <repository-url>
cd FoodDiary
\`\`\`

### 2. Database Setup
\`\`\`bash
// Install PostgreSQL (macOS with Homebrew)
brew install postgresql
brew services start postgresql

// Create database and user
psql postgres
CREATE DATABASE your_database_name;
CREATE USER your_username WITH PASSWORD 'your_secure_password';
GRANT ALL PRIVILEGES ON DATABASE your_database_name TO your_username;
\\q
\`\`\`

### 3. Backend Configuration
\`\`\`bash
cd FoodDiary_API/src/FoodDiary.Web

// Copy configuration templates
cp appsettings.template.json appsettings.json
cp appsettings.Development.template.json appsettings.Development.json

// Edit configuration files with your settings
// Update connection strings, JWT secrets, email settings
\`\`\`

**⚠️ IMPORTANT**: Update the following placeholders in your configuration files:

#### appsettings.json Configuration
\`\`\`json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=YOUR_DATABASE_NAME;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;"
  },
  "JwtSettings": {
    "Secret": "YOUR_SUPER_SECRET_JWT_KEY_MINIMUM_32_CHARACTERS_LONG",
    "Issuer": "FoodDiaryApi",
    "Audience": "FoodDiaryApiUsers",
    "ExpiresInMinutes": 60
  },
  "Mailserver": {
    "Hostname": "YOUR_SMTP_SERVER",
    "Port": 587,
    "Username": "YOUR_EMAIL_USERNAME",
    "Password": "YOUR_EMAIL_PASSWORD",
    "EnableSsl": true
  },
  "FrontendUrl": "http://localhost:5173"
}
\`\`\`

### 4. Frontend Configuration
\`\`\`bash
cd FoodDiary_Frontend/fooddiary-web

// Copy environment template
cp .env.example .env.local

// Install dependencies
npm install
\`\`\`

**Environment Variables** (update in `.env.local`):
\`\`\`
VITE_API_BASE_URL=http://localhost:5001
\`\`\`

### 5. Database Migration
\`\`\`bash
cd FoodDiary_API/src/FoodDiary.Web
dotnet ef database update
\`\`\`

## 🏃‍♂️ Running the Application

### Start Backend API
\`\`\`bash
cd FoodDiary_API/src/FoodDiary.Web
dotnet run --environment Development
\`\`\`
API will be available at: http://localhost:5001

### Start Frontend
\`\`\`bash
cd FoodDiary_Frontend/fooddiary-web
npm run dev
\`\`\`
Frontend will be available at: http://localhost:5173

### Mobile Development
\`\`\`bash
cd FoodDiary_Frontend/fooddiary-mobile

// Build web assets for mobile
npm run build

// Sync with mobile project
npm run sync

// Open in Android Studio
npm run open:android
\`\`\`

**Mobile API Configuration**:
For mobile testing, run the API with mobile profile:
\`\`\`bash
cd FoodDiary_API/src/FoodDiary.Web
dotnet run --launch-profile http-mobile --environment Development
\`\`\`

## 📱 Mobile Configuration

### Android Setup
1. **Copy mobile environment**: `cp .env.mobile.example .env.mobile`
2. **Configure API URL**: Update for your network (emulator uses `http://10.0.2.2:5001`)
3. **Start API with mobile profile**: `dotnet run --launch-profile http-mobile --environment Development`

## 🧪 Testing

### Backend Tests
\`\`\`bash
cd FoodDiary_API
dotnet test
\`\`\`

### Frontend Linting
\`\`\`bash
cd FoodDiary_Frontend/fooddiary-web
npm run lint
\`\`\`

## 📊 API Documentation

When running in development mode, API documentation is available at:
- **Swagger UI**: http://localhost:5001/swagger

## 🔧 Configuration Files

### Backend Configuration Templates
- **appsettings.template.json**: Production configuration template
- **appsettings.Development.template.json**: Development configuration template

### Frontend Configuration Templates
- **.env.example**: Local development environment variables
- **.env.mobile.example**: Mobile development environment variables

## 🔒 Security Configuration

### Required Placeholders to Replace
Before running the application, you **MUST** replace these placeholders:

#### Database Configuration
- `YOUR_DATABASE_NAME` - Your PostgreSQL database name
- `YOUR_USERNAME` - Your PostgreSQL username  
- `YOUR_PASSWORD` - Your PostgreSQL password

#### JWT Security
- `YOUR_SUPER_SECRET_JWT_KEY_MINIMUM_32_CHARACTERS_LONG` - Strong JWT secret (32+ chars)

#### Email Configuration
- `YOUR_SMTP_SERVER` - Your SMTP server hostname
- `YOUR_EMAIL_USERNAME` - Your email username
- `YOUR_EMAIL_PASSWORD` - Your email password

#### Frontend Configuration
- `VITE_API_BASE_URL` - API base URL (default: http://localhost:5001)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit changes: `git commit -m 'Add amazing feature'`
4. Push to branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

### Code Quality Standards
- ✅ **Zero ESLint errors/warnings**
- ✅ **Zero TypeScript compilation errors**
- ✅ **All tests passing**
- ✅ **Clean Architecture principles**
- ✅ **Proper error handling**

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🚀 Deployment

### Production Considerations
1. **Environment Variables**: Secure all sensitive configuration
2. **Database**: Use production PostgreSQL with proper backup
3. **HTTPS**: Enable SSL/TLS in production
4. **Monitoring**: Implement logging and monitoring
5. **Security**: Review and update security settings

### Docker Deployment
\`\`\`bash
// Build and run with Docker Compose
docker-compose up --build
\`\`\`

## 🔧 Troubleshooting

### Common Issues

#### Database Connection Errors
- **Check PostgreSQL is running**: `pg_isready`
- **Verify credentials**: Test connection with `psql`
- **Check connection string**: Ensure correct host, port, database name

#### Frontend API Connection Issues
- **CORS errors**: Verify `FrontendUrl` in appsettings.json
- **Network errors**: Check API is running on correct port
- **Environment variables**: Verify `.env.local` configuration

#### Mobile App Issues
- **API not accessible**: Use `http://10.0.2.2:5001` for Android emulator
- **Build failures**: Ensure web app builds successfully first
- **Sync issues**: Try `npx cap clean` then `npx cap sync`

#### Build/Compilation Errors
- **Backend**: Check .NET SDK version with `dotnet --version`
- **Frontend**: Check Node.js version with `node --version`
- **Dependencies**: Run `dotnet restore` and `npm install`

### Logs and Debugging
- **Backend logs**: Check console output and `log.txt` file
- **Frontend logs**: Check browser developer console
- **Database logs**: Check PostgreSQL logs for connection issues

## 📚 Additional Resources

### Documentation
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [React Documentation](https://reactjs.org/docs/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Capacitor Documentation](https://capacitorjs.com/docs)

### Development Tools
- **Database Management**: pgAdmin, DBeaver
- **API Testing**: Postman, Insomnia
- **Code Editor**: Visual Studio Code with extensions

## ✅ Setup Verification Checklist

### Backend ✅
- [ ] Database created and accessible
- [ ] Configuration files created and configured
- [ ] Dependencies installed (`dotnet restore`)
- [ ] Migrations applied (`dotnet ef database update`)
- [ ] API running on http://localhost:5001
- [ ] Swagger accessible at http://localhost:5001/swagger

### Frontend ✅
- [ ] Environment file configured
- [ ] Dependencies installed (`npm install`)
- [ ] Application running on http://localhost:5173
- [ ] Can register new user
- [ ] Can login with credentials
- [ ] API communication working

### Mobile (Optional) ✅
- [ ] Android Studio installed
- [ ] Mobile environment configured
- [ ] Capacitor project synced
- [ ] Can build and run on emulator/device

---

**🎉 Setup Complete!** You should now have a fully functional FoodDiary application.
