using MaintenanceTickets.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceTickets.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>()
            .Property(ticket => ticket.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Ticket>()
            .HasMany(ticket => ticket.History)
            .WithOne(history => history.Ticket)
            .HasForeignKey(history => history.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}