using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmusmentPlanningSystem.Data.Migrations
{
    public partial class Clientuserinheritanceandnavigationproperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Service",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Clients",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientUserId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Clients_ClientUserId",
                        column: x => x.ClientUserId,
                        principalTable: "Clients",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviders",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviders", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_ServiceProviders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Company_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Service_CompanyId",
                table: "Service",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ClientUserId",
                table: "Comment",
                column: "ClientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ServiceId",
                table: "Comment",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_ServiceProviderId",
                table: "Company",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Users_UserId",
                table: "Clients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Company_CompanyId",
                table: "Service",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Users_UserId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_Company_CompanyId",
                table: "Service");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "ServiceProviders");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Service_CompanyId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Service");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Clients",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
