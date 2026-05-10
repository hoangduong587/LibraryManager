using LibraryManager.Api.Data;
using LibraryManager.Api.Models;
using LibraryManager.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// DATABASE
// ---------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------------------------------------------------------
// IDENTITY
// ---------------------------------------------------------
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ---------------------------------------------------------
// JWT AUTHENTICATION
// ---------------------------------------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"])
        ),

        ClockSkew = TimeSpan.Zero
    };
});

// ---------------------------------------------------------
// SERVICES
// ---------------------------------------------------------
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<BookSeeder>();


// ---------------------------------------------------------
// AUTOMAPPER (REQUIRED)
// ---------------------------------------------------------
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ---------------------------------------------------------
// CONTROLLERS
// ---------------------------------------------------------
builder.Services.AddControllers();

// ---------------------------------------------------------
// SWAGGER + JWT SUPPORT
// ---------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Cors configuration to allow Blazor WebAssembly client to access the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("http://localhost:5099")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
// Apply CORS policy
app.UseCors("AllowBlazor");

// ---------------------------------------------------------
// MIDDLEWARE
// ---------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ---------------------------------------------------------
// DATA SEEDING
// ---------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();

    var bookSeeder = scope.ServiceProvider.GetRequiredService<BookSeeder>();
    await bookSeeder.SeedBooksAsync();
}

// ---------------------------------------------------------
// ENDPOINTS
// ---------------------------------------------------------
app.MapControllers();

app.Run();
