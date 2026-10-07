namespace MaintenanceTickets.Api.Models;

public class Ticket
{
    public int TicketId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Asset { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; set; } = TicketStatus.Pending;

    public string? Diagnosis { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<TicketHistory> History { get; set; }
        = new List<TicketHistory>();
}