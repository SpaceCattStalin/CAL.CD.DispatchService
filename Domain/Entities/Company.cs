namespace Domain;

public class Company : BaseEntity
{
    public Guid CompanyId { get; init; }
    public string CompanyName { get; private set; }
    public string CompanyPhone { get; private set; }
    public string CompanyEmail { get; private set; }
    public CompanyType CompanyType { get; init; }
    public ICollection<User> Users { get; private set; }    
}
