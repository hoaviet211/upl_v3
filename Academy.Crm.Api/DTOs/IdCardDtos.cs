namespace Academy.Crm.Api.DTOs;

public record IdCardDto
{
    public int Id { get; init; }
    public int StudentId { get; init; }
    public string? CardNumber { get; init; }
    public DateOnly? DateOfIssue { get; init; }
    public string? PlaceOfIssue { get; init; }
}

