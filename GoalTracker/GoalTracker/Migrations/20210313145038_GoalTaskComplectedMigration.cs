using Microsoft.EntityFrameworkCore.Migrations;

namespace GoalTracker.Migrations
{
    public partial class GoalTaskComplectedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                "AllTasksCompleted",
                "Goals",
                "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "AllTasksCompleted",
                "Goals");
        }
    }
}