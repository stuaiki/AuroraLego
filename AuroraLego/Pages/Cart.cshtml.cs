using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using INTEXGroup3_7.Models;
using INTEXGroup3_7.Infrastructure;

namespace INTEXGroup3_7.Pages;

public class CartModel : PageModel
{
    private IAmazonRepository _repo;
    
    public Cart Cart { get; set; }

    public CartModel(IAmazonRepository temp, Cart cartService)
    {
        _repo = temp;
        Cart = cartService;
    }
    public string ReturnUrl { get; set; } = "/";
    
    public void OnGet(string returnUrl)
    {
        ReturnUrl = returnUrl ?? "/";
    }

    public IActionResult OnPost(int product_ID, string returnUrl)
    {
        Product prod = _repo.Products
            .FirstOrDefault(x => x.product_ID == product_ID);

        if (prod != null)
        {
            Cart.AddItem(prod, 1);
        }

        return RedirectToPage (new {returnUrl = returnUrl});
    }

    public IActionResult OnPostRemove(int product_ID, string returnUrl)
    {
        Cart.RemoveLine(Cart.Lines.First(x => x.Product.product_ID == product_ID).Product);
        
        return RedirectToPage (new {returnUrl = returnUrl});
    }
    
    public IActionResult OnPostUpdate(int product_ID, string returnUrl, int quantity)
    {
        Product prod = _repo.Products
            .FirstOrDefault(x => x.product_ID == product_ID);

        if (prod != null)
        {
            Cart.UpdateItem(prod, quantity);
        }

        return RedirectToPage (new {returnUrl = returnUrl});
    }
}