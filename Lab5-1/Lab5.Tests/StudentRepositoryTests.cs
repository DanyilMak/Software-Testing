using Lab5.Data;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Testcontainers.PostgreSql;

namespace Lab5.Tests;

public class StudentRepositoryTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres;
    private AppDbContext _context;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder()
            .WithDatabase("lab5")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _context = new AppDbContext(options);
        await _context.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    [Fact]
    public async Task AddStudent_ShouldSaveStudent()
    {
        var student = new Student { FullName = "Test User", Email = "test@test.com" };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var result = await _context.Students.FirstOrDefaultAsync(s => s.Email == "test@test.com");
        result.ShouldNotBeNull();
    }
}