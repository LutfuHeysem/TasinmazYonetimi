# Taşınmaz Yönetimi

A property management backend system built with ASP.NET Core (.NET 8), using Entity Framework Core and PostgreSQL.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Running the Project](#running-the-project)
- [Running the Frontend](#running-the-frontend)

---

## Features

- Property and asset management API
- Built with ASP.NET Core (.NET 8)
- PostgreSQL database integration (via Npgsql)
- Swagger/OpenAPI support for API documentation

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)

## Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/LutfuHeysem/TasinmazYonetimi.git
   cd TasinmazYonetimi
   ```

2. **Navigate to the backend API:**

   ```bash
   cd backend-dotnet/Api
   ```

3. **Restore dependencies:**

   ```bash
   dotnet restore
   ```

## Configuration

1. **App Settings:**

   - Check or update `appsettings.json` and/or `appsettings.Development.json` for your local environment.
   - Ensure your PostgreSQL connection string is correct:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=your_db;Username=your_user;Password=your_password"
   }
   ```

   - You can override settings using environment variables.

## Database Setup

1. **Create the PostgreSQL database:**

   ```bash
   createdb your_db
   ```

2. **Apply Entity Framework migrations:**

   ```bash
   dotnet ef database update
   ```
   > If `dotnet ef` is not installed, run:
   > ```bash
   > dotnet tool install --global dotnet-ef
   > ```

## Running the Project

1. **Run the API server:**

   ```bash
   dotnet run
   ```

## Running the Frontend

1. **Navigate to the Angular frontend directory:**

   ```bash
   cd client-angular
   ```

2. **Install frontend dependencies:**

   ```bash
   npm install
   ```

3. **Start the Angular development server:**

   ```bash
   ng serve
   ```
   or, if you don't have the Angular CLI installed globally:
   ```bash
   npx ng serve
   ```

4. **Access the application in your browser:**

   Open [http://localhost:4200](http://localhost:4200)

> The frontend will communicate with the backend API at `http://localhost:5000` or `https://localhost:5001`, as configured by your environment. Make sure the backend server is running.


