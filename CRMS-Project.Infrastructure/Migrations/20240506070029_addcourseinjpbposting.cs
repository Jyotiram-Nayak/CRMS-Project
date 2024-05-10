using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMS_Project.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addcourseinjpbposting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Courses",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Courses",
                table: "JobPostings");
        }
    }
}
