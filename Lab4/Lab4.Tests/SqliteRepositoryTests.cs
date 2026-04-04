using Lab4.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class SqliteRepositoryTests
{
    private (AppDbContext context, SqliteConnection connection) CreateSqliteContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return (context, connection);
    }

    [Fact]
    public async Task ForeignKey_EnrollingInNonExistingCourse_ThrowsAsync()
    {
        // Arrange
        var (context, connection) = CreateSqliteContext();
        using (connection)
        using (context)
        {
            var enrollment = new Enrollment
            {
                StudentId = 999,
                CourseId = 999,
                Grade = 85
            };

            // Act & Assert
            await Should.ThrowAsync<DbUpdateException>(() =>
            {
                context.Enrollments.Add(enrollment);
                return context.SaveChangesAsync();
            });
        }
    }

    [Fact]
    public async Task UniqueConstraint_DuplicateEmail_ThrowsAsync()
    {
        // Arrange
        var (context, connection) = CreateSqliteContext();
        using (connection)
        using (context)
        {
            var student1 = new Student { FullName = "Alice", Email = "dup@test.com", EnrollmentDate = DateTime.UtcNow };
            var student2 = new Student { FullName = "Bob", Email = "dup@test.com", EnrollmentDate = DateTime.UtcNow };

            context.Students.Add(student1);
            await context.SaveChangesAsync();

            // Act & Assert
            context.Students.Add(student2);
            await Should.ThrowAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }
    }

    [Fact]
    public async Task CascadeDelete_DeletingStudent_RemovesEnrollmentsAsync()
    {
        // Arrange
        var (context, connection) = CreateSqliteContext();
        using (connection)
        using (context)
        {
            var course = new Course { Title = "CS101", Credits = 3 };
            var student = new Student
            {
                FullName = "Charlie",
                Email = "charlie@test.com",
                EnrollmentDate = DateTime.UtcNow,
                Enrollments = new List<Enrollment>
                {
                    new Enrollment { Course = course, Grade = 88 }
                }
            };

            context.Students.Add(student);
            await context.SaveChangesAsync();

            // Act
            context.Students.Remove(student);
            await context.SaveChangesAsync();

            // Assert
            var enrollments = await context.Enrollments.ToListAsync();
            enrollments.ShouldBeEmpty();
        }
    }

    [Fact]
    public async Task Concurrency_UpdateConflict_ThrowsAsync()
    {
        // Arrange
        var (context, connection) = CreateSqliteContext();
        using (connection)
        {
            var student = new Student { FullName = "Concurrent", Email = "con@test.com", EnrollmentDate = DateTime.UtcNow };
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
            var context1 = new AppDbContext(options);
            var context2 = new AppDbContext(options);

            var s1 = await context1.Students.FirstAsync();
            var s2 = await context2.Students.FirstAsync();

            s1.FullName = "Update1";
            s2.FullName = "Update2";

            // Act
            await context1.SaveChangesAsync();

            // Assert
            await Should.ThrowAsync<DbUpdateConcurrencyException>(() => context2.SaveChangesAsync());
        }
    }

    [Fact]
    public async Task CompareBehavior_InMemoryVsSqlite_DifferentConstraints()
    {
        // Arrange
        var (sqliteContext, sqliteConn) = CreateSqliteContext();
        using (sqliteConn)
        using (sqliteContext)
        {
            var student1 = new Student { FullName = "Test", Email = "dup@test.com", EnrollmentDate = DateTime.UtcNow };
            var student2 = new Student { FullName = "Test2", Email = "dup@test.com", EnrollmentDate = DateTime.UtcNow };

            // Act
            sqliteContext.Students.Add(student1);
            await sqliteContext.SaveChangesAsync();

            sqliteContext.Students.Add(student2);

            // Assert
            await Should.ThrowAsync<DbUpdateException>(() => sqliteContext.SaveChangesAsync());
        }
    }
}
