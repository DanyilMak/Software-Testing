using System.ComponentModel.DataAnnotations;
namespace Lab5.Data;

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime EnrollmentDate { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}