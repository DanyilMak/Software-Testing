using Microsoft.EntityFrameworkCore;
using Lab5.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=lab5db;Username=postgres;Password=yourpassword"));


builder.Services.AddScoped<StudentRepository>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();