# SportsStore - Distributed Order Processing Platform

A full-stack e-commerce application built with .NET 10, Blazor, React, and RabbitMQ for event-driven order processing.

## Architecture Overview

```
Customer Checkout 
      │
      ▼
Order Management API (REST)
      │
      ▼
RabbitMQ Message Broker
      │
      ├──► Inventory Service ──► Payment Service ──► Shipping Service
      │         │                     │                    │
      └─────────┴─────────────────────┴────────────────────┘
                           │
                           ▼
                  Order Completed/Failed
```

## Services

| Service | Description | Port |
|---------|-------------|------|
| **SportsStore.OrderApi** | Central REST API for order management | 5000 |
| **SportsStore.InventoryService** | RabbitMQ consumer for inventory validation | - |
| **SportsStore.PaymentService** | RabbitMQ consumer for payment processing | - |
| **SportsStore.ShippingService** | RabbitMQ consumer for shipment creation | - |
| **SportsStore (Blazor)** | Customer-facing web application | 5001 |
| **sportsstore-admin (React)** | Admin dashboard | 3000 |
| **RabbitMQ** | Message broker | 5672, 15672 |

## Event Flow

1. **OrderSubmitted** - Customer completes checkout
2. **InventoryCheckRequested** - Check stock availability
3. **InventoryCheckCompleted/Failed** - Stock validation result
4. **PaymentProcessingRequested** - Initiate payment
5. **PaymentApproved/Rejected** - Payment result
6. **ShippingRequested** - Create shipment
7. **ShippingCreated** - Shipment created
8. **OrderCompleted/Failed** - Final state

## Order Status Lifecycle

```
Cart → Submitted → InventoryPending → PaymentPending → ShippingPending → Completed
                            ↓                ↓
                      InventoryFailed     PaymentFailed → Failed
```

## How to Run

### Prerequisites
- .NET 10 SDK
- Node.js 20+
- Docker & Docker Compose

### Option 1: Docker Compose (Recommended)

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Option 2: Local Development

```bash
# Start RabbitMQ
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management

# Run Order API
cd SportsStore.OrderApi && dotnet run

# Run services (in separate terminals)
cd SportsStore.InventoryService && dotnet run
cd SportsStore.PaymentService && dotnet run
cd SportsStore.ShippingService && dotnet run

# Run Blazor UI
cd SportsStore && dotnet run

# Run React Admin (in separate terminal)
cd sportsstore-admin && npm install && npm start
```

## API Endpoints

### Orders
- `POST /api/orders/checkout` - Create new order
- `GET /api/orders` - List all orders
- `GET /api/orders/{id}` - Get order details
- `GET /api/orders/{id}/status` - Get order status
- `POST /api/orders/{id}/cancel` - Cancel order

### Products
- `GET /api/products` - List products
- `GET /api/products/{id}` - Get product details
- `GET /api/products/categories` - List categories

### Admin
- `GET /api/orders/by-status/{status}` - Filter by status
- `GET /api/orders/dashboard/summary` - Dashboard stats

## Technologies Used

- **Backend**: .NET 10, ASP.NET Core Web API
- **Frontend**: Blazor, React, TypeScript
- **Messaging**: RabbitMQ (v7.0)
- **Database**: SQLite (development), SQL Server (production)
- **CQRS**: MediatR
- **Mapping**: AutoMapper
- **Logging**: Serilog
- **Containerization**: Docker, Docker Compose

## Testing

```bash
# Run .NET tests
dotnet test

# Run React tests
cd sportsstore-admin && npm test
```

## Project Structure

```
SportsSln/
├── SportsStore.Core/           # Domain entities, DTOs, interfaces
├── SportsStore.Infrastructure/ # RabbitMQ, messaging
├── SportsStore.OrderApi/       # Main REST API
├── SportsStore.InventoryService/
├── SportsStore.PaymentService/
├── SportsStore.ShippingService/
├── SportsStore/               # Blazor customer portal
├── sportsstore-admin/         # React admin dashboard
└── docker-compose.yml
```

## Configuration

Environment variables:
- `RabbitMQ__Host` - RabbitMQ hostname
- `RabbitMQ__Port` - RabbitMQ port
- `RabbitMQ__Username` - RabbitMQ username
- `RabbitMQ__Password` - RabbitMQ password
- `ConnectionStrings__DefaultConnection` - Database connection

## License

MIT
