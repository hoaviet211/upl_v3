namespace Academy.Crm.Api.DTOs;

public record StudentDto
{
    public int Id { get; init; }
    public string StudentCode { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? FullName { get; init; }
    public string? NickName { get; init; }
    public string? ShortInfo { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string? ZaloNumber { get; init; }
    public int? YearOfBirth { get; init; }
    public DateOnly? Birthday { get; init; }
    public string? ImagePath { get; init; }
    public string? Email { get; init; }
}

