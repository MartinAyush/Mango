# .NET Core Microservices E-Commerce Project AKA InstaBasket

This repository contains a fully functioning **e-commerce platform** built using **.NET 8** and **microservices architecture**. The project is designed to demonstrate how to implement various microservices with proper separation of concerns, asynchronous communication, secure authentication, and cloud-native deployment using **Azure**.

---

## Project Overview

This project implements an **e-commerce platform** using seven microservices, each responsible for a specific functionality, ensuring the system is modular, scalable, and easy to maintain. Below are the services included and their responsibilities:

### Microservices Overview

1. **Product Catalog Microservice**:
   - Handles product information, CRUD operations, and product listing.
   
2. **Shopping Cart (Basket) Microservice**:
   - Manages shopping cart operations.
   
3. **Order Microservice**:
   - Processes customer orders, including validation and order management.

4. **Coupon/Discount Microservice**:
   - Provides discount calculation functionality.

5. **Email Notification Microservice**:
   - Sends confirmation emails upon order placement using event-driven communication (sendgrid for SMTP server).

6. **Payment Microservice**:
   - Integrates payment processing and handles communication with external payment gateways (stripe payment gateway).

7. **Identity Microservice**:
   - Implements secure authentication and authorization using **.NET Identity**, with role-based access control.

---

## Technologies Used

- **.NET 8**: For building backend APIs and services.
- **ASP.NET Core MVC**: For the web framework and controllers.
- **Ocelot API Gateway**: To manage routing between different microservices.
- **Entity Framework Core**: For database management with **SQL Server**.
- **Azure Service Bus**: For asynchronous communication between microservices.
- **Swagger**: For API documentation and testing.

---

## Application Flow

1. **User Browses Products**: The user views products provided by the **Product Catalog Microservice**.
2. **Shopping Cart**: The user adds items to the cart, which is handled by the **Shopping Cart Microservice**.
3. **Applying Discounts**: The **Coupon Microservice** calculates discounts and applies them at checkout.
4. **Order Placement**: The **Order Microservice** processes the order and communicates with the **Payment Microservice** for secure payment using stripe payment gateway.
5. **Email Notifications**: After payment, the **Email Microservice** sends a confirmation email using .
6. **Authentication**: The **Identity Microservice** ensures secure login and role-based access throughout the platform.

---

## How to Set Up the Project

Follow these steps to set up and run the project locally:

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) 
- [Docker](https://www.docker.com/get-started)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (local or cloud-hosted)
- Visual Studio 2022 or higher

### Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/dotnet-microservices-ecommerce.git
   cd dotnet-microservices-ecommerce
   ```

2. Set up your environment variables:
   - Update the `appsettings.json` files for each microservice with your **SQL Server** configurations.`

3. The Front-End should be up and running. You can access the microservices via the **Ocelot API Gateway** at:
   ```
   https://localhost:7282
   ```

4. **Swagger UI** is enabled for each microservice, allowing you to test the APIs:
   ```
   http://localhost:XXXX/swagger/index.html
   ```

### Running the Application

Once the services are up and running:
- Access the e-commerce application via the web interface.
- Test various functionalities such as:
  - Browsing products.
  - Adding items to the shopping cart.
  - Applying discount coupons.
  - Placing an order.
  - Making a payment.
  - Receiving an email confirmation.

### Testing and Debugging

The solution includes unit tests for each microservice. You can run the tests via the **Test Explorer** in Visual Studio or using the **.NET CLI**:
```bash
dotnet test
```

---

## Deployed Application

You can check the deployed version of the application here: **[Live Demo Link](https://martinshoppingcart.azurewebsites.net)**

---

## Conclusion

This project demonstrates how to structure, implement, and deploy a modern, cloud-native e-commerce platform using **microservices architecture** with **.NET 8**. The solution incorporates clean architecture principles, event-driven communication, and cloud-ready deployments, making it scalable and maintainable for real-world applications.