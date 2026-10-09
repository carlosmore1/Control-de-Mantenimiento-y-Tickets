# Sequence Diagrams

## Ticket Creation

```mermaid
sequenceDiagram
    actor User
    participant Frontend
    participant API
    participant TicketService
    participant Database

    User->>Frontend: Enter ticket information
    User->>Frontend: Create Ticket

    Frontend->>API: POST /api/tickets
    API->>TicketService: Create ticket request

    TicketService->>Database: Insert Ticket with Pending status
    Database-->>TicketService: Ticket created

    TicketService-->>API: TicketResponse
    API-->>Frontend: Created ticket

    Frontend-->>User: Display ticket in Pending column
```

## Ticket Status Transition

```mermaid
sequenceDiagram
    actor Operator
    participant Frontend
    participant API
    participant TicketService
    participant StoredProcedure
    participant Database

    Operator->>Frontend: Select assigned operator
    Operator->>Frontend: Start Progress

    Frontend->>API: PUT /api/tickets/{id}/status
    API->>TicketService: Change status request

    TicketService->>TicketService: Validate transition
    TicketService->>TicketService: Validate assigned operator

    TicketService->>StoredProcedure: Change Pending to InProgress
    StoredProcedure->>Database: Update Ticket status and operator
    StoredProcedure->>Database: Insert TicketHistory
    Database-->>StoredProcedure: Transaction completed

    StoredProcedure-->>TicketService: Success
    TicketService-->>API: Updated TicketResponse
    API-->>Frontend: Updated ticket

    Frontend-->>Operator: Move ticket to In Progress
    Frontend-->>Operator: Show status notification
```

## Ticket Resolution

```mermaid
sequenceDiagram
    actor Operator
    participant Frontend
    participant API
    participant TicketService
    participant StoredProcedure
    participant Database

    Operator->>Frontend: Enter diagnosis
    Operator->>Frontend: Resolve Ticket

    Frontend->>API: PUT /api/tickets/{id}/status
    API->>TicketService: Change status request

    TicketService->>TicketService: Validate transition
    TicketService->>TicketService: Validate diagnosis

    TicketService->>StoredProcedure: Change InProgress to Resolved
    StoredProcedure->>Database: Update Ticket and diagnosis
    StoredProcedure->>Database: Insert TicketHistory
    Database-->>StoredProcedure: Transaction completed

    StoredProcedure-->>TicketService: Success
    TicketService-->>API: Updated TicketResponse
    API-->>Frontend: Updated ticket

    Frontend-->>Operator: Move ticket to Resolved
    Frontend-->>Operator: Show status notification
```

## Status Flow

```text
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