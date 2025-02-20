using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillSwap.EntitiesConfiguration.Migrations
{
    /// <inheritdoc />
    public partial class AmountOnSessionsTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Sessions",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Sessions");
        }
    }
}
