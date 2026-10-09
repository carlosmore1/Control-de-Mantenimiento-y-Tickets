using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceTickets.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedOperatorToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedOperator",
                table: "Tickets",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedOperator",
                table: "Tickets");
        }
    }
}
