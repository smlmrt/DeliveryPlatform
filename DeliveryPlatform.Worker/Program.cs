using DeliveryPlatform.Worker;
using DeliveryPlatform.DataAccess.Contexts;
using DeliveryPlatform.Core.Interfaces;
using DeliveryPlatform.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Veritabanı ve Repository bağlantılarını Worker'a tanıtıyoruz
builder.Services.AddDbContext<DeliveryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Kuyruğu dinleyecek servis
builder.Services.AddHostedService<OrderProcessingWorker>();

var host = builder.Build();
host.Run();