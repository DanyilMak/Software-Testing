using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StudentsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        await Task.Delay(50);
        return Ok(await _db.Students.Take(100).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudent(int id)
    {
        await Task.Delay(20);

        var student = await _db.Students.FindAsync(id);
        return student == null ? NotFound() : Ok(student);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string q)
    {
        await Task.Delay(200);

        var result = await _db.Students
            .Where(s => s.Name.Contains(q))
            .ToListAsync();

        return Ok(result);
    }
}