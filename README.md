# 🚚 TechMove Global Logistics Management System (GLMS)

[![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/SQL%20Server-Express-blue)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
[![Testing](https://img.shields.io/badge/Testing-xUnit-green)](https://xunit.net/)
[![API](https://img.shields.io/badge/API-ExchangeRate-orange)](https://www.exchangerate-api.com/)

> A comprehensive web-based logistics management system for contract management, service requests, and real-time currency conversion.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Running the Application](#running-the-application)
- [Running Unit Tests](#running-unit-tests)
- [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
- [Screenshots](#screenshots)
- [Video Demonstration](#video-demonstration)
- [Author](#author)

---

## 📖 Overview

TechMove Logistics previously relied on spreadsheets, emails, and phone calls to manage freight contracts, driver schedules, and invoicing. This fragmented approach led to missing documents, expired contracts going unnoticed, and operational delays.

**TechMove GLMS** solves these challenges by providing a centralized, web-based platform that brings all major operations into one place.

---

## ✨ Features

### Core Functionality
- ✅ **Client Management** – Full CRUD operations for clients
- ✅ **Contract Management** – Create, edit, delete contracts with PDF upload/download
- ✅ **Service Requests** – Submit requests with live USD → ZAR conversion
- ✅ **Business Rule** – Cannot create requests on Expired or On-Hold contracts
- ✅ **Search & Filter** – LINQ-powered filtering by date range and status

### Technical Features
- ✅ **SQL Server Database** – Entity Framework Core with migrations
- ✅ **Currency API Integration** – Live exchange rates from ExchangeRate-API
- ✅ **File Handling** – PDF upload, validation, and download
- ✅ **Interactive Dashboard** – Real-time statistics and charts using Chart.js
- ✅ **Unit Testing** – xUnit tests for business logic and validation
- ✅ **Modern UI** – Responsive design with Bootstrap 5 and custom styling

---

## 🛠 Technologies Used

| Category | Technology |
|----------|------------|
| **Framework** | ASP.NET Core MVC (.NET 8) |
| **Database** | SQL Server Express / LocalDB |
| **ORM** | Entity Framework Core |
| **Frontend** | Bootstrap 5, Chart.js, Font Awesome |
| **Testing** | xUnit, Moq |
| **API** | ExchangeRate-API (USD → ZAR) |
| **Version Control** | Git & GitHub |

---

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

| Software | Version | Download Link |
|----------|---------|---------------|
| Visual Studio | 2022 | [Download](https://visualstudio.microsoft.com/) |
| .NET SDK | 8.0 | [Download](https://dotnet.microsoft.com/) |
| SQL Server | Express / LocalDB | [Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) |
| SQL Server Management Studio (SSMS) | Latest | [Download](https://docs.microsoft.com/en-us/sql/ssms/) |

---

## 🚀 Installation

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/TechMoveGLMS.git
cd TechMoveGLMS
