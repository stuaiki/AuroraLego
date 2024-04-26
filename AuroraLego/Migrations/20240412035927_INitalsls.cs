using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INTEXGroup3_7.Migrations
{
    /// <inheritdoc />
    public partial class INitalsls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderPredictions",
                columns: table => new
                {
                    predictionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ordertransaction_ID = table.Column<int>(type: "int", nullable: false),
                    prediction = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPredictions", x => x.predictionId);
                    table.ForeignKey(
                        name: "FK_OrderPredictions_Orders_ordertransaction_ID",
                        column: x => x.ordertransaction_ID,
                        principalTable: "Orders",
                        principalColumn: "transaction_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPredictions_ordertransaction_ID",
                table: "OrderPredictions",
                column: "ordertransaction_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderPredictions");
        }
    }
}
