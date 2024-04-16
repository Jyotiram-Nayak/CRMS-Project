using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMS_Project.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PlacementApplications_CompanyId",
                table: "PlacementApplications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PlacementApplications_UniversityId",
                table: "PlacementApplications",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CompanyId",
                table: "JobPostings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_UniversityId",
                table: "JobPostings",
                column: "UniversityId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_AspNetUsers_CompanyId",
                table: "JobPostings",
                column: "CompanyId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_AspNetUsers_UniversityId",
                table: "JobPostings",
                column: "UniversityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlacementApplications_AspNetUsers_CompanyId",
                table: "PlacementApplications",
                column: "CompanyId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlacementApplications_AspNetUsers_UniversityId",
                table: "PlacementApplications",
                column: "UniversityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_AspNetUsers_CompanyId",
                table: "JobPostings");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_AspNetUsers_UniversityId",
                table: "JobPostings");

            migrationBuilder.DropForeignKey(
                name: "FK_PlacementApplications_AspNetUsers_CompanyId",
                table: "PlacementApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_PlacementApplications_AspNetUsers_UniversityId",
                table: "PlacementApplications");

            migrationBuilder.DropIndex(
                name: "IX_PlacementApplications_CompanyId",
                table: "PlacementApplications");

            migrationBuilder.DropIndex(
                name: "IX_PlacementApplications_UniversityId",
                table: "PlacementApplications");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_CompanyId",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_UniversityId",
                table: "JobPostings");
        }
    }
}
