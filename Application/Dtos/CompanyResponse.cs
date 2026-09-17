namespace Application;

public class CompanyResponse(
    Guid CompanyId,
    string CompanyName,
    string CompanyPhone,
    string CompanyEmail
    )
{
    public Guid CompanyId { get; init; } = CompanyId;
    public string CompanyName { get; init; } = CompanyName;
    public string CompanyPhone { get; init; } = CompanyPhone;
    public string CompanyEmail { get; init; } = CompanyEmail;
}