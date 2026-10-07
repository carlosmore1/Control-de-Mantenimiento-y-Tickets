namespace MaintenanceTickets.Api.DTOs;

public class CreateTicketRequest
{
    public string Title { get; set; } = string.Empty;
    public string Asset { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}