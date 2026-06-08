# 🚚 TechMove Global Logistics Management System (GLMS)

[![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/SQL%20Server-Express-blue)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
[![Testing](https://img.shields.io/badge/Testing-xUnit-green)](https://xunit.net/)
[![API](https://img.shields.io/badge/API-ExchangeRate-orange)](https://www.exchangerate-api.com/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue)](https://www.docker.com/)

> A comprehensive web-based logistics management system for contract management, service requests, and real-time currency conversion.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Architecture](#-architecture)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Prerequisites](#-prerequisites)
- [Quick Start (Docker)](#-quick-start-docker)
- [Running Locally](#-running-locally-without-docker)
- [API Endpoints](#-api-endpoints)
- [Testing](#-testing)
- [Database Schema](#-database-schema)
- [Project Structure](#-project-structure)
- [Author](#-author)

---

## 📖 Overview

TechMove Logistics previously relied on spreadsheets, emails, and phone calls to manage freight contracts, driver schedules, and invoicing. This fragmented approach led to missing documents, expired contracts going unnoticed, and operational delays.

**TechMove GLMS** solves these challenges by providing a centralised, web-based platform that brings all major operations into one place.

---

## 🏗 Architecture

The system follows a Service-Oriented Architecture (SOA) with three main components:

```
MVC Frontend (Port 5000)  →  Web API (Port 5001)  →  SQL Server Database (Port 1433)
  glms-frontend-web              glms-backend-api            sql-server-db
```

| Container | Purpose | Port |
|-----------|---------|------|
| `glms-frontend-web` | MVC Frontend | 5000 |
| `glms-backend-api` | Web API | 5001 |
| `sql-server-db` | SQL Server Database | 1433 |

---

## ✨ Features

### Core Functionality
- ✅ **Client Management** — Full CRUD operations for clients
- ✅ **Contract Management** — Create, edit, delete contracts with PDF upload
- ✅ **Service Requests** — Submit requests with live USD → ZAR conversion
- ✅ **Business Rules** — Cannot create requests on Expired or On-Hold contracts
- ✅ **Search & Filter** — LINQ-powered filtering by date range and status

### Technical Features
- ✅ **SQL Server Database** — Entity Framework Core with migrations
- ✅ **Currency API Integration** — Live exchange rates from ExchangeRate-API
- ✅ **JWT Authentication** — Secure API access with JSON Web Tokens
- ✅ **Interactive Dashboard** — Real-time statistics and charts using Chart.js
- ✅ **Unit Testing** — xUnit tests for business logic and validation
- ✅ **Integration Testing** — API endpoint tests for CI/CD pipelines
- ✅ **Modern UI** — Responsive design with Bootstrap 5 and custom styling
- ✅ **Docker Containerisation** — Full containerised deployment with Docker Compose

---

## 🛠 Tech Stack

| Category | Technology |
|----------|------------|
| Framework | ASP.NET Core MVC (.NET 8) |
| API | ASP.NET Core Web API (.NET 8) |
| Database | SQL Server 2022 Express |
| ORM | Entity Framework Core |
| Frontend | Bootstrap 5, Chart.js, Font Awesome |
| Testing | xUnit, Moq |
| API Integration | ExchangeRate-API |
| Containerisation | Docker, Docker Compose |
| Authentication | JWT Bearer Tokens |

---

## 📋 Prerequisites

| Software | Version |
|----------|---------|
| Docker Desktop | Latest |
| .NET SDK | 8.0 |
| Visual Studio 2022 | Latest |
| Git | Latest |

---

## 🚀 Quick Start (Docker)

### 1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/TechMoveGLMS.git
cd TechMoveGLMS
```

### 2. Add your ExchangeRate-API key

Sign up at [exchangerate-api.com](https://www.exchangerate-api.com/) and update both `appsettings.json` files:

```json
{
  "ExchangeRateApi": {
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
```

### 3. Start all containers

```bash
docker-compose up -d
```

### 4. Access the application

| Application | URL |
|-------------|-----|
| MVC Frontend | http://localhost:5000 |
| API Swagger | http://localhost:5001/swagger |

### Default login credentials

| Username | Password | Role |
|----------|----------|------|
| admin | admin123 | Administrator |
| manager | manager123 | Manager |
| user | user123 | User |

### Useful Docker commands

```bash
# Check running containers
docker ps

# Stop all containers
docker-compose down

# Rebuild from scratch
docker-compose down -v
docker-compose up -d --build
```

---

## 💻 Running Locally (Without Docker)

### Step 1: Update connection strings

`TechMoveGLMS/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER\\SQLEXPRESS;Database=TechMoveGLMS;Trusted_Connection=True;TrustServerCertificate=true;"
  },
  "ApiSettings": {
    "BaseUrl": "http://localhost:5001"
  }
}
```

`TechMoveGLMS.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER\\SQLEXPRESS;Database=TechMoveGLMS;Trusted_Connection=True;TrustServerCertificate=true;"
  }
}
```

### Step 2: Apply migrations

```bash
cd TechMoveGLMS
dotnet ef database update
```

### Step 3: Run the API

```bash
cd TechMoveGLMS.API
dotnet run
```

### Step 4: Run the MVC app

```bash
cd TechMoveGLMS
dotnet run
```

---

## 🔌 API Endpoints

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/login` | Login and get JWT token |
| `POST` | `/api/auth/register` | Register new user |

### Clients

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Clients` | Get all clients |
| `GET` | `/api/Clients/{id}` | Get client by ID |
| `POST` | `/api/Clients` | Create new client |
| `PUT` | `/api/Clients/{id}` | Update client |
| `DELETE` | `/api/Clients/{id}` | Delete client |

### Contracts

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Contracts` | Get all contracts (with filtering) |
| `GET` | `/api/Contracts/{id}` | Get contract by ID |
| `POST` | `/api/Contracts` | Create new contract |
| `PATCH` | `/api/Contracts/{id}/status` | Update contract status |
| `DELETE` | `/api/Contracts/{id}` | Delete contract |

### Service Requests

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/ServiceRequests` | Get all service requests |
| `GET` | `/api/ServiceRequests/{id}` | Get request by ID |
| `POST` | `/api/ServiceRequests` | Create service request |
| `PATCH` | `/api/ServiceRequests/{id}/status` | Update request status |
| `DELETE` | `/api/ServiceRequests/{id}` | Delete request |

### Dashboard

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/dashboard/stats` | Get dashboard statistics |
| `GET` | `/api/dashboard/recent-requests` | Get recent requests |

### Filtering example

```
GET /api/Contracts?startDate=2025-01-01&endDate=2025-12-31&status=1
```

---

## 🧪 Testing

### Run all tests

```bash
dotnet test
```

### In Visual Studio

1. Open **Test Explorer** (Test → Test Explorer)
2. Click **Run All Tests**
3. All tests should pass ✅

### Test categories

| Test file | Covers |
|-----------|--------|
| `CurrencyCalculatorTests.cs` | USD → ZAR conversion math |
| `FileValidationTests.cs` | PDF file validation |
| `ServiceRequestRuleTests.cs` | Business rules enforcement |
| `ApiIntegrationTests.cs` | API endpoint integration |

---

## 🗄 Database Schema

### Tables

| Table | Description |
|-------|-------------|
| `Clients` | Client information (Name, ContactDetails, Region) |
| `Contracts` | Contract details linked to Clients |
| `ServiceRequests` | Service requests linked to Contracts |
| `__EFMigrationsHistory` | EF Core migration tracking |

### Relationships

```
Clients (1) ──── (many) Contracts (1) ──── (many) ServiceRequests
```

### Sample data

```sql
INSERT INTO Clients (Name, ContactDetails, Region)
VALUES ('TechMove Logistics', 'info@techmove.com', 'Gauteng');

INSERT INTO Contracts (ClientId, StartDate, EndDate, Status, ServiceLevel)
VALUES (1, '2025-01-01', '2025-12-31', 1, 'Premium');

INSERT INTO ServiceRequests (ContractId, Description, CostUSD, CostZAR, Status, CreatedDate)
VALUES (1, 'Urgent shipment', 500.00, 8273.60, 0, GETDATE());
```

---

## 📁 Project Structure

```
TechMoveGLMS/
├── TechMoveGLMS/                 # MVC Frontend Project
│   ├── Controllers/              # MVC Controllers
│   ├── Views/                    # Razor Views
│   ├── Models/                   # Domain Models
│   ├── Services/                 # ApiService, CurrencyService
│   ├── Program.cs                # Entry point
│   ├── appsettings.json          # Configuration
│   └── Dockerfile                # Docker configuration
├── TechMoveGLMS.API/             # Web API Project
│   ├── Controllers/              # API Controllers
│   ├── Models/                   # Domain Models
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Data/                     # DbContext
│   ├── Program.cs                # Entry point
│   ├── appsettings.json          # Configuration
│   └── Dockerfile                # Docker configuration
├── TechMoveGLMS.Tests/           # Test Project
│   ├── UnitTests/                # Unit tests
│   ├── IntegrationTests/         # API integration tests
│   └── TechMoveGLMS.Tests.csproj
├── docker-compose.yml            # Docker Compose configuration
└── README.md
```

---

## 👨‍💻 Author

| | |
|---|---|
| **Name** | Leo van Niekerk |
| **Student Number** | ST10445055 |
| **Module** | PROG7311 |
| **Institution** | The Independent Institute of Education |

---

## 🙏 Acknowledgements

- [ExchangeRate-API](https://www.exchangerate-api.com/) for free currency conversion
- [Bootstrap](https://getbootstrap.com/) for responsive design
- [Chart.js](https://www.chartjs.org/) for dashboard charts
- [Font Awesome](https://fontawesome.com/) for icons
- [Docker](https://www.docker.com/) for containerisation

---

## 📄 License

This project was developed for academic purposes as part of the PROG7311 module at The Independent Institute of Education.

---

*Built with ❤️ for PROG7311*
