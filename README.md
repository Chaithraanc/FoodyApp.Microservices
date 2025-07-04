# 🍽️ FoodyApp.Microservices

**Online Food Ordering System using ASP.NET Core Microservices Architecture (.NET 8)**

---

## 🛠️ Overview

This is a cloud-deployed, microservices-based e-commerce system built using **ASP.NET 8**. It follows **clean architecture principles**, modular design, and leverages **Azure App Services** and **Azure Service Bus** for cloud-native scalability.

---

## 📘 About

This project was developed as a personal learning exercise based on the Udemy course:  
**"Learn Microservices Architecture with .NET Core MVC (.NET 8), Entity Framework Core, .NET Identity, and Azure Service Bus"**.

> 🔹 All code was written independently, with additional improvements and deployment on Azure.

---

## 🧩 Microservices Implemented

- ✅ **Product Service**
- ✅ **Coupon Service**
- ✅ **Shopping Cart Service**
- ✅ **Order Service**
- ✅ **Payment Service**
- ✅ **Email Notification Service**
- ✅ **Identity Service** (JWT-based Auth with .NET Identity)
- ✅ **Ocelot API Gateway**
- ✅ **MVC Web Front-End** (Admin + User)

---

## 💻 Tech Stack

| Layer                | Technology                              |
|---------------------|------------------------------------------|
| Backend             | ASP.NET Core 8, Web API, MVC             |
| Authentication      | .NET Identity, JWT Tokens                |
| Messaging           | Azure Service Bus                        |
| Data Access         | Entity Framework Core                    |
| Database            | SQL Server (Azure)                       |
| API Gateway         | Ocelot                                    |
| Architecture Style  | Clean Architecture (Domain, Application, Infrastructure) |
| Deployment          | Azure App Services, GitHub Actions (CI/CD) |

---

## ☁️ Deployment on Azure

The entire system is deployed to **Azure App Services**, with each microservice running in a separate web app, and orchestrated via **Ocelot API Gateway**.

### 🌐 Live Demo
🔗 [https://foodyweb-hbd7gja8b6chc7ap.eastasia-01.azurewebsites.net/](https://foodyweb-hbd7gja8b6chc7ap.eastasia-01.azurewebsites.net/)

---

### 🚀 Deployment Highlights

- 🧩 **Modular Deployment**: Each service hosted as an independent Azure App Service.
- 🚪 **API Gateway**: Ocelot used as a reverse proxy and routing layer.
- 📬 **Service Bus**: Azure Service Bus enables asynchronous, decoupled communication.
- 🗃 **Data Storage**: SQL Server deployed in Azure for persistent storage.
- 🔧 **App Configuration**: Managed via Azure App Configuration and environment variables.
- 🔄 **CI/CD Pipeline**: Automated deployments via GitHub Actions & Visual Studio Publish.

---

## 💳 Payment Integration (Stripe Sandbox)

This project uses the **Stripe API in Sandbox mode** to simulate real payment processing during checkout.

- ✅ Users are redirected to **Stripe-hosted payment pages**.
- ✅ Payments are processed using **Stripe test cards**.
- ✅ After a successful payment, an order confirmation email is sent via **Email Microservice**, triggered through **Azure Service Bus**.

> 💡 No real money is involved. The Stripe sandbox environment is used for secure development and demo purposes.

---

## 🔒 License

This project is licensed under the **MIT License**.

