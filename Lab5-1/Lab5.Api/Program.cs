using Lab5.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔌 Підключення до PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// 📦 Додаткові сервіси
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// 📌 Мінімальні ендпоінти

// GET всі студенти
app.MapGet("/students", async (AppDbContext db) =>
{
    return await db.Students.ToListAsync();
});

// GET студент по ID
app.MapGet("/students/{id}", async (int id, AppDbContext db) =>
{
    var student = await db.Students
        .Include(s => s.Enrollments)
        .FirstOrDefaultAsync(s => s.Id == id);

    return student is not null ? Results.Ok(student) : Results.NotFound();
});

// POST студент
app.MapPost("/students", async (Student student, AppDbContext db) =>
{
    db.Students.Add(student);
    await db.SaveChangesAsync();
    return Results.Created($"/students/{student.Id}", student);
});

// DELETE студент
app.MapDelete("/students/{id}", async (int id, AppDbContext db) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null)
        return Results.NotFound();

    db.Students.Remove(student);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// 🗄️ Автостворення БД
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
