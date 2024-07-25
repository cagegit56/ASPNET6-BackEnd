using Authorization_and_Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Authorization_and_Authentication.Auth
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<StockModel> Stock { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Employee>().ToTable("Employee");
            builder.Entity<ProductImage>().ToTable("ProductImage");
            builder.Entity<StockModel>().ToTable("Stock").Property(x => x.ImgData)
            .HasColumnType("varbinary(MAX)");
            
        }

        

    }
}
