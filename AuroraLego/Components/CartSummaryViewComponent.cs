using INTEXGroup3_7.Models;
using Microsoft.AspNetCore.Mvc;

namespace INTEXGroup3_7.Components;

public class CartSummaryViewComponent : ViewComponent
{
    private Cart cart;

    public CartSummaryViewComponent(Cart cartService)
    {
        this.cart = cartService;
    }

    public IViewComponentResult Invoke()
    {
        return View(cart);
    }
}