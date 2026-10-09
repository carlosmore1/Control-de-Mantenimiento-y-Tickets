# API Documentation

## Overview

The Maintenance Ticket Control API provides endpoints to create, list, and update maintenance tickets.

The API manages the ticket lifecycle:

```text
Pending → InProgress → Resolved
```

It also validates business rules, assigns operators, stores diagnoses, and persists ticket status changes in the history table.

## Base URLs

### Local

```text
http://localhost:5054/api/tickets
```

### Production

```text
https://backend-production-68463.up.railway.app/api/tickets
```

---

## Ticket Model

A ticket contains the following information:

```json
{
  "ticketId": 1,
  "title": "ATM screen failure",
  "asset": "ATM-001",
  "description": "The touch screen does not respond.",
  "status": "Pending",
  "diagnosis": null,
  "assignedOperator": null,
  "createdAt": "2026-10-09T15:00:00Z",
  "updatedAt": null
}
```

Possible status values:

```text
Pending
InProgress
Resolved
```

---

# Endpoints

## GET /api/tickets

Returns all tickets stored in the database.

### Request

```http
GET /api/tickets
```

### Successful response

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
    "description": "The touch screen does not respond.",
    "status": "Pending",
    "diagnosis": null,
    "assignedOperator": null,
    "createdAt": "2026-10-09T15:00:00Z",
    "updatedAt": null
  }
]
```

---

## POST /api/tickets

Creates a new maintenance ticket.

Every new ticket is created with the `Pending` status.

### Request

```http
POST /api/tickets
Content-Type: application/json
```

Body:

```json
{
  "title": "ATM screen failure",
  "asset": "ATM-001",
  "description": "The touch screen does not respond."
}
```

### Successful response

The created ticket is returned.

Example:

```json
{
  "ticketId": 1,
  "title": "ATM screen failure",
  "asset": "ATM-001",
  "description": "The touch screen does not respond.",
  "status": "Pending",
  "diagnosis": null,
  "assignedOperator": null,
  "createdAt": "2026-10-09T15:00:00Z",
  "updatedAt": null
}
```

### Validation

The following fields are required:

- `title`
- `asset`
- `description`

---

## PUT /api/tickets/{id}/status

Changes the status of an existing ticket.

The endpoint validates the ticket lifecycle before executing the transition.

### Valid transitions

```text
Pending → InProgress
InProgress → Resolved
```

Other transitions are rejected.

---

### Pending to In Progress

An operator must be assigned when a ticket starts progress.

Request:

```http
PUT /api/tickets/1/status
Content-Type: application/json
```

Body:

```json
{
  "newStatus": "InProgress",
  "comment": "The equipment is being inspected.",
  "assignedOperator": "Operator 1"
}
```

Business rules:

- The current status must be `Pending`.
- The new status must be `InProgress`.
- `assignedOperator` is required.

After the transition:

- The ticket status is updated.
- The operator is stored in the ticket.
- The transition is stored in `TicketHistories`.
- The update date is recorded.

---

### In Progress to Resolved

A diagnosis is required before resolving a ticket.

Request:

```http
PUT /api/tickets/1/status
Content-Type: application/json
```

Body:

```json
{
  "newStatus": "Resolved",
  "comment": "The incident was corrected.",
  "diagnosis": "The touch controller cable was loose."
}
```

Business rules:

- The current status must be `InProgress`.
- The new status must be `Resolved`.
- `diagnosis` is required.
- The previously assigned operator is preserved.

After the transition:

- The status is changed to `Resolved`.
- The diagnosis is stored.
- The assigned operator remains associated with the ticket.
- The transition is stored in `TicketHistories`.
- The update date is recorded.

---

# Business Rules

The application implements the following ticket lifecycle:

```text
Pending
   |
   v
InProgress
   |
   v
Resolved
```

The API blocks invalid transitions such as:

```text
Pending → Resolved
Resolved → InProgress
Resolved → Pending
```

A ticket cannot move from `Pending` to `InProgress` without an assigned operator.

A ticket cannot move from `InProgress` to `Resolved` without a diagnosis.

These validations are implemented in the service layer instead of the controller.

---

# Ticket History

Every valid ticket status transition generates a history record.

Example:

```text
Ticket 1
Pending → InProgress
Comment: The equipment is being inspected.
```

Then:

```text
Ticket 1
InProgress → Resolved
Comment: The incident was corrected.
```

The relationship is:

```text
Ticket 1 ───── N TicketHistory
```

The status update and history creation are executed through the MySQL stored procedure:

```text
sp_change_ticket_status
```

The procedure performs the ticket update and history insertion inside a database transaction.

---

# Error Handling

Expected business errors return controlled responses instead of generic server errors.

Examples include:

### Ticket not found

```text
404 Not Found
```

### Invalid status transition

```text
400 Bad Request
```

### Missing assigned operator

```text
400 Bad Request
```

Example message:

```text
An operator is required to start progress.
```

### Missing diagnosis

```text
400 Bad Request
```

Example message:

```text
A diagnosis is required to resolve the ticket.
```

---

# Bonus Features

The application also includes:

- Ticket assignment to operators.
- Ticket filtering by status.
- Ticket filtering by creation date.
- Simulated frontend notifications after successful status changes.