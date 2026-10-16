# 🏁 RaceDay API (PROG6212 - Part 2)

A robust, enterprise-grade RESTful API backend built with **ASP.NET Core 8**, **Entity Framework Core**, and **SQL Server** designed to support athletic event management, user authentication, secure role-based access control (RBAC), participant registrations, and official race leaderboard tracking.

---

## 🚀 Features & Architecture

* **Secure Authentication & RBAC:** JWT Bearer token authentication integrated with **BCrypt** password hashing. Enforces strict role separation between **Organisers** and **Participants**.
* **Relational Data Management:** Entity Framework Core Code-First architecture mapping `Users`, `Roles`, `Categories`, `Events`, `EventEnrolments`, and `Results` with cascade-loop prevention constraints (`DeleteBehavior.Restrict`).
* **Interactive API Documentation:** Fully configured **Swagger / OpenAPI** interface supporting direct bearer token authorization testing.
* **Automated Unit Testing:** Comprehensive `xUnit` test suite leveraging EF Core InMemory databases for isolated, high-speed test execution.
* **Continuous Integration (CI):** Automated GitHub Actions pipeline ensuring successful build compilation and test verification on every push.

---

## 🛠️ Tech Stack & Tools

* **Backend Framework:** ASP.NET Core 8 Web API
* **ORM:** Entity Framework Core (Microsoft SQL Server)
* **Security & Auth:** Microsoft.AspNetCore.Authentication.JwtBearer & BCrypt.Net-Next
* **Testing:** xUnit
* **Version Control:** Git & GitHub

---

## ⚙️ Setup & Installation Instructions

### **1. Prerequisites**
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed on your machine.
* Microsoft SQL Server or SQL Server Express running locally.

### **2. Clone the Repository**
```bash
git clone [https://github.com/ST10479035/prog2.git](https://github.com/ST10479035/prog2.git)
cd prog2
