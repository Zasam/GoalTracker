using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoalTracker.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Goals",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>("TEXT", nullable: true),
                    Notes = table.Column<string>("TEXT", nullable: true),
                    StartDate = table.Column<DateTime>("TEXT", nullable: false),
                    HasDueDate = table.Column<bool>("INTEGER", nullable: false),
                    EndDate = table.Column<DateTime>("TEXT", nullable: false),
                    GoalAppointmentInterval = table.Column<int>("INTEGER", nullable: false),
                    NotificationTime = table.Column<TimeSpan>("TEXT", nullable: false),
                    NotificationId = table.Column<int>("INTEGER", nullable: false),
                    RequestCode = table.Column<int>("INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>("TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>("TEXT", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Goals", x => x.Id); });

            migrationBuilder.CreateTable(
                "Users",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>("TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>("TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>("TEXT", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Users", x => x.Id); });

            migrationBuilder.CreateTable(
                "GoalAppointments",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoalId = table.Column<int>("INTEGER", nullable: true),
                    ApprovalDate = table.Column<DateTime>("TEXT", nullable: true),
                    AppointmentDate = table.Column<DateTime>("TEXT", nullable: false),
                    Approved = table.Column<bool>("INTEGER", nullable: false),
                    Success = table.Column<bool>("INTEGER", nullable: true),
                    CreateDate = table.Column<DateTime>("TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>("TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalAppointments", x => x.Id);
                    table.ForeignKey(
                        "FK_GoalAppointments_Goals_GoalId",
                        x => x.GoalId,
                        "Goals",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Achievements",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>("TEXT", nullable: true),
                    Description = table.Column<string>("TEXT", nullable: true),
                    Experience = table.Column<int>("INTEGER", nullable: false),
                    InternalTag = table.Column<string>("TEXT", nullable: true),
                    Unlocked = table.Column<bool>("INTEGER", nullable: false),
                    UserId = table.Column<int>("INTEGER", nullable: true),
                    CreateDate = table.Column<DateTime>("TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>("TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                    table.ForeignKey(
                        "FK_Achievements_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_Achievements_UserId",
                "Achievements",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_GoalAppointments_GoalId",
                "GoalAppointments",
                "GoalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Achievements");

            migrationBuilder.DropTable(
                "GoalAppointments");

            migrationBuilder.DropTable(
                "Users");

            migrationBuilder.DropTable(
                "Goals");
        }
    }
}