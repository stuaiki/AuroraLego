using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using INTEXGroup3_7.Models;
using INTEXGroup3_7.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Security.Cryptography.Xml;

namespace INTEXGroup3_7.Controllers;

public class HomeController : Controller
{
    private IAmazonRepository _repo;
    private InferenceSession _session;

    public HomeController(IAmazonRepository temp)
    {
        _repo = temp;
        // bring in Onnx prediction model
        _session = new InferenceSession(
            "fraud_model.onnx");
    }

    public IActionResult Index()
    {
        var top_products = _repo.TopProducts.Select(tp => tp.product_ID);
        var rec_products = _repo.Products
            .Where(p => top_products.Contains(p.product_ID))
            .Take(8)
            .ToList();

        var id = 1;

        var user_rec = _repo.UserRecommendations.FirstOrDefault(x => x.customer_ID == id);

        var recProductNames = new List<string>
        {
            user_rec?.rec1, user_rec?.rec2, user_rec?.rec3, user_rec?.rec4, user_rec?.rec5
        };

        var groupProducts = _repo.Products.Where(x => recProductNames.Contains(x.name)).ToList();

        var data = new HomePageViewModel()
        {
            Products = rec_products,

            GroupOfProducts = groupProducts

        };

        return View(data);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Shop(int pageNum, int productNum, string[] selectedColors, string[] selectedCategories)
    {
        int pageSize = 6;

        if (productNum != 0 && productNum != null)
        {
            pageSize = productNum;
        }

        string[] colorsList = selectedColors;
        string[] categoryList = selectedCategories;

        ViewBag.selectedColors = colorsList;
        ViewBag.selectedColors = categoryList;

        // Get all products if no colors are selected
        var query = _repo.Products.AsQueryable();

        // Filter products by selected categories
        if (selectedCategories != null && selectedCategories.Any())
        {
            query = query.Where(p => selectedCategories.Contains(p.category));
        }

        // Filter products by selected colors
        if (colorsList != null && selectedColors.Any())
        {
            query = query.Where(p => selectedColors.Contains(p.primary_color));
        }

        var data = new ProductsViewModel()
        {
            Products = query
                .OrderBy(x => x.name)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize),

            PaginationInfo = new PaginationInfo
            {
                CurrentPage = pageNum,
                ItemsPerPage = pageSize,
                TotalItems = query.Count()
            },
            CurrentProductColors = colorsList,
            CurrentProductCategories = categoryList,
            CurrentProductNumber = productNum
        };

        return View(data);
    }

    [Authorize(Roles = "Admin, Customer")]
    [HttpGet]
    public IActionResult Checkout()
    {

        return View(new CheckoutOrderViewModel());
    }

    public IActionResult Checkout(CheckoutOrderViewModel model)
    {
        var customerExists = _repo.Customers.Any(c => c.customer_ID == model.Order.Customer.customer_ID);

        var order = new Order
        {
            // Assign properties directly that don't require conditional logic
            date = model.Order.date,
            day_of_week = model.Order.day_of_week,
            time = model.Order.time,
            entry_mode = model.Order.entry_mode,
            amount = model.Order.amount,
            type_of_transaction = model.Order.type_of_transaction,
            country_of_transaction = model.Order.Customer.country_of_residence,
            shipping_address = model.Order.shipping_address,
            bank = model.Order.bank,
            type_of_card = model.Order.type_of_card,
            fraud = model.Order.fraud
        };

        if (customerExists)
        {
            order.customer_ID = model.Order.Customer.customer_ID;
        }
        else
        {
            var customer = new Customer()
            {
                first_name = model.Order.Customer.first_name,

                last_name = model.Order.Customer.last_name,

                birth_date = model.Order.Customer.birth_date,

                country_of_residence = model.Order.Customer.country_of_residence,

                gender = model.Order.Customer.gender,

                age = model.Order.Customer.age
            };

            model.Order.Customer = _repo.AddCustomer(customer);
        }



        // Save the order to the database using the repository
        _repo.AddOrder(order);

        return View("OrderConfirmed", order);
    }
    public IActionResult Product(int product_ID)
    {
        var id = product_ID;

        var rec_data = _repo.ItemRecommendations.FirstOrDefault(x => x.product_ID == id);

        var recProductIds = new List<int?>
        {
            rec_data?.rec1, rec_data?.rec2, rec_data?.rec3, rec_data?.rec4, rec_data?.rec5,
            rec_data?.rec6, rec_data?.rec7, rec_data?.rec8, rec_data?.rec9, rec_data?.rec10
        }.Select(x => x + 1); ;

        var groupProducts = _repo.Products.Where(x => recProductIds.Contains(x.product_ID)).ToList();

        var data = new ProductRecommendationViewModel()
        {
            Product = _repo.Products.FirstOrDefault(x => x.product_ID == id),
            GroupProducts = groupProducts
        };

        return View(data);
    }

    public IActionResult Privacy()
    {

        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Admin()
    {
        var prod_data = _repo.Products.AsQueryable();

        var order_data = _repo.Orders.AsQueryable();

        var customer_date = _repo.Customers.AsQueryable();

        var user_data = _repo.Users.AsQueryable();


        var databaseData = new DatabaseModel()
        {
            Products = prod_data
                .OrderBy(x => x.product_ID),

            Orders = order_data
                .OrderByDescending(x => x.date)
                .Take(50),

            Users = user_data

        };

        return View(databaseData);
    }

    [HttpGet]
    public IActionResult AddOrder()
    {
        return View(new Order());
    }

    [HttpPost]
    public IActionResult AddOrder(Order model)
    {
        if (ModelState.IsValid)
        {
            // Create an Order entity from the view model
            var order = new Order
            {
                customer_ID = model.customer_ID,
                date = model.date,
                day_of_week = model.day_of_week,
                time = model.time,
                entry_mode = model.entry_mode,
                amount = model.amount,
                type_of_transaction = model.type_of_transaction,
                country_of_transaction = model.country_of_transaction,
                shipping_address = model.shipping_address,
                bank = model.bank,
                type_of_card = model.type_of_card,
                fraud = model.fraud
            };
            // Save the order to the database using the repository
            _repo.AddOrder(order);
            return RedirectToAction("OrderConfirmed");
        }
        else
        {
            return Redirect("OrderPending");
        }
    }

    [Authorize(Roles = "Admin")]
    public IActionResult OrderView(int id)
    {
        var recordToEdit = _repo.Orders
            .Include(o => o.Customer)
            .Single(x => x.transaction_ID == id);

        return View(recordToEdit);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult AddProduct()
    {
        ViewBag.PrimaryColors = _repo.Products
            .Select(p => p.primary_color) // Select the primary_color property
            .Distinct() // Get distinct values
            .OrderBy(color => color) // Order the distinct values
            .ToList(); // Convert to List

        ViewBag.SecondaryColors = _repo.Products
            .Select(p => p.secondary_color) // Select the secondary color property
            .Distinct() // Get distinct values
            .OrderBy(seccolor => seccolor) // Order the distinct values
            .ToList(); // Convert to List

        ViewBag.Categories = _repo.Products
            .Select(p => p.category) // Select the category property
            .Distinct() // Get distinct values
            .OrderBy(category => category) // Order the distinct values
            .ToList(); // Convert to List

        ViewBag.SubCategories = _repo.Products
            .Select(p => p.subcategory) // Select the subcategory property
            .Distinct() // Get distinct values
            .OrderBy(subcategory => subcategory) // Order the distinct values
            .ToList(); // Convert to List

        return View(new Product());
    }


    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult AddProduct(Product response)
    {
        if (ModelState.IsValid)
        {
            _repo.AddProductItem(response);

            return Redirect("Admin");
        }
        else
        {
            return View(response);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult ProductItemEdit(int id)
    {
        var recordToEdit = _repo.Products
            .Single(x => x.product_ID == id);

        ViewBag.PrimaryColors = _repo.Products
            .Select(p => p.primary_color) // Select the primary_color property
            .Distinct() // Get distinct values
            .OrderBy(color => color) // Order the distinct values
            .ToList(); // Convert to List

        ViewBag.SecondaryColors = _repo.Products
            .Select(p => p.secondary_color) // Select the secondary color property
            .Distinct() // Get distinct values
            .OrderBy(seccolor => seccolor) // Order the distinct values
            .ToList(); // Convert to List

        ViewBag.Categories = _repo.Products
            .Select(p => p.category) // Select the category property
            .Distinct() // Get distinct values
            .OrderBy(category => category) // Order the distinct values
            .ToList(); // Convert to List

        ViewBag.SubCategories = _repo.Products
            .Select(p => p.subcategory) // Select the subcategory property
            .Distinct() // Get distinct values
            .OrderBy(subcategory => subcategory) // Order the distinct values
            .ToList(); // Convert to List

        return View(recordToEdit);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult ProductItemEdit(Product product)
    {
        _repo.UpdateProductItem(product);
        return Redirect("Admin");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult ProductItemDelete(int id)
    {
        var recordToEdit = _repo.Products
            .Single(x => x.product_ID == id);
        return View(recordToEdit);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult ProductItemDelete(Product product)
    {
        _repo.DeleteProductItem(product);
        return Redirect("Admin");
    }

    [HttpGet]
    public IActionResult AddUser()
    {
        ViewBag.UserRoles = _repo.Roles
            .Select(p => p.Name) // Select the primary_color property
            .Distinct() // Get distinct values
            .OrderBy(role => role) // Order the distinct values
            .ToList();

        var Data = new IdentityUserRole()
        {
            User = new IdentityUser(),

            Role = new IdentityRole()
        };

        return View(Data);
    }

    [HttpPost]
    public IActionResult AddUser(IdentityUserRole response)
    {
        _repo.AddUserRole(response);

        return Redirect("Admin");
    }

    [HttpGet]
    public IActionResult UserEdit(string id)
    {
        ViewBag.UserRoles = _repo.Roles
            .Select(p => p.Name) // Select the Name property
            .Distinct() // Get distinct values
            .OrderBy(role => role) // Order the distinct values
            .ToList();

        ViewBag.TwoFactor = _repo.Users
            .Select(p => p.TwoFactorEnabled) // Select the two factor property
            .Distinct() // Get distinct values
            .OrderBy(factor => factor) // Order the distinct values
            .ToList();

        var user = _repo.Users.SingleOrDefault(x => x.Id == id);
        if (user == null)
        {
            return NotFound(); // Handle the scenario when the user is not found.
        }

        var roles = _repo.Roles; // Assuming _repo.Roles returns IQueryable<IdentityRole>

        var viewModel = new IdentityUserRole
        {
            User = user,
            Role = roles.FirstOrDefault() // Assuming you have a way to determine which role to show, adjust as necessary
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult UserEdit(IdentityUserRole value)
    {
        if (value.User.Email != null)
        {
            if (value.User.UserName != value.User.Email)
            {
                value.User.UserName = value.User.Email;

            }
        }

        _repo.UpdateUser(value);
        return Redirect("Admin");
    }

    [HttpGet]

    public IActionResult UserDelete(string id)
    {
        var user = _repo.Users.SingleOrDefault(x => x.Id == id);
        if (user == null)
        {
            return NotFound(); // Handle the scenario when the user is not found.
        }

        var roles = _repo.Roles; // Assuming _repo.Roles returns IQueryable<IdentityRole>

        var viewModel = new IdentityUserRole
        {
            User = user,
            Role = roles.FirstOrDefault() // Assuming you have a way to determine which role to show, adjust as necessary
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult UserDelete(IdentityUserRole value)
    {
        _repo.DeleteUser(value);
        return Redirect("Admin");
    }

    [Authorize(Roles = "Admin, Customer")]
    public IActionResult OrderConfirmed(string transactionId, decimal amount, string shippingAddress)
    {
        // Retrieve the most recent order from the database
        var latestOrder = _repo.Orders.OrderByDescending(o => o.date).FirstOrDefault();
        if (latestOrder != null)
        {
            ViewBag.TransactionId = latestOrder.transaction_ID;
            ViewBag.Amount = latestOrder.amount;
            ViewBag.ShippingAddress = latestOrder.shipping_address;
        }

        return View();
    }

    [Authorize(Roles = "Admin, Customer")]
    public IActionResult OrderPending()
    {

        return View();
    }

    [Authorize(Roles = "Admin, Customer")]
    [HttpPost]
    public IActionResult PredictFraud(CheckoutOrderViewModel model)
    {
        model.Order.customer_ID = 12;

        // Create an Order entity from the view model
        var newOrder = new Order
        {
            customer_ID = model.Order.customer_ID,
            date = model.Order.date,
            day_of_week = model.Order.day_of_week,
            time = model.Order.time,
            entry_mode = model.Order.entry_mode,
            amount = model.Order.amount,
            type_of_transaction = model.Order.type_of_transaction,
            country_of_transaction = model.Order.country_of_transaction,
            shipping_address = model.Order.shipping_address,
            bank = model.Order.bank,
            type_of_card = model.Order.type_of_card,
            fraud = model.Order.fraud
        };

        var class_type_dict = new Dictionary<int, string>
        {
            {0, "Not Fraud" },
            {1, "Fraud" }
        };

        var input = new List<float>
        {
            (float)(newOrder.customer_ID),
            (float)(newOrder.time),
            (float)(newOrder.amount),

            // Check dummy code for day_of_week
            newOrder.day_of_week == "Mon" ? 1 : 0,
            newOrder.day_of_week == "Sat" ? 1 : 0,
            newOrder.day_of_week == "Sun" ? 1 : 0,
            newOrder.day_of_week == "Thu" ? 1 : 0,
            newOrder.day_of_week == "Tue" ? 1 : 0,
            newOrder.day_of_week == "Wed" ? 1 : 0,

            // Check dummy code for entry_mode
            newOrder.entry_mode == "PIN" ? 1 : 0,
            newOrder.entry_mode == "Tap" ? 1 : 0,

            // Check dummy code for type_of_transaction
            newOrder.type_of_transaction == "Online" ? 1 : 0,
            newOrder.type_of_transaction == "POS" ? 1 : 0,

            // Check dummy code for country_of_transaction
            newOrder.country_of_transaction == "India" ? 1 : 0,
            newOrder.country_of_transaction == "Russia" ? 1 : 0,
            newOrder.country_of_transaction == "USA" ? 1 : 0,
            newOrder.country_of_transaction == "United Kingdom" ? 1 : 0,

            // Check dummy code for shipping_address
            newOrder.shipping_address == "India" ? 1 : 0,
            newOrder.shipping_address == "Russia" ? 1 : 0,
            newOrder.shipping_address == "USA" ? 1 : 0,
            newOrder.shipping_address == "United Kingdom" ? 1 : 0,

            // Check dummy code for bank
            newOrder.bank == "HSBC" ? 1 : 0,
            newOrder.bank == "Halifax" ? 1 : 0,
            newOrder.bank == "Lloyds" ? 1 : 0,
            newOrder.bank == "Metro" ? 1 : 0,
            newOrder.bank == "Monzo" ? 1 : 0,
            newOrder.bank == "RBS" ? 1 : 0,

            // Check dummy code for type_of_card
            newOrder.type_of_card == "Visa" ? 1 : 0,

        };

        var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });
        var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
                };

        string fraudPrediction;
        using (var results = _session.Run(inputs)) // makes the prediction with the inputs from the form 
        {
            var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
            if (prediction != null && prediction.Length > 0)
            {
                // Use the prediction to get the fraud status from the dictionary
                fraudPrediction = class_type_dict.GetValueOrDefault((int)prediction[0], "Unknown");
            }
            else
            {
                fraudPrediction = "Error: Unable to make a prediction.";
            }
        }

        var predictionResult = new OrderPrediction { order = newOrder, prediction = fraudPrediction };

        // Add the prediction to the Prod
        _repo.AddPrediction(predictionResult);

        if (predictionResult.prediction == "Not Fraud")
        {
            return View("OrderConfirmed", predictionResult.order);
        }
        else
        {
            _repo.ChangeFraud(predictionResult.order.transaction_ID);
            return View("OrderPending");
        }
    }
}