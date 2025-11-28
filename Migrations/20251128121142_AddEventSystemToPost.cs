using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatingWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddEventSystemToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserInterests",
                schema: "event",
                newName: "UserInterests",
                newSchema: "post");

            migrationBuilder.RenameTable(
                name: "MissedEventsHistory",
                schema: "event",
                newName: "MissedEventsHistory",
                newSchema: "post");

            migrationBuilder.RenameTable(
                name: "Interests",
                schema: "event",
                newName: "Interests",
                newSchema: "post");

            migrationBuilder.RenameTable(
                name: "Events",
                schema: "event",
                newName: "Events",
                newSchema: "post");

            migrationBuilder.RenameTable(
                name: "EventParticipants",
                schema: "event",
                newName: "EventParticipants",
                newSchema: "post");

            migrationBuilder.RenameTable(
                name: "CreditTransactions",
                schema: "event",
                newName: "CreditTransactions",
                newSchema: "post");

            migrationBuilder.RenameTable(
                name: "CreditProducts",
                schema: "event",
                newName: "CreditProducts",
                newSchema: "post");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "event");

            migrationBuilder.RenameTable(
                name: "UserInterests",
                schema: "post",
                newName: "UserInterests",
                newSchema: "event");

            migrationBuilder.RenameTable(
                name: "MissedEventsHistory",
                schema: "post",
                newName: "MissedEventsHistory",
                newSchema: "event");

            migrationBuilder.RenameTable(
                name: "Interests",
                schema: "post",
                newName: "Interests",
                newSchema: "event");

            migrationBuilder.RenameTable(
                name: "Events",
                schema: "post",
                newName: "Events",
                newSchema: "event");

            migrationBuilder.RenameTable(
                name: "EventParticipants",
                schema: "post",
                newName: "EventParticipants",
                newSchema: "event");

            migrationBuilder.RenameTable(
                name: "CreditTransactions",
                schema: "post",
                newName: "CreditTransactions",
                newSchema: "event");

            migrationBuilder.RenameTable(
                name: "CreditProducts",
                schema: "post",
                newName: "CreditProducts",
                newSchema: "event");
        }
    }
}
