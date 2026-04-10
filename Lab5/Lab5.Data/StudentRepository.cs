using Microsoft.EntityFrameworkCore;
namespace Lab5.Data;

public class StudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Students.ToListAsync();
    }

    public async Task AddAsync(Student student)
    {
        if (student.RowVersion == null || student.RowVersion.Length == 0)
        {
            student.RowVersion = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
        }

        _context.Students.Add(student);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        student.RowVersion = BitConverter.GetBytes(DateTime.UtcNow.Ticks);

        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Student>> GetTopStudentsAsync(int count)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
            .OrderByDescending(s => s.Enrollments.Average(e => e.Grade ?? 0))
            .Take(count)
            .ToListAsync();
    }
}