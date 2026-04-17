using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔌 PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 📦 Сервіси
builder.Services.AddControllers();

var app = builder.Build();

// 📊 Міграції та сидінг
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Students.Any())
    {
        var students = new List<Student>();
        for (int i = 1; i <= 10000; i++)
        {
            students.Add(new Student
            {
                Name = $"Student {i}",
                Age = Random.Shared.Next(18, 30),
                Email = $"student{i}@test.com"
            });
        }
        db.Students.AddRange(students);
        db.SaveChanges();
    }
}

// 🌐 Middleware
app.UseAuthorization();
app.MapControllers();

// ✅ Health endpoint для CI/CD
app.MapGet("/health/ready", () => Results.Ok("ready"));

app.Run();
