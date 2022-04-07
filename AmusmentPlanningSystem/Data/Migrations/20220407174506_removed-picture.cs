using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmusmentPlanningSystem.Data.Migrations
{
    public partial class removedpicture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Service");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "Service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
