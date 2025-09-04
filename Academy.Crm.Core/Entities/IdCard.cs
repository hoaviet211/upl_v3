namespace Academy.Crm.Core.Entities;

public class IdCard : BaseEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public string? CardNumber { get; set; }
    public DateOnly? DateOfIssue { get; set; }
    public string? PlaceOfIssue { get; set; }
}

