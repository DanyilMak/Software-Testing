using Microsoft.AspNetCore.Mvc;
using Lab5.Data;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly StudentRepository _repo;

    public StudentsController(StudentRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
        => Ok(await _repo.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(Student s)
    {
        await _repo.AddAsync(s);
        return Ok(s);
    }

    [HttpPut]
    public async Task<IActionResult> Update(Student s)
    {
        await _repo.UpdateAsync(s);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return Ok();
    }
}