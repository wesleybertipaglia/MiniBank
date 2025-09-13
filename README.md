# MiniBank – Distributed Banking System

A distributed microservices-based banking system for simulating basic financial operations using modern software engineering practices.

## Table of Contents

* [Features](#features)
* [Getting Started](#getting-started)
* [Usage](#usage)
* [Entities](#entities)
* [Contributing](#contributing)
* [License](#license)

## Features

* Modular microservices architecture
* Asynchronous messaging with **RabbitMQ**
* Centralized API routing with **Ocelot**
* Structured logging with **Serilog**
* User authentication and email confirmation flow
* PostgreSQL integration with **Entity Framework Core**
* Transactional email simulation
* Caching support with **Redis**
* Automated unit testing with **xUnit**
* CI/CD with **GitHub Actions**
* Cloud-ready (AWS compatible)

## Getting Started

### Prerequisites

Make sure you have the following installed:

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/)
* [Docker Compose](https://docs.docker.com/compose/)

### 1. Clone the Repository

```bash
git clone https://github.com/wesleybertipaglia/MiniBank.git
cd MiniBank
```

### 2. Run Docker Infrastructure

```bash
docker compose up -d
```

This will start the **RabbitMQ** broker and **PostgreSQL** databases used by the services.

### 3. Run the Microservices

Each service has its own solution and follows a layered architecture. You can run them individually:

```bash
dotnet build
dotnet run --project MiniBank.Auth/MiniBank.Auth.Api                # Port 5020
dotnet run --project MiniBank.Bank/MiniBank.Bank.Api                # Port 5030
dotnet run --project MiniBank.Mailer/MiniBank.Mailer.Api            # Port 5040
dotnet run --project MiniBank.ApiGateway/MiniBank.ApiGateway.Api    # Port 5000
```

## Usage

You can interact with the services using the Swagger UI or any HTTP client through the API Gateway:

### Sign Up

`POST /auth/signup`

Registers a new user.

* Content-Type: `application/json`
* Body:

```json
{
  "name": "user",
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

Returns: `201 Created` on success with user details and token

### Sign In

`POST /auth/signin`

Authenticates a user and returns a JWT token.

* Content-Type: `application/json`
* Body:

```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

Returns: `200 OK` on success with user details and token

### Confirm Email

`GET /user/confirm-email/{userId}`

Confirms a user's email address using their ID.

Returns: `200 OK` on success or `404 Not Found` if the user does not exist

### Deposit to Account

`POST /accounts/{userId}/deposit`

Deposits funds into a user’s account.

* Content-Type: `application/json`
* Body:

```json
{
  "amount": 100.00,
  "description": "Initial deposit"
}
```

Returns: `200 OK` with updated account balance

## Entities

### Auth Service

* **User**: Registration, login, email confirmation, password management

### Bank Service

* **Account**: Balance, deposits, withdrawals, and transactions
* **Transaction**: Represents financial operations tied to accounts

### Mailer Service

* **Email**: Simulates sending transactional messages (confirmation, notifications)

## Contributing

Contributions are welcome!
Feel free to fork this repository and submit a pull request with a clear explanation of your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
