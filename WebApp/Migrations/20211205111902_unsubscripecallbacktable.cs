using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class unsubscripecallbacktable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.CreateTable(
                name: "UnSubscripeCallBackHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subscription_uuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    charging_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    merchant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    operation_reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    subscription_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    billing_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    service_starts_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    service_ends_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    consumer_identity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timestamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    unsubscribe_url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnSubscripeCallBackHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnSubscripeCallBackHistory");

            
        }
    }
}
