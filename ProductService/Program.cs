using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ProductService.Data;
using ProductService.Middleware;
using ProductService.Repository;
using ProductService.Repository.Interfaces;
using ProductService.Services;
using ProductService.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{

 options.TokenValidationParameters =
     new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,

         ValidIssuer = builder.Configuration["Jwt:Issuer"],
         ValidAudience = builder.Configuration["Jwt:Audience"],

         IssuerSigningKey =
             new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(
                     builder.Configuration["Jwt:Key"]!
                 ))
     };


 options.Events = new JwtBearerEvents
 {
     OnMessageReceived = context =>
     {

         return Task.CompletedTask;
     },

     OnTokenValidated = context =>
     {

         return Task.CompletedTask;
     },

     OnAuthenticationFailed = context =>
     {

         return Task.CompletedTask;
     }
 };
});

// Add Controllers
builder.Services.AddControllers();

// Register DbContext
builder.Services.AddDbContext<EcommerceApiDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register Service
builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });


    options.AddSecurityRequirement(document =>
    {
        return new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer", document),
                new List<string>()
            }
        };
    });
});

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

//adding Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();