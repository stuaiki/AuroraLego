namespace INTEXGroup3_7.Models.ViewModels;

public class HomePageViewModel
{
    public List<Product> Products { get; set; }
    
    public IEnumerable<Product>? GroupOfProducts { get; set; }
}