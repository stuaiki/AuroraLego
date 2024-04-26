using System.Text.Json.Serialization;
using INTEXGroup3_7.Infrastructure;
using INTEXGroup3_7.Models;

namespace INTEXGroup3_7.Models;

public class SessionCart : Cart
{
    public static Cart GetCart(IServiceProvider services)
    {
        ISession? session = services.GetRequiredService<IHttpContextAccessor>()
            .HttpContext?.Session;

        SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? 
            new SessionCart();

        cart.Session = session;

        return cart;
    }
    
    [JsonIgnore]
    public ISession? Session { get; set; }

    public override void AddItem(Product prod, int quanity)
    {
        base.AddItem(prod, quanity);
        Session?.SetJson("Cart", this);
    }
    public override void UpdateItem(Product prod, int quantity)
    {
        base.UpdateItem(prod, quantity);
        Session?.SetJson("Cart", this);
    }

    public override void RemoveLine(Product prod)
    {
        base.RemoveLine(prod);
        Session?.SetJson("Cart", this);
    }
    public override void Clear()
    {
        base.Clear();
        Session?.Remove("Cart");
    }
    
}