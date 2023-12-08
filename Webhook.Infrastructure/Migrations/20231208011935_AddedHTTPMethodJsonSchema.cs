using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webhook.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedHTTPMethodJsonSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JsonSchema",
                table: "Webhooks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonSchema",
                table: "Webhooks");
        }
    }
}
