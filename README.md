# Maintenance Ticket Control

Maintenance Ticket Control is a full-stack web application developed for the Fractal Developer Training Program - Level 2 challenge.

The application manages maintenance incidents through tickets, controlled status transitions, business rules, operator assignment, diagnosis validation, comments, filters, notifications, and a persistent history of status changes.

## Live Application

### Frontend

https://frontend-production-e04f.up.railway.app

### Backend API

https://backend-production-68463.up.railway.app

### Tickets Endpoint

https://backend-production-68463.up.railway.app/api/tickets

## Main Features

- Create maintenance tickets.
- View tickets grouped by status.
- Move tickets from `Pending` to `InProgress`.
- Move tickets from `InProgress` to `Resolved`.
- Require an assigned operator before starting progress.
- Persist the assigned operator in the ticket.
- Require a diagnosis before resolving a ticket.
- Reject invalid status transitions.
- Add optional comments during status changes.
- Store every valid status change in ticket history.
- Persist ticket information in MySQL.
- Update ticket status and history through a MySQL stored procedure.
- Filter tickets by status.
- Filter tickets by creation date.
- Display simulated notifications after successful status changes.
- Display controlled validation errors in the frontend.
- Run the complete application publicly on Railway.

## Bonus Features

The project includes the optional bonus features:

- Ticket assignment to operators.
- Ticket filtering by status.
- Ticket filtering by creation date.
- Simulated notifications after successful status changes.

An operator must be assigned before a ticket can move from `Pending` to `InProgress`.

The assigned operator is persisted in the database and remains associated with the ticket when it is resolved.

## Ticket Workflow

The application uses the following lifecycle:

```text
Pending -> InProgress -> Resolved
```

Allowed transitions:

```text
Pending -> InProgress
InProgress -> Resolved
```

Invalid transitions are rejected by the backend.

For example:

```text
Pending -> Resolved
```

is not allowed directly.

An operator is required before a ticket can be moved from `Pending` to `InProgress`.

A diagnosis is required before a ticket can be moved from `InProgress` to `Resolved`.

## Technology Stack

### Frontend

- Angular
- TypeScript
- HTML
- CSS
- Nginx

### Backend

- ASP.NET Core Web API
- C#
- Entity Framework Core

### Database

- MySQL
- Entity Framework Core Migrations
- MySQL Stored Procedures

### Deployment and Tools

- Railway
- Docker
- Git
- GitHub

## Technology Choices

### Angular

Angular was selected for the frontend because it provides a component-based architecture, form handling, HTTP communication, and a clear structure for building the ticket board and its interactions.

### ASP.NET Core

ASP.NET Core was selected for the backend because it provides a strongly typed REST API, dependency injection, and a clear separation between controllers, business logic, and data access.

### Entity Framework Core

Entity Framework Core is used to manage relational persistence, entity mappings, and database migrations while keeping database access separated from the controller layer.

### MySQL

MySQL was selected as the relational database because the application requires referential integrity between tickets and their history records. It also integrates with Entity Framework Core and Railway.

### Railway

Railway is used to deploy the Angular frontend, ASP.NET Core backend, and MySQL database in the same cloud environment while exposing the application through public URLs.

### Docker and Nginx

Docker provides reproducible production builds for the frontend and backend.

Nginx is used to serve the compiled Angular application in production.

## Project Structure

```text
.
├── backend/
│   └── MaintenanceTickets.Api/
│       ├── Controllers/
│       ├── Data/
│       ├── DTOs/
│       ├── Migrations/
│       ├── Models/
│       ├── Services/
│       ├── Dockerfile
│       └── Program.cs
│
├── database/
│   └── 01-change-ticket-status.sql
│
├── docs/
│   ├── api.md
│   ├── architecture.drawio
│   ├── architecture.png
│   ├── data-model.md
│   ├── mockup.png
│   └── sequence-diagram.md
│
├── frontend/
│   ├── src/
│   ├── Dockerfile
│   └── nginx.conf
│
├── .gitignore
├── PROMPTS.md
└── README.md
```

## System Architecture

The application separates frontend, backend, business logic, data access, and database responsibilities.

```text
User / Operator
       |
       | HTTPS
       v
Railway Frontend
Nginx + Angular
       |
       | HTTPS / REST API
       v
Railway Backend
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
       | Railway Private Network
       v
Railway MySQL
```

### Frontend

The Angular frontend is responsible for:

- Displaying the ticket management interface.
- Creating new tickets.
- Showing tickets grouped by status.
- Assigning operators before starting ticket progress.
- Sending status changes to the backend.
- Requesting a diagnosis before resolving a ticket.
- Filtering tickets by status and creation date.
- Displaying simulated status-change notifications.
- Displaying validation errors returned by the application.

The production Angular files are served using Nginx.

### Backend

The ASP.NET Core Web API is responsible for:

- Receiving HTTP requests.
- Validating ticket data.
- Applying business rules.
- Controlling valid status transitions.
- Requiring an operator before starting progress.
- Requiring a diagnosis before resolving a ticket.
- Rejecting invalid transitions.
- Executing database operations.

The `TicketsController` handles HTTP requests while the `TicketService` contains the business logic.

### Data Access

Entity Framework Core is used for:

- Mapping application entities to MySQL tables.
- Creating and reading tickets.
- Managing database migrations.
- Executing the stored procedure used for status transitions.

### Database

MySQL stores the ticket and ticket history information.

The backend communicates with MySQL using Railway private networking.

The production architecture diagram is available in:

```text
docs/architecture.png
```

The editable Draw.io file is available in:

```text
docs/architecture.drawio
```

## Data Model

The application contains two main entities.

### Ticket

Represents a maintenance incident.

Main fields:

- `TicketId`
- `Title`
- `Asset`
- `Description`
- `Status`
- `Diagnosis`
- `AssignedOperator`
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

The relationship is:

```text
Ticket 1 ---- N TicketHistory
```

One ticket can have multiple history records.

More information is available in:

```text
docs/data-model.md
```

## Stored Procedure

Ticket status changes are processed using the following MySQL stored procedure:

```text
sp_change_ticket_status
```

The stored procedure is responsible for:

- Reading the previous ticket status.
- Updating the ticket status.
- Updating the diagnosis when provided.
- Updating the assigned operator when provided.
- Preserving the assigned operator during later transitions.
- Updating the modification date.
- Inserting the status change into `TicketHistories`.
- Executing the update and history registration inside the same transaction.

The SQL source code is located in:

```text
database/01-change-ticket-status.sql
```

## API Endpoints

### Get All Tickets

```http
GET /api/tickets
```

Returns all maintenance tickets.

Production example:

```text
https://backend-production-68463.up.railway.app/api/tickets
```

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
  "comment": "Technician started inspection",
  "assignedOperator": "Operator 1"
}
```

An assigned operator is required before a ticket can start progress.

Example request to resolve a ticket:

```json
{
  "newStatus": "Resolved",
  "comment": "Equipment tested successfully",
  "diagnosis": "Power cable was disconnected"
}
```

The backend rejects invalid transitions and requires a diagnosis before resolving a ticket.

The operator assigned during the first transition remains associated with the ticket after resolution.

Complete API documentation is available in:

```text
docs/api.md
```

## Sequence Diagrams

The project documents the main application flows:

1. Ticket creation.
2. Ticket assignment and transition from `Pending` to `InProgress`.
3. Ticket resolution and history registration.

The sequence diagrams are available in:

```text
docs/sequence-diagram.md
```

## Interface Mockup

A low-fidelity interface mockup was created for the maintenance ticket board.

The mockup includes:

- Ticket creation form.
- Pending column.
- In Progress column.
- Resolved column.
- Status transition actions.
- Diagnosis input.
- Optional comments.

The mockup is available in:

```text
docs/mockup.png
```

## Running the Project Locally

### Requirements

Install:

- .NET SDK 10
- Node.js
- Angular CLI
- MySQL Server
- Git

## Database Setup

Create the local database:

```sql
CREATE DATABASE maintenance_tickets_db;
```

Configure a MySQL user with access to the database.

The database connection string and password are not stored directly in the source code.

Before running the backend, configure the connection string using an environment variable.

Example using PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection='Server=localhost;Port=3306;Database=maintenance_tickets_db;User=maintenance_app;Password=YOUR_PASSWORD;'
```

Replace `YOUR_PASSWORD` with the password configured for the local MySQL user.

## Apply Database Migrations

Open a terminal in:

```text
backend/MaintenanceTickets.Api
```

Run:

```bash
dotnet ef database update
```

Entity Framework Core will create and update the required application tables.

The migrations include the `AssignedOperator` field used by the bonus ticket-assignment functionality.

## Create the Stored Procedure

Execute the following file in the `maintenance_tickets_db` database:

```text
database/01-change-ticket-status.sql
```

This creates:

```text
sp_change_ticket_status
```

The stored procedure must be created after applying the database migrations.

## Run the Backend

Open a terminal in:

```text
backend/MaintenanceTickets.Api
```

Make sure the connection string environment variable is configured.

Run:

```bash
dotnet run
```

The local API runs at:

```text
http://localhost:5054
```

The local tickets endpoint is:

```text
http://localhost:5054/api/tickets
```

## Run the Frontend

Open another terminal in:

```text
frontend
```

Install dependencies:

```bash
npm install
```

Start the Angular development server:

```bash
ng serve
```

Open:

```text
http://localhost:4200
```

The development environment communicates with:

```text
http://localhost:5054/api/tickets
```

## Environment Configuration

The project uses environment-based configuration instead of hardcoded private credentials.

### Backend

The database connection is configured through:

```text
ConnectionStrings__DefaultConnection
```

CORS origins can be configured through:

```text
AllowedOrigins
```

The Railway application port is obtained from:

```text
PORT
```

### Frontend

Angular uses separate configuration for development and production.

Development API:

```text
http://localhost:5054/api/tickets
```

Production API:

```text
https://backend-production-68463.up.railway.app/api/tickets
```

## Deployment

The complete application is deployed on Railway.

The production environment contains three main services:

```text
Railway
├── Frontend
│   ├── Angular
│   └── Nginx
│
├── Backend
│   └── ASP.NET Core Web API
│
└── MySQL
    ├── Tickets
    ├── TicketHistories
    ├── __EFMigrationsHistory
    └── sp_change_ticket_status
```

### Frontend Deployment

The Angular application is built using Docker and served with Nginx.

Public URL:

```text
https://frontend-production-e04f.up.railway.app
```

### Backend Deployment

The ASP.NET Core API is built and deployed using Docker.

Public URL:

```text
https://backend-production-68463.up.railway.app
```

The backend automatically applies pending Entity Framework Core migrations when the application starts.

### Database Deployment

MySQL runs as a Railway database service.

The backend connects to MySQL through Railway private networking.

The database contains:

- `Tickets`
- `TicketHistories`
- `__EFMigrationsHistory`
- `sp_change_ticket_status`

The `Tickets` table includes the `AssignedOperator` field used to persist operator assignment.

Database credentials are configured using Railway environment variables and are not stored in the repository.

## CORS Configuration

The backend uses configurable CORS origins.

For local development:

```text
http://localhost:4200
```

For production:

```text
https://frontend-production-e04f.up.railway.app
```

This allows the Angular frontend to communicate securely with the ASP.NET Core API.

## Documentation

Project documentation is available in the `docs` directory:

- `api.md` - REST API documentation.
- `architecture.drawio` - Editable production architecture diagram.
- `architecture.png` - Production architecture image.
- `data-model.md` - Ticket and TicketHistory data model.
- `mockup.png` - Interface mockup.
- `sequence-diagram.md` - Application sequence diagrams.

## AI Usage

Artificial intelligence was used as a support tool during the development process.

It was used mainly for:

- Reviewing the initial technology stack.
- Defining the initial project structure.
- Supporting backend and database configuration.
- Troubleshooting Entity Framework and MySQL issues.
- Supporting Angular development and debugging.
- Implementing and reviewing the ticket workflow.
- Supporting deployment configuration.
- Supporting the implementation of bonus filters, notifications, and operator assignment.
- Reviewing technical documentation.

The prompts used during development are documented in:

```text
PROMPTS.md
```

## Repository

This repository contains:

- Angular frontend source code.
- ASP.NET Core backend source code.
- Entity Framework Core migrations.
- MySQL stored procedure.
- Docker deployment configuration.
- Nginx production configuration.
- Interface mockup.
- Architecture diagram.
- Data model documentation.
- REST API documentation.
- Sequence diagrams.
- AI prompt documentation.

## Final Status

The Level 2 requirements are implemented and publicly deployed.

The application currently supports the complete maintenance ticket workflow:

```text
Create Ticket
     |
     v
Pending
     |
     | Assigned operator required
     v
InProgress
     |
     | Diagnosis required
     v
Resolved
```

Each valid transition is controlled by backend business rules and persisted in the database history.

The final implementation also includes:

- Operator assignment.
- Status filtering.
- Creation-date filtering.
- Simulated status-change notifications.
- Public frontend deployment.
- Public backend API.
- Persistent MySQL database.
- Database migrations.
- Stored procedure-based status transitions.
- Technical diagrams and documentation.