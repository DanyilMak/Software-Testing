using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Xunit;
using Shouldly;
using Lab5.Data;
namespace Lab5.Tests;
public class SqliteTests
{
    private (AppDbContext context, SqliteConnection connection) CreateContext()
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
    public async Task ForeignKey_InvalidIds_ThrowsAsync()
    {
        var (context, connection) = CreateContext();

        using (connection)
        using (context)
        {
            var enrollment = new Enrollment
            {
                StudentId = 999, 
                CourseId = 999   
            };

            context.Enrollments.Add(enrollment);

            await Should.ThrowAsync<DbUpdateException>(() =>
                context.SaveChangesAsync());
        }
    }


    [Fact]
    public async Task Unique_Email_Duplicate_ThrowsAsync()
    {
        var (context, connection) = CreateContext();

        using (connection)
        using (context)
        {
            var s1 = new Student
            {
                FullName = "A",
                Email = "dup@test.com",
                
            };

            var s2 = new Student
            {
                FullName = "B",
                Email = "dup@test.com",
                
            };

            context.Students.Add(s1);
            await context.SaveChangesAsync();

            context.Students.Add(s2);

            await Should.ThrowAsync<DbUpdateException>(() =>
                context.SaveChangesAsync());
        }
    }


    [Fact]
    public async Task CascadeDelete_RemovesEnrollments()
    {
        var (context, connection) = CreateContext();

        using (connection)
        using (context)
        {
            var course = new Course { Title = "Math" };

            var student = new Student
            {
                FullName = "Test",
                Email = "test@test.com",
                Enrollments = new List<Enrollment>
                {
                    new Enrollment { Course = course, Grade = 90 }
                },
                
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
        var (context, connection) = CreateContext();

        using (connection)
        {

            var student = new Student
            {
                FullName = "Concurrent",
                Email = "con@test.com",
                
            };

            context.Students.Add(student);
            await context.SaveChangesAsync();


            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            using var ctx1 = new AppDbContext(options);
            using var ctx2 = new AppDbContext(options);

            var s1 = await ctx1.Students.FirstAsync();
            var s2 = await ctx2.Students.FirstAsync();

            s1.FullName = "Update1";
            await ctx1.SaveChangesAsync();

            s2.FullName = "Update2";

            await Should.ThrowAsync<DbUpdateConcurrencyException>(() =>
                ctx2.SaveChangesAsync());
        }
    }

    [Fact]
    public async Task InMemory_DoesNotThrow_But_Sqlite_Does()
    {
        var (context, connection) = CreateContext();

        using (connection)
        using (context)
        {
            // У SQLite це впаде через FK
            var enrollment = new Enrollment
            {
                StudentId = 999,
                CourseId = 999
            };

            context.Enrollments.Add(enrollment);

            await Should.ThrowAsync<DbUpdateException>(() =>
                context.SaveChangesAsync());
        }
    }
}