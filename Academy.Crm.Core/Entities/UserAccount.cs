namespace Academy.Crm.Core.Entities;

public class UserAccount : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Student"; // Admin or Student
    public int? StudentId { get; set; }
    public Student? Student { get; set; }
}

