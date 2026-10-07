# Data Model

The system uses two main entities: `Ticket` and `TicketHistory`.

A ticket can have multiple history records, creating a one-to-many relationship.

```mermaid
erDiagram
    TICKET ||--o{ TICKET_HISTORY : has

    TICKET {
        int TicketId PK
        string Title
        string Asset
        string Description
        string Status
        string Diagnosis
        datetime CreatedAt
        datetime UpdatedAt
    }

    TICKET_HISTORY {
        int TicketHistoryId PK
        int TicketId FK
        string PreviousStatus
        string NewStatus
        string Comment
        datetime CreatedAt
    }