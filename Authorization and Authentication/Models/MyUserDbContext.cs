using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Authorization_and_Authentication.Models;

public partial class MyUserDbContext : DbContext
{
    public MyUserDbContext()
    {
    }

    public MyUserDbContext(DbContextOptions<MyUserDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-HK5AN4U;Database=MyUserDB;Integrated Security=True;TrustServerCertificate=True;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("ProductImage");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ImgData).HasColumnType("image");
            entity.Property(e => e.ProdName)
                .HasMaxLength(40)
                .IsFixedLength();
            entity.Property(e => e.ProdPrice)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
