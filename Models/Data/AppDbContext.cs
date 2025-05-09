using Microsoft.EntityFrameworkCore;

namespace COSMESTIC.Models.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet <Account> Accounts { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Catalogs> Catalogs { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet <orderDetail> orderDetails { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<DeliveryIFMT> DeliveryIFMT { get; set; }
        public DbSet<Revenue> Revenue { get; set; }
        public DbSet<Discount> Discount
        {
            get; set;

        }
        public DbSet<ProductReView> ProductReView { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(u => u.userID)
                       .UseIdentityColumn(1000, 1);
                entity.Property(u => u.role).IsRequired();
                entity.Property(u => u.fullName).IsRequired();
            }
            );

            modelBuilder.Entity<Account>(
                entity =>
                {
                    entity.Property(a => a.accountID)
                        .UseIdentityColumn(20000, 1);
                    entity.Property(a => a.username).IsRequired();
                    entity.Property(a => a.password).IsRequired();
                    entity.HasOne(a => a.user)
                        .WithOne(u => u.account)
                        .HasForeignKey<Account>(a => a.userID).IsRequired();

                });
            modelBuilder.Entity<Orders>(entity =>
            {
                // Ánh xạ bảng 'Orders' trong Entity Framework với tên bảng 'Order' trong cơ sở dữ liệu
                entity.ToTable("Order");

                // Các cấu hình khác của bảng Orders
                entity.Property(o => o.orderID)
                    .UseIdentityColumn(60000, 1);
                entity.Property(o => o.orderDate).HasDefaultValueSql("GETDATE()").IsRequired();
                entity.HasOne(o => o.users)
                    .WithMany(u => u.orders)
                    .HasForeignKey(o => o.userID).IsRequired();
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.Property(p => p.productID)
                    .UseIdentityColumn(30000, 1);
                entity.Property(p => p.productName).IsRequired();
                entity.Property(p => p.price).IsRequired();
                entity.Property(p => p.imagePath).IsRequired();
                entity.HasOne(p => p.catalog)
                    .WithMany(c => c.products)
                    .HasForeignKey(p => p.catalogID).IsRequired();
            });
            modelBuilder.Entity<Catalogs>(entity =>
            {
                entity.Property(c => c.catalogID)
                    .UseIdentityColumn(40000, 1);
                entity.Property(c => c.catalogName).IsRequired();

            });
            modelBuilder.Entity<ShoppingCart>(entity => {
                entity.Property(sc => sc.cartID)
                .UseIdentityColumn(5000, 1);
                entity.HasOne(sc => sc.users)
                .WithOne(u => u.ShoppingCart)
                .HasForeignKey<ShoppingCart>(sc => sc.userID).IsRequired();
            });
            modelBuilder.Entity<CartItem>(entity => { 
            entity.Property(c => c.cartItemID).UseIdentityColumn(100, 1);
                entity.HasOne(c => c.cart)
                    .WithMany(sc => sc.cartItems)
                    .HasForeignKey(c => c.cartID).IsRequired();
                entity.HasOne(c => c.products)
                    .WithMany(p => p.cartItems)
                    .HasForeignKey(c => c.productID).IsRequired();

            }
                );

            modelBuilder.Entity<orderDetail>(entity=>
            {
                entity.Property(od => od.orderDetailID).UseIdentityColumn(200, 1);
                entity.HasOne(od => od.orders)
                    .WithMany(o => o.orderDetails)
                    .HasForeignKey(od => od.orderID).IsRequired();
                entity.HasOne(od => od.products)
                    .WithMany(p => p.orderDetails)
                    .HasForeignKey(od => od.productID).IsRequired();
            });
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(i => i.invoiceID)
                    .UseIdentityColumn(70000, 1);
                entity.HasOne(i => i.orders)
                    .WithOne(o => o.invoice)
                    .HasForeignKey<Invoice>(i => i.orderID).IsRequired();
            });
            modelBuilder.Entity<DeliveryIFMT>(entity =>
            {
                entity.Property(entity => entity.deliveryID)
                    .UseIdentityColumn(300, 1);
                entity.Property(d=>d.deliveryAddress).IsRequired();
                entity.Property(de=>de.deliveryName).IsRequired();
                entity.Property(d => d.deliveryPhone).IsRequired();
                entity.HasOne(d => d.user)
                    .WithMany(u => u.deliverys)
                    .HasForeignKey(d => d.userID).IsRequired();
            });
            modelBuilder.Entity<Revenue>(entity =>
            {
                entity.Property(r => r.revenueID)
                    .UseIdentityColumn(10000, 1);
              entity.HasOne(entity => entity.orders)
                    .WithMany(cr => cr.revenues)
                    .HasForeignKey(entity => entity.orderID).IsRequired();
            });
            modelBuilder.Entity<Discount>(entity =>
            {
                entity.Property(d => d.discountID)
                    .UseIdentityColumn(80000, 1);

            });
        }


    }
}
          