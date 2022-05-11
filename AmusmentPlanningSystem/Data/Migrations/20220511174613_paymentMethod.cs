using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmusmentPlanningSystem.Data.Migrations
{
    public partial class paymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MethodOfPayment",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MethodOfPayment",
                table: "Orders");
        }
    }
}
