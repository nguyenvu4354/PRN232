using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingWeb.Config;
using ShoppingWeb.Models;
using ShoppingWeb.Services;
using ShoppingWeb.Services.Interface;
using ShoppingWeb.Services.IServices;
using ShoppingWeb.Services.ThirdParty;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ShoppingWebContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ShoppingWeb.Services.Interface.IBrandService, ShoppingWeb.Services.BrandService>();
builder.Services.AddScoped<ShoppingWeb.Services.Interface.ICategoryService, ShoppingWeb.Services.CategoryService>();
builder.Services.AddScoped<ShoppingWeb.Services.Interface.IProductManagementService, ShoppingWeb.Services.ProductManagementService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
builder.Services.AddScoped<IGHN, GHN>();
builder.Services.AddScoped<IPayOS, PayOS>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
