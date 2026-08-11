using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ProductService.Data;
using ProductService.DTO;
using ProductService.Middleware;
using ProductService.Repository;
using ProductService.Repository.Interfaces;
using ProductService.Services;
using ProductService.Services.Interfaces;
using ProductService.Validator;
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
builder.Services.AddControllers()
           .AddFluentValidation();

builder.Services.AddHttpContextAccessor();

builder.Services.AddValidatorsFromAssemblyContaining<ProductValidor>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value!.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed.",
            StatusCode = StatusCodes.Status400BadRequest,
            Data = errors
        };

        return new BadRequestObjectResult(response);
    };
});
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