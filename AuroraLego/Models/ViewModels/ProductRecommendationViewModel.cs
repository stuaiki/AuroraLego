namespace INTEXGroup3_7.Models.ViewModels;

public class ProductRecommendationViewModel
{
    public Product? Product { get; set; }
    public IEnumerable<Product>? GroupProducts { get; set; }
}