using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionApi.Model.Migrations
{
    /// <inheritdoc />
    public partial class Timezone_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Transactions");
        }
    }
}
