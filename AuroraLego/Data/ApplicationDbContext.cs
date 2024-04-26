using INTEXGroup3_7.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace INTEXGroup3_7.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<top_product> top_products { get; set; }
    
    public DbSet<ItemRecommendation> ItemRecommendations { get; set; }
    
    public DbSet<UserRecommendations> UserRecommendations { get; set; }
    
    public DbSet<LineItem> LineItems { get; set; }
    public DbSet<OrderPrediction> OrderPredictions { get; set; }
}