namespace INTEXGroup3_7.Models.ViewModels;

public class ProductsViewModel
{
    public IQueryable<Product> Products { get; set; }
    public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
    public string[]? CurrentProductColors { get; set; }
    
    public string[]? CurrentProductCategories { get; set; }
    
    public int? CurrentProductNumber { get; set; }
}