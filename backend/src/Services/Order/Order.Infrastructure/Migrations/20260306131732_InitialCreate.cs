using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ShippingAddress_FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShippingAddress_LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShippingAddress_EmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ShippingAddress_AddressLine = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ShippingAddress_Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShippingAddress_State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShippingAddress_ZipCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PaymentInfo_CardName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PaymentInfo_CardNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PaymentInfo_Expiration = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PaymentInfo_Cvv = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    PaymentInfo_PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
