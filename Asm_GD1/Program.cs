using Asm_GD1.Data;
using Asm_GD1.Models;
using Asm_GD1.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;

    });

// Thêm Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<SlugGenerator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Accounts.RemoveRange(context.Accounts);
    context.Products.RemoveRange(context.Products);
    context.ProductSizes.RemoveRange(context.ProductSizes);
    context.ProductToppings.RemoveRange(context.ProductToppings);
    context.SaveChanges();

    if (!context.Accounts.Any())
    {
        var hasher = new PasswordHasher<Account>();

        var adminit = new Account
        {
            Username = "adminit",
            Email = "adminit@gmail.com",
            Role = "adminit",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        adminit.Password = hasher.HashPassword(adminit, "adminit@gmail.com");

        var admin = new Account
        {
            Username = "admin",
            Email = "admin@gmail.com",
            Role = "admin",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        admin.Password = hasher.HashPassword(admin, "admin@gmail.com");

        var admin1 = new Account
        {
            Username = "admin1",
            Email = "admin1@gmail.com",
            Role = "admin1",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        admin1.Password = hasher.HashPassword(admin1, "admin1@gmail.com");

        var staff = new Account
        {
            Username = "staff",
            Email = "staff@gmail.com",
            Role = "staff",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        staff.Password = hasher.HashPassword(staff, "staff@gmail.com");

        var user = new Account
        {
            Username = "user",
            Email = "user@gmail.com",
            Role = "user",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        user.Password = hasher.HashPassword(user, "user@gmail.com");

        context.Accounts.AddRange(adminit, admin, admin1, staff, user);
        context.SaveChanges();
    }
    if (!context.ProductSizes.Any())
    {
        var sizes = new List<ProductSize>
        {
            new ProductSize { SizeName = "Bình thường", ExtraPrice = 0 },
            new ProductSize { SizeName = "Lớn", ExtraPrice = 10000 },
            new ProductSize { SizeName = "Đặc biệt", ExtraPrice = 20000 }
        };
        context.ProductSizes.AddRange(sizes);
        context.SaveChanges();
    }

    if (!context.ProductToppings.Any())
    {
        var toppings = new List<ProductTopping>
        {
            new ProductTopping { ToppingName = "Thêm trứng", ExtraPrice = 5000 },
            new ProductTopping { ToppingName = "Thêm chả", ExtraPrice = 8000 },
            new ProductTopping { ToppingName = "Thêm sườn", ExtraPrice = 15000 }
        };
        context.ProductToppings.AddRange(toppings);
        context.SaveChanges();
    }

    if (!context.Products.Any())
    {
        var sizeId = context.ProductSizes.First().SizeID;
        var toppingId = context.ProductToppings.First().ToppingID;

        var product1 = new Product
        {
            Name = "Cơm tấm sườn nướng",
            Description = "Cơm tấm thơm ngon với sườn nướng và nước mắm đặc biệt",
            Slug = "com-tam-suon-nuong",
            ImageUrl = "~/Images/com-tam.jpg",
            BasePrice = 45000,
            DiscountPrice = 40000,
            DiscountPercent = 11,
            Rating = 4.8M,
            RatingCount = 124,
            SoldCount = 250,
            IsHot = true,
            SizeID = sizeId,
            ToppingID = toppingId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        context.Products.Add(product1);
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Routing cho ASP.NET Core
app.MapControllerRoute(
    name: "productDetails",
    pattern: "food/{Slug}",
    defaults: new { controller = "Food", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
