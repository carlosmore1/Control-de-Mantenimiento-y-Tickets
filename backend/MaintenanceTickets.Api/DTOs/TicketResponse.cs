namespace MaintenanceTickets.Api.DTOs;

public class TicketResponse
{
    public int TicketId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Asset { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Diagnosis { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? AssignedOperator { get; set; }
}