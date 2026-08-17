using DeliveryPlatform.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using DeliveryPlatform.Business.Interfaces;
using DeliveryPlatform.Business.RabbitMQ;
using DeliveryPlatform.Business.Services;
using DeliveryPlatform.Core.Interfaces;
using DeliveryPlatform.DataAccess.Repositories;
using DeliveryPlatform.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DeliveryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Generic Repository Kaydı
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// RabbitMQ ve Sipariş Servislerinin Kaydı
builder.Services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Add services to the container.
builder.Services.AddControllersWithViews();builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<CourierHub>("/courierHub");


app.Run();
