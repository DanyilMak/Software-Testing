using Microsoft.AspNetCore.Mvc;

namespace Lab4.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateStudent([FromBody] CreateStudentRequest request)
        {
            return Ok(new { Message = "Student created successfully", Student = request });
        }
    }
}
