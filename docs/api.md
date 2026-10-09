# Maintenance Ticket Control API

This document describes the REST API used by the Maintenance Ticket Control application.

## Base URLs

### Local Development

```text
http://localhost:5054
```

### Production

```text
https://backend-production-68463.up.railway.app
```

## Endpoints

### Get All Tickets

Returns all maintenance tickets.

```http
GET /api/tickets
```

#### Example Request

```text
GET https://backend-production-68463.up.railway.app/api/tickets
```

#### Success Response

Status:

```text
200 OK
```

Example:

```json
[
  {
    "ticketId": 1,
    "title": "ATM screen failure",
    "asset": "ATM-001",
    "description": "The screen is not responding",
    "status": "Resolved",
    "diagnosis": "Display connection cable was disconnected",
    "createdAt": "2026-10-08T20:00:00",
    "updatedAt": "2026-10-08T20:15:00"
  }
]
```

If there are no tickets, the API returns:

```json
[]
```

---

### Create Ticket

Creates a new maintenance ticket.

```http
POST /api/tickets
```

New tickets are automatically created with the status:

```text
Pending
```

#### Request Body

```json
{
  "title": "Printer failure",
  "asset": "PRN-001",
  "description": "The printer is not responding"
}
```

#### Success Response

Status:

```text
201 Created
```

Example:

```json
{
  "ticketId": 2,
  "title": "Printer failure",
  "asset": "PRN-001",
  "description": "The printer is not responding",
  "status": "Pending",
  "diagnosis": null,
  "createdAt": "2026-10-08T20:30:00",
  "updatedAt": null
}
```

#### Validation

The following fields are required:

- `title`
- `asset`
- `description`

If any required field is missing or empty, the API returns:

```text
400 Bad Request
```

---

### Change Ticket Status

Changes the status of an existing ticket.

```http
PUT /api/tickets/{id}/status
```

The application uses the following workflow:

```text
Pending -> InProgress -> Resolved
```

Only valid transitions are accepted.

---

#### Move Ticket to InProgress

Example request:

```http
PUT /api/tickets/1/status
```

Request body:

```json
{
  "newStatus": "InProgress",
  "comment": "Technician started inspection"
}
```

#### Success Response

Status:

```text
200 OK
```

Example:

```json
{
  "ticketId": 1,
  "title": "Printer failure",
  "asset": "PRN-001",
  "description": "The printer is not responding",
  "status": "InProgress",
  "diagnosis": null,
  "createdAt": "2026-10-08T20:30:00",
  "updatedAt": "2026-10-08T20:35:00"
}
```

---

#### Resolve Ticket

A ticket can only be resolved from the `InProgress` state.

A diagnosis is required.

Example request:

```http
PUT /api/tickets/1/status
```

Request body:

```json
{
  "newStatus": "Resolved",
  "comment": "Equipment tested successfully",
  "diagnosis": "Power cable was disconnected"
}
```

#### Success Response

Status:

```text
200 OK
```

Example:

```json
{
  "ticketId": 1,
  "title": "Printer failure",
  "asset": "PRN-001",
  "description": "The printer is not responding",
  "status": "Resolved",
  "diagnosis": "Power cable was disconnected",
  "createdAt": "2026-10-08T20:30:00",
  "updatedAt": "2026-10-08T20:45:00"
}
```

## Business Rules

The backend controls the ticket lifecycle.

Valid transitions:

```text
Pending -> InProgress
InProgress -> Resolved
```

Examples of invalid transitions:

```text
Pending -> Resolved
Resolved -> InProgress
Resolved -> Pending
```

Invalid transitions return:

```text
400 Bad Request
```

A ticket cannot be resolved without a diagnosis.

If the diagnosis is missing, the API returns:

```text
400 Bad Request
```

## Error Responses

### Ticket Not Found

If the requested ticket does not exist:

```text
404 Not Found
```

### Invalid Status

If the requested status is not supported:

```text
400 Bad Request
```

### Invalid Transition

If the requested status transition is not allowed:

```text
400 Bad Request
```

### Missing Diagnosis

If a ticket is moved to `Resolved` without a diagnosis:

```text
400 Bad Request
```

## Ticket Status Values

The API currently uses the following status values:

```text
Pending
InProgress
Resolved
```

## Ticket History

Every valid status transition is persisted in the `TicketHistories` table.

Each history record stores:

- Ticket identifier.
- Previous status.
- New status.
- Optional comment.
- Creation date.

Status updates and history registration are executed through the MySQL stored procedure:

```text
sp_change_ticket_status
```

The stored procedure performs the ticket update and history insertion inside the same transaction.