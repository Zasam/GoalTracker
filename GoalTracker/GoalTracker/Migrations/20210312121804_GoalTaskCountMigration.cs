using Microsoft.EntityFrameworkCore.Migrations;

namespace GoalTracker.Migrations
{
    public partial class GoalTaskCountMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                "GoalTaskCount",
                "Goals",
                "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "GoalTaskCount",
                "Goals");
        }
    }
}