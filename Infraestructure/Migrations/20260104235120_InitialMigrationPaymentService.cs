using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationPaymentService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    QrCodeUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientSecretHash = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAccounts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ServiceAccounts",
                columns: new[] { "Id", "ClientId", "ClientSecretHash", "CreatedAt", "IsActive", "ServiceName" },
                values: new object[] { new Guid("911c65ea-54f6-4310-a739-82914326e53c"), new Guid("17036aa9-79d8-4fb7-9218-741d316c003f"), "$2a$11$6mcP.1X3HO9WzBv/boKC2uJOGhR6UlbyGtrTeS8sZoAJEr7vOqzjq", new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "OrderService" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAccounts_ServiceName",
                table: "ServiceAccounts",
                column: "ServiceName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ServiceAccounts");
        }
    }
}
