using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoalTracker.Migrations
{
    public partial class GoalTasksMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "GoalTasks",
                table => new
                {
                    Id = table.Column<int>("INTEGER", false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoalId = table.Column<int>("INTEGER", nullable: true),
                    Title = table.Column<string>("TEXT", true),
                    Notes = table.Column<string>("TEXT", true),
                    Finished = table.Column<bool>("INTEGER", false),
                    CreateDate = table.Column<DateTime>("TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>("TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalTasks", x => x.Id);
                    table.ForeignKey("FK_GoalTasks_Goals_GoalId",
                        x => x.GoalId,
                        "Goals",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_GoalTasks_GoalId",
                "GoalTasks",
                "GoalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("GoalTasks");
        }
    }
}