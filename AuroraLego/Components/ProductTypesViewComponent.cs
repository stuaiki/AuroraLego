using INTEXGroup3_7.Models;
using Microsoft.AspNetCore.Mvc;
using INTEXGroup3_7.Models.ViewModels;

namespace INTEXGroup3_7.Components;

public class ProductTypesViewComponent : ViewComponent
{
    private IAmazonRepository _Repo;
    
    public ProductTypesViewComponent(IAmazonRepository temp)
    {
        _Repo = temp;
    }

    public IViewComponentResult Invoke()
    {
        
        var productColors = _Repo.Products
            .Select(x => x.primary_color)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var productCategories = _Repo.Products
            .Select(x => x.category)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var ProductTypes = new ProductFilterViewModel()
        {
            Colors = productColors,
            Category = productCategories
        };

        return View(ProductTypes);
    }
}