using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Smarket.Models;

namespace Smarket.DataAccess;

public class ApplicationDbContext : IdentityDbContext<User, Role, int,
    IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Role>().ToTable("Roles");
        builder.Entity<User>().ToTable("Users");
        builder.Entity<UserRole>().ToTable("UserRole");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

        builder.Entity<User>()
               .HasMany(ur => ur.UserRoles)
               .WithOne(u => u.User)
               .HasForeignKey(ur => ur.UserId)
               .IsRequired();

        builder.Entity<Role>()
            .HasMany(ur => ur.UsersRole)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();
        builder.Entity<Product>()
           .HasOne(x => x.Image)
           .WithMany()
           .HasForeignKey(x => x.ImageId)
           .OnDelete(DeleteBehavior.NoAction);
/*        builder.Entity<Package>()
           .HasOne(x => x.Inventory)
           .WithMany()
           .HasForeignKey(x => x.InventoryId)
           .OnDelete(DeleteBehavior.NoAction);*/
        builder.Entity<Package>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Product>()
                    .HasOne(p => p.Brand)
                    .WithMany(b => b.Products)
                    .OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Product>()
           .HasOne(x => x.SubCategory)
           .WithMany(subcategory => subcategory.Products)
           .HasForeignKey(x => x.SubCategoryId)
           .OnDelete(DeleteBehavior.NoAction);

    }

    public DbSet<Brand> Brands { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<SubCategory> SubCategories { get; set; }
    public DbSet<UserReview> UserReviews { get; set; }



   /* public void SeedData()
    {
        if (!Database.CanConnect())
        {
            Console.WriteLine("Database connection is not available.");
            return;
        }

*//*        if (Brands.Any())
        {
            Console.WriteLine("Database already seeded.");
            return;
        }*//*

        try
        {
            var jsonData = File.ReadAllText("D:\\DataSeed\\Brand.json");
            var data = JsonConvert.DeserializeObject<List<Brand>>(jsonData);

            // Add data to DbSet and save changes
            Brands.AddRange(data); // Adjust this line based on the DbSet you want to seed

            SaveChanges();

            Console.WriteLine("Database seeded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding database: {ex.Message}");
        }
    }*/
}