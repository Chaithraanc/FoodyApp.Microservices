using Foody.Web.Service;
using Foody.Web.Service.IService;
using Foody.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();  //For accessing HttpContext ie cookies in services
builder.Services.AddHttpClient(); 

builder.Services.AddScoped<ITokenProvider, TokenProvider>(); // For JWT token management
builder.Services.AddHttpClient<ICouponService, CouponService>(); // For accessing Coupon API
builder.Services.AddHttpClient<IAuthService , AuthService>(); // For accessing Auth API
builder.Services.AddHttpClient<IProductService, ProductService>(); // For accessing Product API
builder.Services.AddHttpClient<IcartService, CartService>(); // For accessing Shopping Cart API
builder.Services.AddHttpClient<IOrderService, OrderService>(); // For accessing Order API

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IProductService, ProductService>(); // For accessing Product API
builder.Services.AddScoped<IcartService, CartService>(); // For accessing Shopping Cart API
builder.Services.AddScoped<IOrderService, OrderService>(); // For accessing Order API
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10); // Set cookie expiration time
        options.LoginPath = "/Auth/Login"; // Redirect to login page if not authenticated
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Redirect to access denied page
    });

SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"]; // For accessing Coupon API
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];    // For accessing Auth API
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"]; // For accessing Product API
SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"]; // For accessing Shopping Cart API
SD.OrderAPIBase = builder.Configuration["ServiceUrls:OrderAPI"]; // For accessing Order API
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
