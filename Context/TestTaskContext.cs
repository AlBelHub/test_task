using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using test_task.Models;

namespace test_task.Context
{
    /// <summary>
    /// Контекст БД
    /// </summary>
    public partial class TestTaskContext : DbContext
    {
        public TestTaskContext()
        {
        }

        public TestTaskContext(DbContextOptions<TestTaskContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("Server=ALEKSEY\\AMSSQLSERVER;Database=testTask;User Id=sa;Password=Password!1;TrustServerCertificate=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BAF8DBF8F38");

                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.OrderDate).HasColumnType("datetime");
                entity.Property(e => e.OrderPrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06A187DC4DE2");

                entity.ToTable("OrderItem");

                entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6ED26F5E839");

                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ProductName).HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACD5397FFA");

                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.UserLogin).HasMaxLength(255);
                entity.Property(e => e.UserName).HasMaxLength(255);
            });

            //Преобразование для enum UserRole
            //https://learn.microsoft.com/ru-ru/ef/core/modeling/value-conversions?tabs=data-annotations
            modelBuilder
                .Entity<User>()
                .Property(e => e.UserRole)
                .HasConversion(
                    v => v.ToString(),
                    v => (UserRole)Enum.Parse(typeof(UserRole), v));
            
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}


