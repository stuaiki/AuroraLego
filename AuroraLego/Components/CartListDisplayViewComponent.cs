using INTEXGroup3_7.Models;
using Microsoft.AspNetCore.Mvc;

using INTEXGroup3_7.Models;
using Microsoft.AspNetCore.Mvc;
using INTEXGroup3_7.Models.ViewModels;


namespace INTEXGroup3_7.Components
{
    public class CartListDisplayViewComponent : ViewComponent
    {
        private Cart cart;
        public Order order;

        public CartListDisplayViewComponent(Cart cartService)
        {
            this.cart = cartService;
        }

        public IViewComponentResult Invoke()
        {
            var data = new CheckoutOrderViewModel
            {
                Cart = cart,

                Order = order
            };
            return View(data);
        }
    }

}