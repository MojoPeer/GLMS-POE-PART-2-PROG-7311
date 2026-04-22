# Global Logistics Management System (GLMS)

## Project Description
GLMS is a web-based logistics management system built with ASP.NET Core MVC (.NET 8). It simplifies the management of clients, contracts, and service requests for logistics companies. The system replaces manual processes like emails and spreadsheets with a centralized application.

## Key Features
- **Client Management**: Create, view, edit, and delete clients.
- **Contract Management**: Assign contracts to clients, upload/download signed agreements (PDF only), and filter contracts by status and date range.
- **Service Requests**: Create service requests linked to contracts, with automatic USD to ZAR currency conversion.
- **Business Rules**: Prevent service requests for contracts that are `Draft`, `Expired`, or `On Hold`.
- **Mock Data**: Preloaded with realistic logistics data for demonstration.
- **Unit Tests**: Includes tests for currency conversion, file validation, and business rules.
- **GitHub Actions**: Automated build and test workflow.

## System Requirements
- .NET 8 SDK
- SQL Server
- Modern web browser

## Setup Instructions
1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/glms.git
   cd glms
   ```
2. Update the connection string in `appsettings.json` to point to your SQL Server instance.
3. Apply migrations to create the database:
   ```bash
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run
   ```
5. Open your browser and navigate to `https://localhost:5001`.

## How to Run Tests
1. Navigate to the test project directory:
   ```bash
   cd GLMS.Tests
   ```
2. Run the tests:
   ```bash
   dotnet test
   ```

## Mock Data
The system comes preloaded with:
- 5 clients
- 5 contracts (with mixed statuses: `Draft`, `Active`, `Expired`, `On Hold`)
- 4 service requests

This data is seeded automatically on application startup.

## Migration Files
Ensure the `Migrations` folder exists and contains the necessary migration files. If not, generate migrations:
```bash
dotnet ef migrations add InitialCreate
```

## Screenshots
(Include screenshots here)

## Video Demonstration Checklist
1. Create a client.
2. Assign a contract to the client.
3. Upload a valid PDF.
4. Attempt to upload an invalid file (e.g., `.jpg`) and show rejection.
5. Create a valid service request.
6. Attempt to create a service request for a `Draft`/`Expired`/`On Hold` contract and show rejection.
7. Demonstrate currency conversion (USD to ZAR).
8. Download a signed agreement.
9. Filter contracts by status and date range.

## Code Attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

