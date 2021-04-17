using Microsoft.EntityFrameworkCore.Migrations;

namespace GoalTracker.Migrations
{
    public partial class GoalImageMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "DetailImage",
                "Goals",
                "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "DetailImage",
                "Goals");
        }
    }
}