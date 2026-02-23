# 🚀 ATS -- Devotion Ventures

A production-ready, multi-tenant Applicant Tracking System (ATS) built
with ASP.NET Core (.NET 10), SQL Server / Azure SQL, and deployed to
Microsoft Azure using full CI/CD pipelines.

------------------------------------------------------------------------

## 📌 Project Overview

ATS -- Devotion Ventures is a SaaS-style recruitment management system
that allows:

-   Admins to manage companies and users
-   Companies (Customers) to manage jobs and candidates
-   Tracking applications through hiring stages
-   Viewing candidates in Kanban board format
-   Secure multi-tenant data isolation

The system is fully deployed to Azure and automated using CI/CD
pipelines.

------------------------------------------------------------------------

# 🏗 Architecture & Technology Stack

## 🔹 Backend

-   ASP.NET Core (.NET 10)
-   Entity Framework Core
-   ASP.NET Identity (Role-based authentication)
-   SQL Server / Azure SQL Database

## 🔹 Frontend

-   Razor Views
-   Bootstrap 5
-   Bootstrap Icons
-   Responsive custom layout
-   Modern dashboard UI

## 🔹 Cloud & DevOps

-   Azure App Service
-   Azure SQL Database
-   Azure DevOps (CI/CD Pipelines)
-   GitHub (Source Control)

------------------------------------------------------------------------

# 👥 Roles & Authorization

## 👑 Admin

-   Full system access
-   Create & manage Companies (Accounts)
-   Create Customer users (1 per company)
-   Create additional Admin users
-   View all jobs & candidates
-   Switch company context
-   Access system dashboards

## 🏢 Customer (Company User)

-   Linked to one company
-   Create & manage Jobs
-   Add Candidates
-   Track Applications
-   View Kanban board
-   Access company dashboard

Authorization is enforced using role-based attributes.

------------------------------------------------------------------------

# 🧱 Database Structure

## Identity Tables

-   AspNetUsers
-   AspNetRoles
-   AspNetUserRoles

## Custom Tables

-   Accounts (Companies)
-   Jobs
-   Candidates
-   Applications

### Application Stages

-   Applied
-   Interview
-   Hired
-   Rejected

Each company's data is isolated using `AccountId` (Multi-tenant ready).

------------------------------------------------------------------------

# 📊 Core Features

### ✅ Company Management

Admin can create and manage companies.

### ✅ User Management

-   Create Customer users
-   Create Admin users
-   Edit/Delete customer users
-   Role-based access

### ✅ Jobs Management

-   Create jobs per company
-   View jobs in modern UI

### ✅ Candidate Management

-   Add candidates with:
    -   Full name
    -   Email
    -   Phone
    -   LinkedIn profile
-   View per job applications

### ✅ Applications Tracking

Each candidate can: - Apply for multiple jobs - Move across stages

### ✅ Kanban Board

Visual tracking of candidates by stage.

### ✅ Modern Dashboards

-   KPI cards
-   Color-coded stage counters
-   Responsive design

------------------------------------------------------------------------

# 🔐 Security

-   Role-based authorization
-   Identity authentication
-   Admin-only access to system management
-   Multi-tenant data filtering
-   Production connection strings secured in Azure

------------------------------------------------------------------------

# ☁️ Azure Deployment

## 🌐 Azure App Service

The application is deployed to Azure Web App.

## 🗄 Azure SQL Database

Production database hosted in Azure SQL.

## ⚙ Configuration

-   Production connection string configured in Azure App Settings
-   Email confirmation disabled for internal system use
-   First Admin created and assigned role securely

------------------------------------------------------------------------

# 🔄 CI/CD Pipeline

## 🧪 Continuous Integration (CI)

Pipeline steps: - dotnet restore - dotnet build - dotnet publish -
Artifact generation

## 🚀 Continuous Deployment (Release)

-   Deploy build artifact to Azure App Service
-   Automatic deployment after successful build
-   Environment-based configuration

------------------------------------------------------------------------

# 🛠 Local Development Setup

## 1️⃣ Clone Repository

``` bash
git clone https://github.com/HanyMagdy-cloud/ATS.project.git
```

## 2️⃣ Update Connection String (appsettings.json)

``` json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=ATS.DB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## 3️⃣ Run Migrations

``` powershell
Update-Database
```

## 4️⃣ Run Project

``` bash
dotnet run
```

------------------------------------------------------------------------

# 📈 Production-Ready Capabilities

✔ Multi-tenant architecture\
✔ Role-based security\
✔ Azure cloud hosting\
✔ Automated CI/CD\
✔ Responsive UI\
✔ Clean architecture\
✔ Identity-based authentication

------------------------------------------------------------------------

# 📌 Author

Developed and deployed by Hany Magdy\
Full-stack ASP.NET Developer

------------------------------------------------------------------------

# ⭐ Project Status

Production deployed\
CI/CD automated\
Azure hosted
