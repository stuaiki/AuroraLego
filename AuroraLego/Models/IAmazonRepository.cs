using Microsoft.AspNetCore.Identity;
using INTEXGroup3_7.Models.ViewModels;

namespace INTEXGroup3_7.Models;

public interface IAmazonRepository
{
    public IQueryable<Product> Products { get; }
    public IQueryable<Order> Orders { get; }
    public IQueryable<Customer> Customers { get; }

    public IQueryable<top_product> TopProducts { get; }
    
    public IQueryable<ItemRecommendation> ItemRecommendations { get; }

    public IQueryable<UserRecommendations> UserRecommendations { get; }
    public IQueryable<OrderPrediction> OrderPredictions { get; }


    public IQueryable<IdentityUser> Users { get; }

    public IQueryable<IdentityRole> Roles { get; }


    public IQueryable<IdentityUserRole> UserRoles { get; }


    public void UpdateOrder(Order order);


    public void DeleteOrder(Order order);


    public void AddOrder(Order order);

    public void UpdateProductItem(Product product);


    public void DeleteProductItem(Product product);


    public void AddProductItem(Product product);


    public void AddUserRole(IdentityUserRole user);


    public void UpdateUser(IdentityUserRole value);


    public void DeleteUser(IdentityUserRole value);


    public void AddPrediction(OrderPrediction prediction);


    public void ChangeFraud(int orderId);
}

    
    
    
    