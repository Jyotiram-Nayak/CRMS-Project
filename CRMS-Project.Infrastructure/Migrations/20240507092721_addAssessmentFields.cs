using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMS_Project.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAssessmentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AssessmentCompleted",
                table: "JobApplications",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssessmentCompletionDate",
                table: "JobApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssessmentFeedback",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssessmentLink",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssessmentScore",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessmentCompleted",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "AssessmentCompletionDate",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "AssessmentFeedback",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "AssessmentLink",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "AssessmentScore",
                table: "JobApplications");
        }
    }
}
