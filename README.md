# Maintenance Ticket Control

Maintenance Ticket Control is a full-stack web application developed for the Fractal Developer Training Program - Level 2 challenge.

The application allows users to create maintenance tickets, manage their status through a controlled workflow, and keep a persistent history of valid status changes.

## Main Features

- Create maintenance tickets.
- View tickets grouped by status.
- Move tickets from `Pending` to `InProgress`.
- Move tickets from `InProgress` to `Resolved`.
- Require a diagnosis before resolving a ticket.
- Reject invalid status transitions.
- Add optional comments during status changes.
- Store the history of ticket status changes.
- Update ticket status and history through a MySQL stored procedure.

## Technology Stack

### Frontend

- Angular
- TypeScript
- HTML
- CSS

### Backend

- ASP.NET Core Web API
- C#
- Entity Framework Core

### Database

- MySQL
- Entity Framework Core Migrations
- Stored Procedures

### Tools and Deployment

- Git
- GitHub
- Railway

## Ticket Workflow

The application uses the following ticket lifecycle:

```text
Pending -> InProgress -> Resolved
```

Allowed transitions:

```text
Pending -> InProgress
InProgress -> Resolved
```

Invalid transitions are rejected by the backend.

For example, the following transition is not allowed:

```text
Pending -> Resolved
```

A diagnosis is required before a ticket can be moved to `Resolved`.

## Project Structure

```text
.
├── backend/
│   └── MaintenanceTickets.Api/
│
├── database/
│   └── 01-change-ticket-status.sql
│
├── docs/
│   ├── architecture.drawio
│   ├── architecture.png
│   ├── data-model.md
│   ├── mockup.png
│   └── sequence-diagram.md
│
├── frontend/
│
├── .gitignore
├── PROMPTS.md
└── README.md
```

## System Architecture

The application separates the frontend, backend, business logic, data access, and database responsibilities.

```text
User / Operator
       |
       v
Angular Frontend
       |
       | HTTP / REST
       v
ASP.NET Core Web API
       |
       v
TicketsController
       |
       v
TicketService
       |
       v
ApplicationDbContext
Entity Framework Core
       |
       v
MySQL
```

The `TicketsController` receives HTTP requests.

The `TicketService` contains the ticket business rules and validates status transitions.

Entity Framework Core is used for data access and communication with MySQL.

The architecture diagram is available in:

```text
docs/architecture.png
```

The editable Draw.io file is available in:

```text
docs/architecture.drawio
```

## Data Model

The application currently uses two main entities.

### Ticket

Represents a maintenance incident.

Main fields:

- `TicketId`
- `Title`
- `Asset`
- `Description`
- `Status`
- `Diagnosis`
- `CreatedAt`
- `UpdatedAt`

### TicketHistory

Stores each valid ticket status change.

Main fields:

- `TicketHistoryId`
- `TicketId`
- `PreviousStatus`
- `NewStatus`
- `Comment`
- `CreatedAt`

Relationship:

```text
Ticket 1 ---- N TicketHistory
```

One ticket can have multiple history records.

More information about the data model is available in:

```text
docs/data-model.md
```

## Stored Procedure

The project uses the following MySQL stored procedure:

```text
sp_change_ticket_status
```

The stored procedure is responsible for:

- Reading the previous ticket status.
- Updating the ticket status.
- Updating the diagnosis when provided.
- Updating the modification date.
- Registering the status change in `TicketHistories`.
- Executing the update and history registration inside the same transaction.

The SQL script is located in:

```text
database/01-change-ticket-status.sql
```

## API Endpoints

### Get All Tickets

```http
GET /api/tickets
```

Returns all maintenance tickets.

### Create Ticket

```http
POST /api/tickets
```

Example request:

```json
{
  "title": "Printer failure",
  "asset": "PRN-001",
  "description": "The printer is not responding"
}
```

New tickets are created with the `Pending` status.

### Change Ticket Status

```http
PUT /api/tickets/{id}/status
```

Example request to move a ticket to `InProgress`:

```json
{
  "newStatus": "InProgress",
  "comment": "Technician started inspection"
}
```

Example request to resolve a ticket:

```json
{
  "newStatus": "Resolved",
  "comment": "Equipment tested successfully",
  "diagnosis": "Power cable was disconnected"
}
```

Invalid transitions are rejected by the backend.

## Running the Project Locally

### Requirements

The project was developed using:

- .NET SDK 10
- Node.js
- Angular CLI
- MySQL Server 8

## Database Configuration

Create the database:

```sql
CREATE DATABASE maintenance_tickets_db;
```

Configure a MySQL user with access to the database.

The database password and connection string are not stored directly in the source code.

Before running the backend, configure the connection string using an environment variable.

Example using PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection='Server=localhost;Port=3306;Database=maintenance_tickets_db;User=maintenance_app;Password=YOUR_PASSWORD;'
```

Replace `YOUR_PASSWORD` with the password configured for your MySQL user.

### Apply Database Migrations

Open a terminal in:

```text
backend/MaintenanceTickets.Api
```

Run:

```bash
dotnet ef database update
```

This creates the application tables using the existing Entity Framework Core migrations.

### Create the Stored Procedure

Execute the following SQL file in the `maintenance_tickets_db` database:

```text
database/01-change-ticket-status.sql
```

This creates:

```text
sp_change_ticket_status
```

## Run the Backend

Open a terminal in:

```text
backend/MaintenanceTickets.Api
```

Make sure the database connection environment variable is configured.

Run:

```bash
dotnet run
```

The local API is currently configured to run at:

```text
http://localhost:5054
```

## Run the Frontend

Open another terminal in:

```text
frontend
```

Install the dependencies:

```bash
npm install
```

Start the Angular application:

```bash
ng serve
```

Open the application in the browser:

```text
http://localhost:4200
```

## Documentation

Project documentation is stored in the `docs` directory.

It includes:

- Interface mockup.
- System architecture diagram.
- Data model.
- Sequence diagrams.

The interface mockup is available in:

```text
docs/mockup.png
```

The sequence diagrams are available in:

```text
docs/sequence-diagram.md
```

## AI Usage

Artificial intelligence was used as a support tool during the development process.

The prompts used during the project are documented in:

```text
PROMPTS.md
```

## Deployment

Public deployment is currently pending.

The application will be deployed using Railway.

The public frontend and backend URLs will be added to this section after deployment.