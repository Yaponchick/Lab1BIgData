using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab1BIgData.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePhoneLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guest",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    date_registered = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Guest__3213E83FC5999379", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Hotel",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    star_rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Hotel__3213E83F0CC345B7", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    hire_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    salary = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__3213E83F67EFB1AF", x => x.id);
                    table.ForeignKey(
                        name: "Employee_fk1",
                        column: x => x.hotel_id,
                        principalTable: "Hotel",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guest_id = table.Column<int>(type: "int", nullable: false),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    review_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    review_date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Review__3213E83F93412ED8", x => x.id);
                    table.ForeignKey(
                        name: "Review_fk1",
                        column: x => x.guest_id,
                        principalTable: "Guest",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Review_fk2",
                        column: x => x.hotel_id,
                        principalTable: "Hotel",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    room_number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    room_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    price_per_night = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    is_available = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Room__3213E83F6409CBB1", x => x.id);
                    table.ForeignKey(
                        name: "Room_fk1",
                        column: x => x.hotel_id,
                        principalTable: "Hotel",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Service__3213E83FFB186CF1", x => x.id);
                    table.ForeignKey(
                        name: "Service_fk1",
                        column: x => x.hotel_id,
                        principalTable: "Hotel",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guest_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    check_in_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    check_out_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    total_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Booking__3213E83FC42F6F64", x => x.id);
                    table.ForeignKey(
                        name: "Booking_fk1",
                        column: x => x.guest_id,
                        principalTable: "Guest",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Booking_fk2",
                        column: x => x.room_id,
                        principalTable: "Room",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "RoomCleaning",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    cleaning_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RoomClea__3213E83F8307D60D", x => x.id);
                    table.ForeignKey(
                        name: "RoomCleaning_fk1",
                        column: x => x.room_id,
                        principalTable: "Room",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "RoomCleaning_fk2",
                        column: x => x.employee_id,
                        principalTable: "Employee",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "GuestService",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guest_id = table.Column<int>(type: "int", nullable: false),
                    service_id = table.Column<int>(type: "int", nullable: false),
                    date_used = table.Column<DateTime>(type: "datetime", nullable: false),
                    total_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuestSer__3213E83F75EEBC6A", x => x.id);
                    table.ForeignKey(
                        name: "GuestService_fk1",
                        column: x => x.guest_id,
                        principalTable: "Guest",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "GuestService_fk2",
                        column: x => x.service_id,
                        principalTable: "Service",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guest_id = table.Column<int>(type: "int", nullable: false),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    guest_service_id = table.Column<int>(type: "int", nullable: true),
                    payment_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    payment_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__3213E83F322AB14E", x => x.id);
                    table.ForeignKey(
                        name: "Payment_fk1",
                        column: x => x.guest_id,
                        principalTable: "Guest",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Payment_fk2",
                        column: x => x.booking_id,
                        principalTable: "Booking",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Payment_fk3",
                        column: x => x.guest_service_id,
                        principalTable: "GuestService",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_guest_id",
                table: "Booking",
                column: "guest_id");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_room_id",
                table: "Booking",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Booking__3213E83E3065221F",
                table: "Booking",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_hotel_id",
                table: "Employee",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Employee__3213E83E1DD0F5BD",
                table: "Employee",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Guest__3213E83E6A5AEF92",
                table: "Guest",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Guest__AB6E61641B388C4D",
                table: "Guest",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuestService_guest_id",
                table: "GuestService",
                column: "guest_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuestService_service_id",
                table: "GuestService",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "UQ__GuestSer__3213E83EF16F4C81",
                table: "GuestService",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Hotel__3213E83EBF535E23",
                table: "Hotel",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_booking_id",
                table: "Payment",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_guest_id",
                table: "Payment",
                column: "guest_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_guest_service_id",
                table: "Payment",
                column: "guest_service_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Payment__3213E83E7D55C3B7",
                table: "Payment",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Review_guest_id",
                table: "Review",
                column: "guest_id");

            migrationBuilder.CreateIndex(
                name: "IX_Review_hotel_id",
                table: "Review",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Review__3213E83EBE388CAF",
                table: "Review",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Room_hotel_id",
                table: "Room",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Room__3213E83EBBA95484",
                table: "Room",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomCleaning_employee_id",
                table: "RoomCleaning",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_RoomCleaning_room_id",
                table: "RoomCleaning",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "UQ__RoomClea__3213E83E5D601177",
                table: "RoomCleaning",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Service_hotel_id",
                table: "Service",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Service__3213E83EB3AAF395",
                table: "Service",
                column: "id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "RoomCleaning");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "GuestService");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Guest");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Hotel");
        }
    }
}
