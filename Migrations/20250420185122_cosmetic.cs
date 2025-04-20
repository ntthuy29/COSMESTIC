using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class cosmetic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Catalog",
                columns: table => new
                {
                    catalogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "40000, 1"),
                    catalogName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    catalogDescription = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog", x => x.catalogID);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    discountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "80000, 1"),
                    value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    discountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.discountID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    userID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
                    fullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.userID);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    productID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "30000, 1"),
                    productName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    productDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    imagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    catalogID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.productID);
                    table.ForeignKey(
                        name: "FK_Product_Catalog_catalogID",
                        column: x => x.catalogID,
                        principalTable: "Catalog",
                        principalColumn: "catalogID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    accountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "20000, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.accountID);
                    table.ForeignKey(
                        name: "FK_Account_User_userID",
                        column: x => x.userID,
                        principalTable: "User",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryIFMT",
                columns: table => new
                {
                    deliveryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "300, 1"),
                    deliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deliveryPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deliveryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryIFMT", x => x.deliveryID);
                    table.ForeignKey(
                        name: "FK_DeliveryIFMT_User_userID",
                        column: x => x.userID,
                        principalTable: "User",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    orderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "60000, 1"),
                    userID = table.Column<int>(type: "int", nullable: false),
                    totalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    orderDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    discountID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.orderID);
                    table.ForeignKey(
                        name: "FK_Order_Discount_discountID",
                        column: x => x.discountID,
                        principalTable: "Discount",
                        principalColumn: "discountID");
                    table.ForeignKey(
                        name: "FK_Order_User_userID",
                        column: x => x.userID,
                        principalTable: "User",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    cartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "5000, 1"),
                    userID = table.Column<int>(type: "int", nullable: false),
                    totalQuantity = table.Column<int>(type: "int", nullable: false),
                    totalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => x.cartID);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_User_userID",
                        column: x => x.userID,
                        principalTable: "User",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountProduct",
                columns: table => new
                {
                    discountProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "9000, 1"),
                    discountID = table.Column<int>(type: "int", nullable: false),
                    productID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountProduct", x => x.discountProductID);
                    table.ForeignKey(
                        name: "FK_DiscountProduct_Discount_discountID",
                        column: x => x.discountID,
                        principalTable: "Discount",
                        principalColumn: "discountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountProduct_Product_productID",
                        column: x => x.productID,
                        principalTable: "Product",
                        principalColumn: "productID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    invoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "70000, 1"),
                    orderID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.invoiceID);
                    table.ForeignKey(
                        name: "FK_Invoice_Order_orderID",
                        column: x => x.orderID,
                        principalTable: "Order",
                        principalColumn: "orderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    orderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "200, 1"),
                    orderID = table.Column<int>(type: "int", nullable: false),
                    productID = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.orderDetailID);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_orderID",
                        column: x => x.orderID,
                        principalTable: "Order",
                        principalColumn: "orderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Product_productID",
                        column: x => x.productID,
                        principalTable: "Product",
                        principalColumn: "productID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Revenue",
                columns: table => new
                {
                    revenueID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "10000, 1"),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    totalRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    totalProduct = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    orderID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Revenue", x => x.revenueID);
                    table.ForeignKey(
                        name: "FK_Revenue_Order_orderID",
                        column: x => x.orderID,
                        principalTable: "Order",
                        principalColumn: "orderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItem",
                columns: table => new
                {
                    cartItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "100, 1"),
                    cartID = table.Column<int>(type: "int", nullable: false),
                    productID = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unitprice = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => x.cartItemID);
                    table.ForeignKey(
                        name: "FK_CartItem_Product_productID",
                        column: x => x.productID,
                        principalTable: "Product",
                        principalColumn: "productID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItem_ShoppingCart_cartID",
                        column: x => x.cartID,
                        principalTable: "ShoppingCart",
                        principalColumn: "cartID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogRevenue",
                columns: table => new
                {
                    catalogRevenueID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "500, 1"),
                    revenueID = table.Column<int>(type: "int", nullable: false),
                    catalogID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogRevenue", x => x.catalogRevenueID);
                    table.ForeignKey(
                        name: "FK_CatalogRevenue_Catalog_catalogID",
                        column: x => x.catalogID,
                        principalTable: "Catalog",
                        principalColumn: "catalogID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogRevenue_Revenue_revenueID",
                        column: x => x.revenueID,
                        principalTable: "Revenue",
                        principalColumn: "revenueID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_userID",
                table: "Account",
                column: "userID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_cartID",
                table: "CartItem",
                column: "cartID");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_productID",
                table: "CartItem",
                column: "productID");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogRevenue_catalogID",
                table: "CatalogRevenue",
                column: "catalogID");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogRevenue_revenueID",
                table: "CatalogRevenue",
                column: "revenueID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryIFMT_userID",
                table: "DeliveryIFMT",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountProduct_discountID",
                table: "DiscountProduct",
                column: "discountID");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountProduct_productID",
                table: "DiscountProduct",
                column: "productID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_orderID",
                table: "Invoice",
                column: "orderID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_discountID",
                table: "Order",
                column: "discountID");

            migrationBuilder.CreateIndex(
                name: "IX_Order_userID",
                table: "Order",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_orderID",
                table: "OrderDetail",
                column: "orderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_productID",
                table: "OrderDetail",
                column: "productID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_catalogID",
                table: "Product",
                column: "catalogID");

            migrationBuilder.CreateIndex(
                name: "IX_Revenue_orderID",
                table: "Revenue",
                column: "orderID");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_userID",
                table: "ShoppingCart",
                column: "userID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "CartItem");

            migrationBuilder.DropTable(
                name: "CatalogRevenue");

            migrationBuilder.DropTable(
                name: "DeliveryIFMT");

            migrationBuilder.DropTable(
                name: "DiscountProduct");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.DropTable(
                name: "Revenue");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Catalog");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
