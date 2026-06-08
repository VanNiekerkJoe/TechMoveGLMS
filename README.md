# 🚚 TechMove Global Logistics Management System (GLMS)

[![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/SQL%20Server-Express-blue)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
[![Testing](https://img.shields.io/badge/Testing-xUnit-green)](https://xunit.net/)
[![API](https://img.shields.io/badge/API-ExchangeRate-orange)](https://www.exchangerate-api.com/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue)](https://www.docker.com/)

> A comprehensive web-based logistics management system for contract management, service requests, and real-time currency conversion.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running with Docker](#running-with-docker)
- [Running Locally](#running-locally)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Database Schema](#database-schema)
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
- ✅ **Contract Management** – Create, edit, delete contracts with PDF upload
- ✅ **Service Requests** – Submit requests with live USD → ZAR conversion
- ✅ **Business Rule** – Cannot create requests on Expired or On-Hold contracts
- ✅ **Search & Filter** – LINQ-powered filtering by date range and status

### Technical Features
- ✅ **SQL Server Database** – Entity Framework Core with migrations
- ✅ **Currency API Integration** – Live exchange rates from ExchangeRate-API
- ✅ **JWT Authentication** – Secure API access with JSON Web Tokens
- ✅ **Interactive Dashboard** – Real-time statistics and charts using Chart.js
- ✅ **Unit Testing** – xUnit tests for business logic and validation
- ✅ **Integration Testing** – API endpoint tests for CI/CD pipelines
- ✅ **Modern UI** – Responsive design with Bootstrap 5 and custom styling
- ✅ **Docker Containerization** – Full containerized deployment with Docker Compose

---

## 🛠 Technologies Used

| Category | Technology |
|----------|------------|
| **Framework** | ASP.NET Core MVC (.NET 8) |
| **API** | ASP.NET Core Web API (.NET 8) |
| **Database** | SQL Server 2022 Express |
| **ORM** | Entity Framework Core |
| **Frontend** | Bootstrap 5, Chart.js, Font Awesome |
| **Testing** | xUnit, Moq |
| **API Integration** | ExchangeRate-API |
| **Containerization** | Docker, Docker Compose |
| **Authentication** | JWT Bearer Tokens |
| **Version Control** | Git & GitHub |

---

## 🏗 Architecture

The system follows a **Service-Oriented Architecture (SOA)** with three main components:
