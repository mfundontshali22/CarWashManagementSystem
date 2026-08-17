# SudsLog — Car Wash Operations (ASP.NET Core MVC + SQL Server)

A daily-operations web app for a car wash business, built with **C# / ASP.NET Core MVC**,
**Entity Framework Core**, and **SQL Server** (created and managed through **SQL Server
Management Studio**).

## What's included

- **Dashboard** — today's wash count, revenue, active employees, bay status
- **Wash Log** — CarID, CarName, WashTime, and the Employee Responsible (full CRUD)
- **Employees** — EmployeeNum, EmployeeName, ServiceType (full CRUD)
- **Clients** — contact info, license plate, notes, linked to their wash history (full CRUD)

## Database schema

```
Employees(EmployeeNum PK, EmployeeName, ServiceType, Phone, Status)
Clients(ClientId PK, ClientName, Phone, Email, LicensePlate, Notes)
Washes(CarID PK, CarName, LicensePlate, WashTime, WashDate, ServiceType,
       Price, Status, EmployeeNum FK -> Employees, ClientId FK -> Clients)
```

Both foreign keys are nullable with `ON DELETE SET NULL`, so removing an employee or
client never deletes wash history — it just falls back to "Unassigned" / "Walk-in".

## Prerequisites

- **Visual Studio 2022** (Community edition is fine) with the **ASP.NET and web
  development** workload, *or* the .NET 8 SDK + VS Code
- **SQL Server** (Express, Developer, or full edition) and **SQL Server Management
  Studio (SSMS)**

## Setup — 3 steps

### 1. Create the database in SSMS

1. Open SQL Server Management Studio and connect to your instance.
2. Open a **New Query** window.
3. Open `Database/Schema.sql` from this project, paste its contents in, and click
   **Execute** (F5).

This creates the `CarWashDB` database with all three tables, the foreign keys, useful
indexes, and a few sample rows so you can see the app working immediately. It's safe to
re-run — it only creates things that don't already exist.

### 2. Point the app at your SQL Server instance

Open `appsettings.json` and check the connection string matches your server:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=CarWashDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

- If you installed the **default instance** of SQL Server (not Express), change
  `Server=.\SQLEXPRESS` to `Server=.` or `Server=localhost`.
- If you're using **SQL Server Authentication** instead of Windows auth, replace
  `Trusted_Connection=True` with `User Id=youruser;Password=yourpassword;`.
- You can find the exact server name in SSMS's connection dialog / Object Explorer title bar.

### 3. Run the app

**In Visual Studio:** open `CarWashOps.csproj`, wait for NuGet packages to restore, then
press **F5** (or the green ▶ Run button). Your browser will open to the Dashboard.

**From the command line:**
```bash
cd CarWashOps
dotnet restore
dotnet run
```
Then browse to the URL shown in the terminal (e.g. `https://localhost:7250`).

## Using EF Core migrations instead (optional)

If you'd rather let Entity Framework manage the schema instead of running the SQL
script by hand, skip step 1 above and instead run, from the project folder:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This reads the same models (`Wash`, `Employee`, `Client`) and creates a matching
database for you — useful if you plan to evolve the schema over time and want EF to
track those changes.

## Project structure

```
CarWashOps/
  Controllers/       HomeController, WashesController, EmployeesController, ClientsController
  Models/             Wash.cs, Employee.cs, Client.cs
  Data/                ApplicationDbContext.cs
  Views/              Razor views for the dashboard and CRUD screens
  Database/           Schema.sql — run this in SSMS
  wwwroot/css/        site.css — app styling
  appsettings.json     connection string lives here
```

## Extending it

Some natural next steps once this is running:
- Add a login system (ASP.NET Core Identity) so each employee has their own account
- Add a "checkout/payment" table to track how each wash was paid for
- Add email/SMS reminders for clients via a scheduled job
