using Microsoft.EntityFrameworkCore;
using Order.Data;
using Order.Middleware;
using Order.Repository;
using Order.Repository.Interfaces;
using Order.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Register OrderDbContext
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register HttpClient
var productServiceUrl = builder.Configuration["ServiceUrls:ProductService"];

builder.Services.AddHttpClient("ProductService", client =>
{
    client.BaseAddress = new Uri(productServiceUrl!);
});
// Register Repository
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register Service
builder.Services.AddScoped<IOrderService, OrderService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

//adding middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();