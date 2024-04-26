using Microsoft.AspNetCore.Identity;

namespace INTEXGroup3_7.Models.ViewModels;

public class DatabaseModel
{
    public IQueryable<Product> Products { get; set; }
    
    public IQueryable<Order> Orders { get; set; }
    
    public IQueryable<Customer> Customers { get; set; }
    public IQueryable<IdentityUser> Users { get; set; }
    
    public IQueryable<IdentityUserRole> UserRoles { get; set; }
    
    public IdentityUserRole UserRole { get; set; }

    public IdentityUser User { get; set; }

    public IdentityRole Role { get; set; }
    
    public IQueryable<IdentityRole> Roles { get; set; }
}