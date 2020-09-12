using Microsoft.EntityFrameworkCore.Migrations;

namespace UserWebApi.Migrations.Reservation
{
    public partial class RoomTypeColAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumOfRooms",
                table: "Reservations");

            migrationBuilder.AddColumn<string>(
                name: "RoomType",
                table: "Reservations",
                type: "nvarchar(150)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "Reservations");

            migrationBuilder.AddColumn<int>(
                name: "NumOfRooms",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
