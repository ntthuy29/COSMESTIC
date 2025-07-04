﻿// <auto-generated />
using System;
using COSMESTIC.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace COSMESTIC.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250602012655_AddNoteToOrders")]
    partial class AddNoteToOrders
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("COSMESTIC.Models.Data.Account", b =>
                {
                    b.Property<int>("accountID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("accountID"), 20000L);

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("userID")
                        .HasColumnType("int");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("accountID");

                    b.HasIndex("userID")
                        .IsUnique();

                    b.ToTable("Account");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.CartItem", b =>
                {
                    b.Property<int>("cartItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("cartItemID"), 100L);

                    b.Property<int>("cartID")
                        .HasColumnType("int");

                    b.Property<int>("productID")
                        .HasColumnType("int");

                    b.Property<int>("quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("unitprice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("cartItemID");

                    b.HasIndex("cartID");

                    b.HasIndex("productID");

                    b.ToTable("CartItem");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Catalogs", b =>
                {
                    b.Property<int>("catalogID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("catalogID"), 40000L);

                    b.Property<string>("catalogDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("catalogName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("catalogID");

                    b.ToTable("Catalog");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.DeliveryIFMT", b =>
                {
                    b.Property<int>("deliveryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("deliveryID"), 300L);

                    b.Property<string>("deliveryAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("deliveryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("deliveryPhone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("userID")
                        .HasColumnType("int");

                    b.HasKey("deliveryID");

                    b.HasIndex("userID");

                    b.ToTable("DeliveryIFMT");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Discount", b =>
                {
                    b.Property<int>("discountID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("discountID"), 80000L);

                    b.Property<decimal>("discountAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("discountType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("endDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("startDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("value")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("discountID");

                    b.ToTable("Discount");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Invoice", b =>
                {
                    b.Property<int>("invoiceID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("invoiceID"), 70000L);

                    b.Property<int>("orderID")
                        .HasColumnType("int");

                    b.HasKey("invoiceID");

                    b.HasIndex("orderID")
                        .IsUnique();

                    b.ToTable("Invoice");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Orders", b =>
                {
                    b.Property<int>("orderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("orderID"), 60000L);

                    b.Property<int?>("DeliveryID")
                        .HasColumnType("int");

                    b.Property<int?>("discountID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("endDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("note")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("orderDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("payMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("totalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("totalItems")
                        .HasColumnType("int");

                    b.Property<int>("userID")
                        .HasColumnType("int");

                    b.HasKey("orderID");

                    b.HasIndex("DeliveryID");

                    b.HasIndex("discountID");

                    b.HasIndex("userID");

                    b.ToTable("Order", (string)null);
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.ProductReView", b =>
                {
                    b.Property<int>("reviewID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("reviewID"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("comment")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("orderID")
                        .HasColumnType("int");

                    b.Property<int>("productID")
                        .HasColumnType("int");

                    b.Property<int>("rating")
                        .HasColumnType("int");

                    b.Property<int>("userID")
                        .HasColumnType("int");

                    b.HasKey("reviewID");

                    b.HasIndex("orderID");

                    b.HasIndex("productID");

                    b.HasIndex("userID");

                    b.ToTable("ProductReView");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Products", b =>
                {
                    b.Property<int>("productID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("productID"), 30000L);

                    b.Property<int>("catalogID")
                        .HasColumnType("int");

                    b.Property<string>("imagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("productDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("productName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("quantity")
                        .HasColumnType("int");

                    b.HasKey("productID");

                    b.HasIndex("catalogID");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Revenue", b =>
                {
                    b.Property<int>("revenueID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("revenueID"), 10000L);

                    b.Property<DateTime>("endDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("note")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("orderID")
                        .HasColumnType("int");

                    b.Property<DateTime>("startDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("totalProduct")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("totalRevenue")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("revenueID");

                    b.HasIndex("orderID");

                    b.ToTable("Revenue");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.ShoppingCart", b =>
                {
                    b.Property<int>("cartID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("cartID"), 5000L);

                    b.Property<decimal>("totalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("totalQuantity")
                        .HasColumnType("int");

                    b.Property<int>("userID")
                        .HasColumnType("int");

                    b.HasKey("cartID");

                    b.HasIndex("userID")
                        .IsUnique();

                    b.ToTable("ShoppingCart");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Users", b =>
                {
                    b.Property<int>("userID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("userID"), 1000L);

                    b.Property<decimal>("TotalSpent")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("createdDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("dateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("userID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.orderDetail", b =>
                {
                    b.Property<int>("orderDetailID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("orderDetailID"), 200L);

                    b.Property<int>("orderID")
                        .HasColumnType("int");

                    b.Property<int>("productID")
                        .HasColumnType("int");

                    b.Property<int>("quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("unitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("orderDetailID");

                    b.HasIndex("orderID");

                    b.HasIndex("productID");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Account", b =>
                {
                    b.HasOne("COSMESTIC.Models.Data.Users", "user")
                        .WithOne("account")
                        .HasForeignKey("COSMESTIC.Models.Data.Account", "userID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.CartItem", b =>
                {
                    b.HasOne("COSMESTIC.Models.Data.ShoppingCart", "cart")
                        .WithMany("cartItems")
                        .HasForeignKey("cartID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("COSMESTIC.Models.Data.Products", "products")
                        .WithMany("cartItems")
                        .HasForeignKey("productID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("cart");

                    b.Navigation("products");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.DeliveryIFMT", b =>
                {
                    b.HasOne("COSMESTIC.Models.Data.Users", "user")
                        .WithMany("deliverys")
                        .HasForeignKey("userID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Invoice", b =>
                {
                    b.HasOne("COSMESTIC.Models.Data.Orders", "orders")
                        .WithOne("invoice")
                        .HasForeignKey("COSMESTIC.Models.Data.Invoice", "orderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("orders");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Orders", b =>
                {
                    b.HasOne("COSMESTIC.Models.Data.DeliveryIFMT", "Delivery")
                        .WithMany()
                        .HasForeignKey("DeliveryID");

                    b.HasOne("COSMESTIC.Models.Data.Discount", null)
                        .WithMany("Orders")
                        .HasForeignKey("discountID");

                    b.HasOne("COSMESTIC.Models.Data.Users", "users")
                        .WithMany("orders")
                        .HasForeignKey("userID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Delivery");

                    b.Navigation("users");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.ProductReView", b =>
                {
                    b.HasOne("COSMESTIC.Models.Data.Orders", "order")
                        .WithMany("ProductReView")
                        .HasForeignKey("orderID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("COSMESTIC.Models.Data.Products", "product")
                        .WithMany("ProductReviews")
                        .HasForeignKey("productID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("COSMESTIC.Models.Data.Users", "user")
                        .WithMany("ProductReviews")
                        .HasForeignKey("userID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("order");

                    b.Navigation("product");

                    b.Navigation("user");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Products", b =>
                {
                    b.HasOne("COSMESTIC.Models.Data.Catalogs", "catalog")
                        .WithMany("products")
                        .HasForeignKey("catalogID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("catalog");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Revenue", b =>
                {
                    b.HasOne("COSMESTIC.Models.Data.Orders", "orders")
                        .WithMany("revenues")
                        .HasForeignKey("orderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("orders");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.ShoppingCart", b =>
                {
                    b.HasOne("COSMESTIC.Models.Data.Users", "users")
                        .WithOne("ShoppingCart")
                        .HasForeignKey("COSMESTIC.Models.Data.ShoppingCart", "userID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("users");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.orderDetail", b =>
                {
                    b.HasOne("COSMESTIC.Models.Data.Orders", "orders")
                        .WithMany("orderDetails")
                        .HasForeignKey("orderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("COSMESTIC.Models.Data.Products", "products")
                        .WithMany("orderDetails")
                        .HasForeignKey("productID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("orders");

                    b.Navigation("products");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Catalogs", b =>
                {
                    b.Navigation("products");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Discount", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Orders", b =>
                {
                    b.Navigation("ProductReView");

                    b.Navigation("invoice")
                        .IsRequired();

                    b.Navigation("orderDetails");

                    b.Navigation("revenues");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Products", b =>
                {
                    b.Navigation("ProductReviews");

                    b.Navigation("cartItems");

                    b.Navigation("orderDetails");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.ShoppingCart", b =>
                {
                    b.Navigation("cartItems");
                });

            modelBuilder.Entity("COSMESTIC.Models.Data.Users", b =>
                {
                    b.Navigation("ProductReviews");

                    b.Navigation("ShoppingCart")
                        .IsRequired();

                    b.Navigation("account")
                        .IsRequired();

                    b.Navigation("deliverys");

                    b.Navigation("orders");
                });
#pragma warning restore 612, 618
        }
    }
}
