using DefaultNamespace;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dodanie DbContext z konfiguracją połączenia
builder.Services.AddDbContext<JamSpaceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Brak Swaggera, brak HTTPS redirection – maksymalne uproszczenie

app.Run();