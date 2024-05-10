using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMS_Project.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addisselectedtostudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "isSelected",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isSelected",
                table: "Students");
        }
    }
}
