using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class addoperationrefandsusuuidcolintosubstatustable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<string>(
                name: "Operation_reference",
                table: "Subscription_Status",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subscription_uuid",
                table: "Subscription_Status",
                type: "nvarchar(max)",
                nullable: true);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Operation_reference",
                table: "Subscription_Status");

            migrationBuilder.DropColumn(
                name: "Subscription_uuid",
                table: "Subscription_Status");

            
        }
    }
}
