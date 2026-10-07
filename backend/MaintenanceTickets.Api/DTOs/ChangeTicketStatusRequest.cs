namespace MaintenanceTickets.Api.DTOs;

public class ChangeTicketStatusRequest
{
    public string NewStatus { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public string? Diagnosis { get; set; }
}