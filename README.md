# InventoryHub

A full-stack inventory app built with Microsoft Copilot:

- **ServerApp**: ASP.NET Core minimal API (.NET 8) exposing `GET /api/productlist` and `GET /api/productlist/{id}`
- **ClientApp**: Blazor WebAssembly front end with a product list page (`/fetchproducts`)

## Features

- Front end / back end integration through a `ProductService` and a configurable API address (`ClientApp/wwwroot/appsettings.json`)
- CORS policy restricted to the client's origins (`ServerApp/appsettings.json` → `ClientOrigins`)
- Structured JSON: products with a nested category object, camelCase names
- Error handling for timeouts, an unreachable API, unexpected JSON and unknown ids (404)
- Performance: server-side memory cache, Brotli/Gzip response compression, client-side caching with a Refresh button

## Run locally

Requires the .NET 8 SDK.

```bash
# terminal 1: back end on http://localhost:5178
dotnet run --project ServerApp --urls http://localhost:5178

# terminal 2: front end on http://localhost:5108
dotnet run --project ClientApp --urls http://localhost:5108
```

Open http://localhost:5108/fetchproducts.

## Example response

```json
[
  { "id": 1, "name": "Laptop", "price": 1200.50, "stock": 25, "category": { "id": 101, "name": "Electronics" } }
]
```

See [REFLECTION.md](REFLECTION.md) for how Copilot was used in each step.
