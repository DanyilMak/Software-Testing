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

public class InMemoryTests
{
    private AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Add_Works()
    {
        using var ctx = Create();
        var repo = new StudentRepository(ctx);

        await repo.AddAsync(new Student
        {
            FullName = "Test",
            Email = "test@test.com",
            EnrollmentDate = DateTime.UtcNow,
            
        });

        ctx.Students.Count().ShouldBe(1);
    }

    [Fact]
    public async Task GetById_IncludesEnrollments()
    {
        using var ctx = Create();

        var course = new Course { Title = "Math" };
        var student = new Student
        {
            FullName = "A",
            Email = "a@test.com",
            EnrollmentDate = DateTime.UtcNow,
            Enrollments = new List<Enrollment>
            {
                new Enrollment { Course = course, Grade = 90 }
            },
            
        };

        ctx.Students.Add(student);
        await ctx.SaveChangesAsync();

        var repo = new StudentRepository(ctx);
        var result = await repo.GetByIdAsync(student.Id);

        result!.Enrollments.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Update_Works()
    {
        using var ctx = Create();

        var student = new Student { FullName = "A", Email = "a@test.com",  };
        ctx.Students.Add(student);
        await ctx.SaveChangesAsync();

        student.FullName = "Updated";

        var repo = new StudentRepository(ctx);
        await repo.UpdateAsync(student);

        ctx.Students.First().FullName.ShouldBe("Updated");
    }

    [Fact]
    public async Task Delete_Works()
    {
        using var ctx = Create();

        var student = new Student { FullName = "A", Email = "a@test.com",  };
        ctx.Students.Add(student);
        await ctx.SaveChangesAsync();

        var repo = new StudentRepository(ctx);
        await repo.DeleteAsync(student.Id);

        ctx.Students.Count().ShouldBe(0);
    }

    [Fact]
    public async Task GetAll_Works()
    {
        using var ctx = Create();

        ctx.Students.Add(new Student { FullName = "A", Email = "a@test.com",  });
        ctx.Students.Add(new Student { FullName = "B", Email = "b@test.com",  });

        await ctx.SaveChangesAsync();

        var repo = new StudentRepository(ctx);
        var result = await repo.GetAllAsync();

        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetTopStudents_Works()
    {
        using var ctx = Create();

        var course = new Course { Title = "Math" };

        ctx.Students.AddRange(
            new Student
            {
                FullName = "Low",
                Email = "l@test.com",
                Enrollments = new List<Enrollment>
                {
                    new Enrollment { Course = course, Grade = 50 }
                },
                
            },
            new Student
            {
                FullName = "High",
                Email = "h@test.com",
                Enrollments = new List<Enrollment>
                {
                    new Enrollment { Course = course, Grade = 100 }
                },
                
            }
        );

        await ctx.SaveChangesAsync();

        var repo = new StudentRepository(ctx);
        var top = await repo.GetTopStudentsAsync(1);

        top.First().FullName.ShouldBe("High");
    }
}