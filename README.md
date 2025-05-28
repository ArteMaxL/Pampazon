# Pampazon Warehouse Management API

This API provides endpoints to manage Pampazon's warehouse operations, including product management, stock control, and order processing.

## Getting Started

1. Clone the repository
2. Navigate to the project directory
3. Run `dotnet restore`
4. Run `dotnet run`

The API will be available at `https://localhost:7001` and `http://localhost:5001`

## API Endpoints

### Products

- `GET /api/products` - Get all products
- `GET /api/products/{code}` - Get a specific product
- `PUT /api/products/{code}` - Update a product
- `POST /api/products` - Create a new product
- `DELETE /api/products/{code}` - Delete a product

### Stock

- `GET /api/stock` - Get all stock positions
- `GET /api/stock/product/{productCode}` - Get stock positions for a product
- `POST /api/stock` - Create a new stock position
- `PUT /api/stock/{id}` - Update a stock position
- `DELETE /api/stock/{id}` - Delete a stock position

### Clients

- `GET /api/clients` - Get all clients
- `GET /api/clients/{cuit}` - Get a specific client
- `PUT /api/clients/{cuit}` - Update a client
- `POST /api/clients` - Create a new client
- `DELETE /api/clients/{cuit}` - Delete a client

### Receipts (Incoming Goods)

- `GET /api/receipts` - Get all receipts
- `GET /api/receipts/{id}` - Get a specific receipt
- `PUT /api/receipts` - Create a new receipt
- `POST /api/receipts/{id}/status` - Update receipt status
- `POST /api/receipts/{id}/positions` - Assign warehouse positions to received goods

### Orders (Outgoing Goods)

- `GET /api/orders` - Get all orders
- `GET /api/orders/{orderNumber}` - Get a specific order
- `PUT /api/orders` - Create a new order
- `POST /api/orders/{orderNumber}/status` - Update order status
- `POST /api/orders/{orderNumber}/positions` - Assign warehouse positions for order preparation

### Dispatches

- `GET /api/dispatches` - Get all dispatches
- `GET /api/dispatches/{dispatchNumber}` - Get a specific dispatch
- `PUT /api/dispatches` - Create a new dispatch
- `POST /api/dispatches/{dispatchNumber}/orders/{orderNumber}` - Add an order to a dispatch
- `POST /api/dispatches/{dispatchNumber}/finalize` - Finalize a dispatch

## Models

### Product
```json
{
  "code": "string",
  "description": "string",
  "height": 0,
  "width": 0,
  "depth": 0
}
```

### StockPosition
```json
{
  "id": 0,
  "aisle": "A",
  "section": 1,
  "shelf": 1,
  "level": 1,
  "quantity": 0,
  "productCode": "string",
  "clientId": "string"
}
```

### Client
```json
{
  "cuit": "string",
  "businessName": "string"
}
```

### Receipt
```json
{
  "id": 0,
  "date": "2024-05-27T00:00:00Z",
  "clientCUIT": "string",
  "carrierCUIT": "string",
  "status": "PendingEntry",
  "items": [
    {
      "productCode": "string",
      "quantity": 0
    }
  ]
}
```

### Order
```json
{
  "orderNumber": "string",
  "date": "2024-05-27T00:00:00Z",
  "clientCUIT": "string",
  "recipientName": "string",
  "recipientAddress": "string",
  "status": "Pending",
  "items": [
    {
      "productCode": "string",
      "quantity": 0
    }
  ]
}
```

### Dispatch
```json
{
  "dispatchNumber": "string",
  "date": "2024-05-27T00:00:00Z",
  "carrierCUIT": "string",
  "isFinalized": false
}
```

## Testing

You can test the API using tools like:
- REST Client (VS Code extension)
- Postman
- Insomnia

Example request using REST Client:
```http
### Get all products
GET https://localhost:7001/api/products

### Create a new product
POST https://localhost:7001/api/products
Content-Type: application/json

{
  "code": "PROD001",
  "description": "Test Product",
  "height": 10,
  "width": 20,
  "depth": 30
}
``` 