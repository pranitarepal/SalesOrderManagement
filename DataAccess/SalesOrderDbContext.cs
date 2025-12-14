using Microsoft.EntityFrameworkCore;
using SalesOrderManagement.Models.Entities;

namespace SalesOrderManagement.DataAccess
{
    public class SalesOrderDbContext : DbContext
    {
        public SalesOrderDbContext(DbContextOptions<SalesOrderDbContext> options) : base(options) { }

        public DbSet<PurchaseOrder> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ItemMaster> ItemMasters { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(po => po.OrderItems)
                .WithOne(li => li.Order)
                .HasForeignKey(li => li.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Explicit configuration to handle database schema
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderLineItems");
                entity.Property(e => e.ItemId).HasColumnName("ItemId");
                entity.Property(e => e.OrderId).HasColumnName("OrderId");
                entity.Property(e => e.ProductName).HasColumnName("ProductName").IsRequired(false);
                entity.Property(e => e.Manufacturer).HasColumnName("Manufacturer").IsRequired(false);
                entity.Property(e => e.Price).HasColumnName("Price").IsRequired(false);
                entity.Property(e => e.Qty).HasColumnName("Qty").IsRequired(false);
                entity.Property(e => e.Subtotal).HasColumnName("Subtotal").IsRequired(false);
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.ToTable("Orders");
                entity.Property(e => e.OrderId).HasColumnName("OrderId");
                entity.Property(e => e.UserId).HasColumnName("UserId").IsRequired(false);
                entity.Property(e => e.TotalAmount).HasColumnName("TotalAmount").IsRequired(false);
                entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired(false);
                entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted").IsRequired(false);
                entity.Property(e => e.OrderStatus).HasColumnName("OrderStatus").IsRequired(false);
                entity.Property(e => e.Notes).HasColumnName("Notes").IsRequired(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.FirstName).HasColumnName("FirstName").IsRequired(false);
                entity.Property(e => e.LastName).HasColumnName("LastName").IsRequired(false);
                entity.Property(e => e.Email).HasColumnName("Email").IsRequired(false);
                entity.Property(e => e.ContactNo).HasColumnName("ContactNo").IsRequired(false);
                entity.Property(e => e.Address).HasColumnName("Address").IsRequired(false);
                entity.Property(e => e.ZipCode).HasColumnName("ZipCode").IsRequired(false);
                entity.Property(e => e.UserType).HasColumnName("UserType").IsRequired(false);
            });
        }
    }
}
