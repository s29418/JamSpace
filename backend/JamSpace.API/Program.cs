using JamSpace.Application.Interfaces;
// using JamSpace.Application.Services;
using JamSpace.Infrastructure.Repositories;
using JamSpace.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DefaultNamespace;
using JamSpace.Application.Authentication;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// 1. DB CONTEXT (Entity Framework Core)
// =====================================
builder.Services.AddDbContext<JamSpaceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =====================================
// 2. SERWISY APLIKACYJNE (DI)
// =====================================
// builder.Services.AddScoped<TeamService>(); // zostaje Twój TeamService

// =====================================
// 3. REPOZYTORIA (interfejs + implementacja)
// =====================================
builder.Services.AddScoped<IUserRepository, UserRepository>();

// =====================================
// 4. JWT TOKEN GENERATOR
// =====================================
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// =====================================
// 5. MEDIATR – obsługa Commandów i Handlerów
// =====================================
builder.Services.AddMediatR(typeof(RegisterUserHandler).Assembly);

// =====================================
// 6. JWT Authentication
// =====================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

// =====================================
// 7. KONTROLERY + SWAGGER
// =====================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =====================================
// 8. ŚRODOWISKO (SWAGGER DEV ONLY)
// =====================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =====================================
// 9. MIDDLEWARE
// =====================================
app.UseAuthentication(); // <- JWT auth
app.UseAuthorization();

app.MapControllers();
app.Run();
