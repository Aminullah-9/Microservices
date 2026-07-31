using Microsoft.EntityFrameworkCore;
using Order.Data;
using Order.Repository;
using Order.Repository.Interfaces;
using Order.Services;
using Order.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Register OrderDbContext
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register HttpClient
builder.Services.AddHttpClient();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();