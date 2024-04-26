using INTEXGroup3_7.Data;
using INTEXGroup3_7.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace INTEXGroup3_7.Models;

public class EFAmazonRepository : IAmazonRepository
{
    private ApplicationDbContext _context;

    public EFAmazonRepository(ApplicationDbContext temp)
    {
        _context = temp;
    }

    public IQueryable<Product> Products => _context.Products;
    public IQueryable<Order> Orders => _context.Orders;
    public IQueryable<Customer> Customers => _context.Customers;

    public IQueryable<top_product> TopProducts => _context.top_products;
    
    public IQueryable<UserRecommendations> UserRecommendations => _context.UserRecommendations;

    public IQueryable<ItemRecommendation> ItemRecommendations => _context.ItemRecommendations;

    public IQueryable<OrderPrediction> OrderPredictions => _context.OrderPredictions;

    public IQueryable<IdentityUser> Users => _context.Users.AsQueryable();
    public IQueryable<IdentityRole> Roles => _context.Roles.AsQueryable();

    public IQueryable<IdentityUserRole> UserRoles
    {
        get
        {
            return from user in _context.Users
                join userRole in _context.UserRoles on user.Id equals userRole.UserId
                join role in _context.Roles on userRole.RoleId equals role.Id
                select new IdentityUserRole
                {
                    User = user,
                    Role = role
                };
        }
    }

    public void UpdateOrder(Order order)
    {
        _context.Update(order);
        _context.SaveChanges();
    }

    public void DeleteOrder(Order order)
    {
        _context.Orders.Remove(order);
        _context.SaveChanges();
    }
    
    public void AddOrder(Order order)
    {
        _context.Add(order);
        _context.SaveChanges();
    }

    public void UpdateProductItem(Product product)
    {
        _context.Update(product);
        _context.SaveChanges();
    }

    public void DeleteProductItem(Product product)
    {
        _context.Products.Remove(product);
        _context.SaveChanges();
    }

    public void AddProductItem(Product product)
    {
        _context.Add(product);
        _context.SaveChanges();
    }

    public void AddUserRole(IdentityUserRole user)
    {
        _context.Users.Add(user.User);
        _context.SaveChanges();

        _context.Roles.Add(user.Role);
        _context.SaveChanges();
    }

    public void UpdateUser(IdentityUserRole value)
    {
        _context.Update(value.User);
        _context.SaveChanges();

        _context.Update(value.Role);
        _context.SaveChanges();
    }

    public void DeleteUser(IdentityUserRole value)
    {
        _context.Users.Remove(value.User);
        _context.SaveChanges();

        _context.Roles.Remove(value.Role);
        _context.SaveChanges();
    }

    public void AddPrediction(OrderPrediction prediction)
    {
        _context.OrderPredictions.Add(prediction);
        _context.SaveChanges();
    }

    public void ChangeFraud(int orderId)
    {
        var order = _context.Orders.FirstOrDefault(x => x.transaction_ID == orderId);
        if (order != null)
        {
            order.fraud = true;
            _context.SaveChanges();
        }
    }
}