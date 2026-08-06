namespace Domain;

public class User : BaseEntity
{
    public Guid UserId { get; init; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }    
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public string UserName { get; private set; }
    public string PasswordHash { get; init; }
    public bool IsActive { get; private set; }
    public UserRole UserRole { get; init; }
    public Company Company { get; init; }
    public Guid CompanyId { get; init; }
}
