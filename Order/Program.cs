using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Order.Data;
using Order.DTO;
using Order.Middleware;
using Order.Repository;
using Order.Repository.Interfaces;
using Order.Services;
using Order.Validators;
using System.Text;

public partial class Program
{
    private static void Main(string[] args)
    {
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

        // Add services to the container
        builder.Services.AddControllers()
            .AddFluentValidation();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddValidatorsFromAssemblyContaining<OrderCreateValidator>();

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
        // Register OrderDbContext
        builder.Services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));

        // Register HttpClient
        var productServiceUrl =
    builder.Configuration["ServiceUrls:ProductService"];

        builder.Services
     .AddHttpClient("ProductService", client =>
     {
         client.BaseAddress = new Uri(productServiceUrl!);
     })
     .AddStandardResilienceHandler(options =>
     {
         // Timeout
         options.AttemptTimeout.Timeout =
             TimeSpan.FromSeconds(5);

         // Retry
         options.Retry.MaxRetryAttempts = 1;

         // Circuit Breaker
         options.CircuitBreaker.SamplingDuration =
             TimeSpan.FromSeconds(30);

         options.CircuitBreaker.FailureRatio = 0.5;

         options.CircuitBreaker.MinimumThroughput = 2;

         options.CircuitBreaker.BreakDuration =
             TimeSpan.FromSeconds(20);
     });

        builder.Services.AddHttpContextAccessor();

        // Register Repository
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();

        // Register Service
        builder.Services.AddScoped<IOrderService, OrderService>();
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

        //adding middleware
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

         app.MapControllers();

        app.Run();
    }
}