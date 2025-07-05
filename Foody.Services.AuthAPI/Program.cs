
using Microsoft.EntityFrameworkCore;
using Foody.Services.AuthAPI.Data;
using Microsoft.AspNetCore.Identity;
using Foody.Services.AuthAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using Foody.Services.AuthAPI.Service.IService;
using Foody.Services.AuthAPI.Service;
using Foody.MessageBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService , AuthService>();
builder.Services.AddScoped<IJwtTokenGenerator , JwtTokenGenerator>();
builder.Services.AddScoped<IMessageBus, MessageBus>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI(c=>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Foody Auth API V1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at the app's root
    });

//// Allow anonymous access to Swagger
//app.Use(async (context, next) =>
//{
//    var path = context.Request.Path.Value;

//    if (path != null && (path.StartsWith("/swagger") || path == "/swagger/index.html" || path == "/" || path.Contains("swagger.json")))
//    {
//        await next();
//    }
//    else
//    {
//        await next();
//    }
//});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.MapGet("/", () => "Foody Auth API is running...");

app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}
