# RSM Order Management System

## Project Overview

RSM Order Management System is a full-stack web application designed to manage customer orders, products, shipping information, and order status workflows. The system allows users to create, update, delete, and track orders while applying business rules such as stock validation, discontinued product restrictions, shipping address validation, and controlled order status transitions.

The application also supports exporting reports in PDF and Excel formats and includes unit testing with code coverage analysis to ensure reliability and maintainability.

The project follows a clean architecture approach. The backend was developed using ASP.NET Core Web API with Entity Framework Core and SQL Server, while the frontend was built using Vue.js with the Quasar Framework. The solution also integrates Google Address Validation services to validate shipping addresses and obtain geographical coordinates.

---

# Technologies Used

## Backend
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- xUnit
- Moq

## Frontend
- Vue.js 3
- Quasar Framework
- Vue Router
- Axios
- Chart.js
- Vue-ChartJS
- jsPDF
- jsPDF-AutoTable
- ExcelJS
- XLSX


## Testing & Coverage
- Coverlet
- ReportGenerator

---

# Project Structure

```text
Northwind_Program/
│
├── RSM.Backend
├── RSM.Application
├── RSM.Domain
├── RSM.Infrastructure
├── RSM.Frontend
├── RSM.Tests
└── RSMProject.sln
```

The solution is organized into separate projects to improve maintainability and separation of concerns.

- `RSM.Backend` contains the API controllers and startup configuration.
- `RSM.Application` contains business logic and services.
- `RSM.Domain` stores the domain entities and models.
- `RSM.Infrastructure` handles database access and external integrations.
- `RSM.Frontend` contains the Quasar frontend application.
- `RSM.Tests` includes unit tests and coverage analysis.

---

# Requirements

Before running the project, install the following tools:

- .NET SDK 8 or later
- Node.js 18 or later
- SQL Server
- Visual Studio 2022 or Visual Studio Code
- Quasar CLI

---

# Database Setup

The project uses SQL Server as its database engine.

## Create Database

Open SQL Server Management Studio and run:

```sql
CREATE DATABASE Northwind;
GO
```

## Execute SQL Script

After creating the database, execute the provided SQL script file:

```text
script.sql
```

The script creates all necessary tables, relationships, constraints, and seed data required by the application.

---

# Backend Setup

Navigate to the backend folder:

```powershell
cd RSM.Backend
```

Restore all dependencies:

```powershell
dotnet restore
```

Open the `appsettings.json` file and configure the SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Configure the Google Address Validation API key inside `appsettings.json`:

```json
{
  "GoogleMaps": {
    "ApiKey": "YOUR_API_KEY"
  }
}
```

Run the backend server:

```powershell
dotnet run
```

The API will start locally and Swagger documentation will be available automatically in the browser.

---

# Frontend Setup

Navigate to the frontend directory:

```powershell
cd RSM.Frontend
```

Install project dependencies:

```powershell
npm install
```

If Quasar CLI is not installed globally, install it with:

```powershell
npm install -g @quasar/cli
```

Open the following file:

```text
src/boot/axios.js
```

Verify that the `baseURL` points to the backend API URL.

Run the frontend application:

```powershell
quasar dev
```

The frontend application will run locally and connect to the backend API.

---

# Running Unit Tests

Navigate to the testing project:

```powershell
cd RSM.Tests
```

Run all tests:

```powershell
dotnet test
```

The test project includes validations for:

- Order creation
- Product stock handling
- Order updates
- Status transitions
- Shipping validations
- Business rules

---

# Generating Coverage Reports

Run tests with coverage enabled:

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

Install ReportGenerator globally if necessary:

```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Generate the HTML coverage report:

```powershell
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

Open the generated report:

```text
coveragereport/index.html
```

The report displays:

- Line coverage
- Branch coverage
- Class metrics
- Method metrics

---

# Main Features

- Order creation and management
- Product stock validation
- Discontinued product restrictions
- Google shipping address validation
- Order status workflow validation
- PDF export
- Excel export
- Unit testing and coverage analysis

---

# Order Status Workflow

```text
Pending → Processing → Shipped → Completed
```

Orders can only be cancelled while in:
- Pending
- Processing

---

# Important Business Rules

- Orders must contain at least one product
- A valid shipper is required
- Shipping addresses must be validated
- Products cannot exceed available stock
- Discontinued products cannot be added to new orders
- Invalid order status transitions are blocked
- Only pending orders can be deleted

---

# Coverage Results

Current approximate coverage results:

| Metric | Result |
|---|---|
| Line Coverage | 87% |
| Branch Coverage | 61% |

---

# Author

Developed by Diego Guerrero.
