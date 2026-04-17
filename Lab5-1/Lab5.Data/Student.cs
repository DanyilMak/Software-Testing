namespace Lab5.Data;
public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;   // default init
    public string Email { get; set; } = string.Empty;      // default init
    public DateTime EnrollmentDate { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
