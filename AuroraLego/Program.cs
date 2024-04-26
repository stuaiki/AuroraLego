using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using INTEXGroup3_7.Data;
using INTEXGroup3_7.Models;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Add Connection string to the Default Connection
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

        //Session Creates instance of EFAmazonRepository
        builder.Services.AddScoped<IAmazonRepository, EFAmazonRepository>();

        builder.Services.AddRazorPages();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();

        builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // BETTER Password settings - INCREASE LENGTH and allow extra unique characters.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 12;
            options.Password.RequiredUniqueChars = 1;
        });

        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
        {
            microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"];
            microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
            microsoftOptions.SaveTokens = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts(); //USES HSTS Requirement 1.2
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Xss-Protection", "1");
            context.Response.Headers.Add("X-Frame-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            // context.Response.Headers.Add(
            //     "Content-Security-Policy",  
            //         "default-src 'self' ;" +
            //            "script-src 'self' ;" +  
            //             "style-src 'self' https://fonts.googleapis.com https://getbootstrap.com/;" +  
            //             "img-src 'self' 'https' data http://www.w3.org https://www.lego.com https://images.brickset.com https://m.media-amazon.com https://www.brickeconomy.com" +
            //             "font-src 'self' https://fonts.googleapis.com https://fonts.gstatic.com/;" +
            //             "media-src 'self'; " +
            //              "frame-src 'self'; "
            //     );  
            await next();
        });

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();
        app.UseRouting();


        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "productDetail",
            pattern: "Product/{product_ID}",
            defaults: new { Controller = "Home", action = "Product" }
        );

        app.MapControllerRoute(
            name: "shop",
            pattern: "{action=Shop}/{productNum}/{pageNum}",
            defaults: new { Controller = "Home", action = "Shop", pageNum = 1 }
        );

        app.MapControllerRoute(
            name: "default",
            pattern: "{action=Index}",
            defaults: new { Controller = "Home", action = "Index" }
        );

        app.MapDefaultControllerRoute();
        app.MapRazorPages();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Admin", "Customer", "Visitor" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        app.Run();
    }
}