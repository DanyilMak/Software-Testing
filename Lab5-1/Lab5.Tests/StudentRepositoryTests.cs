using Lab5.Data;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Lab5.Tests;

public class StudentRepositoryTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=lab5;Username=postgres;Password=postgres")
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    // 1. CREATE
    [Fact]
    public async Task AddStudent_ShouldSaveStudent()
    {
        // Arrange
        var context = CreateDbContext();

        var student = new Student
        {
            FullName = "Test User",
            Email = "test1@test.com",
            EnrollmentDate = DateTime.UtcNow
        };

        // Act
        context.Students.Add(student);
        await context.SaveChangesAsync();

        // Assert
        var result = await context.Students.FirstOrDefaultAsync(s => s.Email == "test1@test.com");
        result.ShouldNotBeNull();
        result.FullName.ShouldBe("Test User");
    }

    // 2. READ ALL
    [Fact]
    public async Task GetAll_ShouldReturnStudents()
    {
        // Arrange
        var context = CreateDbContext();

        context.Students.Add(new Student { FullName = "A", Email = "a@a.com" });
        context.Students.Add(new Student { FullName = "B", Email = "b@b.com" });
        await context.SaveChangesAsync();

        // Act
        var result = await context.Students.ToListAsync();

        // Assert
        result.Count.ShouldBe(2);
    }

    // 3. READ BY ID + INCLUDE
    [Fact]
    public async Task GetStudent_ShouldIncludeEnrollments()
    {
        // Arrange
        var context = CreateDbContext();

        var student = new Student
        {
            FullName = "WithEnrollments",
            Email = "inc@test.com",
            Enrollments = new List<Enrollment>
            {
                new Enrollment { Grade = 95 }
            }
        };

        context.Students.Add(student);
        await context.SaveChangesAsync();

        // Act
        var result = await context.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Email == "inc@test.com");

        // Assert
        result.ShouldNotBeNull();
        result.Enrollments.Count.ShouldBe(1);
    }

    // 4. DELETE
    [Fact]
    public async Task DeleteStudent_ShouldRemove()
    {
        // Arrange
        var context = CreateDbContext();

        var student = new Student
        {
            FullName = "Delete Me",
            Email = "del@test.com"
        };

        context.Students.Add(student);
        await context.SaveChangesAsync();

        // Act
        context.Students.Remove(student);
        await context.SaveChangesAsync();

        // Assert
        var result = await context.Students.FindAsync(student.Id);
        result.ShouldBeNull();
    }

    // 5. UNIQUE EMAIL constraint
    [Fact]
    public async Task DuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var context = CreateDbContext();

        context.Students.Add(new Student { Email = "dup@test.com", FullName = "1" });
        await context.SaveChangesAsync();

        context.Students.Add(new Student { Email = "dup@test.com", FullName = "2" });

        // Act & Assert
        await Should.ThrowAsync<DbUpdateException>(async () =>
        {
            await context.SaveChangesAsync();
        });
    }

    // 6. FOREIGN KEY constraint
    [Fact]
    public async Task InvalidEnrollment_ShouldFail()
    {
        // Arrange
        var context = CreateDbContext();

        var enrollment = new Enrollment
        {
            StudentId = 999,
            CourseId = 999,
            Grade = 50
        };

        context.Enrollments.Add(enrollment);

        // Act & Assert
        await Should.ThrowAsync<DbUpdateException>(async () =>
        {
            await context.SaveChangesAsync();
        });
    }
}