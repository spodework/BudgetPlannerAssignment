using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpensesAndStuff.Migrations
{
    /// <inheritdoc />
    public partial class AddPayMonth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpensePayMonth",
                table: "Expenses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpensePayMonth",
                table: "Expenses");
        }
    }
}
