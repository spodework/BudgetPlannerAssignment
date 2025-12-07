using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpensesAndStuff.Migrations
{
    /// <inheritdoc />
    public partial class AddModelAbsence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpensePayMonth",
                table: "Expenses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpensePayMonth",
                table: "Expenses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
