using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_manager.Migrations
{
    public partial class update13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxTotalSubject",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxTotalSubject",
                table: "Subjects");
        }
    }
}
