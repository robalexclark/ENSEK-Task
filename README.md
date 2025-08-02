# ENSEK-Task

This repository contains an end-to-end sample for loading and viewing electricity meter readings.

## Solution structure

- `MeterReadingsApi` – ASP.NET Core Web API responsible for managing meter readings.
  - **Controllers** expose REST endpoints for uploading readings and retrieving account data.
  - **Repositories** wrap Entity Framework Core access to a SQLite database seeded with sample accounts.
  - **Services** parse CSV uploads and coordinate validation using FluentValidation.
  - **Mappers** (AutoMapper) translate between EF entities and DTOs defined in the Shared project.
- `MeterReadingsApi.DataModel` – entity classes and the `MeterReadingsContext` Entity Framework Core context.
- `MeterReadingsApi.Shared` – shared DTOs consumed by both API and Blazor client.
- `MeterReadingsBlazorClient` – Blazor WebAssembly front-end using Blazorise DataGrid components to list accounts and related readings.
- Test projects verify different layers of the solution:
  - `MeterReadingsApi.UnitTests` isolate one class at a time and use Moq to mock dependencies.
  - `MeterReadingsApi.IntegrationTests` exercise the API endpoints with an in-memory database for fast end-to-end checks.
  - `MeterReadingsBlazorClient.BUnit` demonstrates bUnit tests that load accounts and ensure meter readings appear when an account is selected.

## Running the application

1. Start the API:

   ```bash
   dotnet run --project MeterReadingsApi/MeterReadingsApi
   ```

   The service listens on `https://localhost:7242` (and `http://localhost:5028`).

2. Start the Blazor WebAssembly client:

   ```bash
   dotnet run --project MeterReadingsApi/MeterReadingsBlazorClient
   ```

   Navigate to `http://localhost:5167`, which is configured to call the API at `https://localhost:7242`.

## Running the tests

```bash
dotnet test MeterReadingsApi/MeterReadingsApi.sln
```
