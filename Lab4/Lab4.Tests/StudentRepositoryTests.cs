using Lab4.Data;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lab4.Tests
{
    public class StudentRepositoryTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ValidStudent_SavesSuccessfully()
        {
            using var context = CreateInMemoryContext();
            var repo = new StudentRepository(context);

            var student = new Student
            {
                FullName = "John Doe",
                Email = "john@example.com",
                EnrollmentDate = DateTime.UtcNow
            };

            await repo.AddAsync(student);

            var saved = await context.Students.FirstOrDefaultAsync(s => s.Email == "john@example.com");
            saved.ShouldNotBeNull();
            saved.FullName.ShouldBe("John Doe");
        }

        [Fact]
        public async Task GetByIdAsync_IncludesEnrollments()
        {
            using var context = CreateInMemoryContext();
            var course = new Course { Title = "Testing 101", Credits = 3 };
            var student = new Student
            {
                FullName = "Jane Smith",
                Email = "jane@example.com",
                EnrollmentDate = DateTime.UtcNow,
                Enrollments = new List<Enrollment>
                {
                    new Enrollment { Course = course, Grade = 95 }
                }
            };
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var repo = new StudentRepository(context);
            var result = await repo.GetByIdAsync(student.Id);

            result.ShouldNotBeNull();
            result.Enrollments.Count.ShouldBe(1);
            result.Enrollments.First().Grade.ShouldBe(95m);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllStudents()
        {
            using var context = CreateInMemoryContext();
            context.Students.AddRange(
                new Student { FullName = "A", Email = "a@test.com", EnrollmentDate = DateTime.UtcNow },
                new Student { FullName = "B", Email = "b@test.com", EnrollmentDate = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();

            var repo = new StudentRepository(context);
            var all = await repo.GetAllAsync();

            all.Count.ShouldBe(2);
        }

        [Fact]
        public async Task UpdateAsync_ChangesPersisted()
        {
            using var context = CreateInMemoryContext();
            var student = new Student { FullName = "Old Name", Email = "old@test.com", EnrollmentDate = DateTime.UtcNow };
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var repo = new StudentRepository(context);
            student.FullName = "New Name";
            await repo.UpdateAsync(student);

            var updated = await context.Students.FindAsync(student.Id);
            updated.FullName.ShouldBe("New Name");
        }

        [Fact]
        public async Task DeleteAsync_RemovesStudent()
        {
            using var context = CreateInMemoryContext();
            var student = new Student { FullName = "Delete Me", Email = "del@test.com", EnrollmentDate = DateTime.UtcNow };
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var repo = new StudentRepository(context);
            await repo.DeleteAsync(student.Id);

            var deleted = await context.Students.FindAsync(student.Id);
            deleted.ShouldBeNull();
        }

        [Fact]
        public async Task GetTopStudentsAsync_ReturnsOrderedByAverageGrade()
        {
            using var context = CreateInMemoryContext();
            var course1 = new Course { Title = "Math", Credits = 4 };
            var course2 = new Course { Title = "Science", Credits = 3 };

            var studentA = new Student
            {
                FullName = "Alice", Email = "alice@test.com", EnrollmentDate = DateTime.UtcNow,
                Enrollments = new List<Enrollment>
                {
                    new Enrollment { Course = course1, Grade = 70 },
                    new Enrollment { Course = course2, Grade = 80 }
                }
            };
            var studentB = new Student
            {
                FullName = "Bob", Email = "bob@test.com", EnrollmentDate = DateTime.UtcNow,
                Enrollments = new List<Enrollment>
                {
                    new Enrollment { Course = course1, Grade = 90 },
                    new Enrollment { Course = course2, Grade = 95 }
                }
            };

            context.Students.AddRange(studentA, studentB);
            await context.SaveChangesAsync();

            var repo = new StudentRepository(context);
            var top = await repo.GetTopStudentsAsync(1);

            top.Count.ShouldBe(1);
            top.First().FullName.ShouldBe("Bob");
        }
    }
}