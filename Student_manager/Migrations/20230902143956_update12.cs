using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_manager.Migrations
{
    public partial class update12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentName",
                table: "SubjectInfos",
                newName: "Studentid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Studentid",
                table: "SubjectInfos",
                newName: "StudentName");
        }
    }
}
