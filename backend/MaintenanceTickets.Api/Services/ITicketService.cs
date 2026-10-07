using MaintenanceTickets.Api.DTOs;

namespace MaintenanceTickets.Api.Services;

public interface ITicketService
{
    Task<IEnumerable<TicketResponse>> GetAllAsync();
    Task<TicketResponse> CreateAsync(CreateTicketRequest request);
}