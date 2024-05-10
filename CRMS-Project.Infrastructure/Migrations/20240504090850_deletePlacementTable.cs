using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMS_Project.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deletePlacementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlacementApplications");

            migrationBuilder.AddColumn<int>(
                name: "Course",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Course",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "PlacementApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniversityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacementApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlacementApplications_AspNetUsers_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlacementApplications_AspNetUsers_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlacementApplications_CompanyId",
                table: "PlacementApplications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PlacementApplications_UniversityId",
                table: "PlacementApplications",
                column: "UniversityId");
        }
    }
}
