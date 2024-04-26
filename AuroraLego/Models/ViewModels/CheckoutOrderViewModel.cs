namespace INTEXGroup3_7.Models.ViewModels;

public class CheckoutOrderViewModel
{
    public Cart Cart { get; set; }

    public Order Order { get; set; } = new Order();
    
    public Customer Customer { get; set; }
}