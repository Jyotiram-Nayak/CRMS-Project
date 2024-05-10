using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMS_Project.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeisselectedtoboolean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isSelected",
                table: "Students",
                newName: "IsSelected");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSelected",
                table: "Students",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSelected",
                table: "Students",
                newName: "isSelected");

            migrationBuilder.AlterColumn<int>(
                name: "isSelected",
                table: "Students",
                type: "int",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
