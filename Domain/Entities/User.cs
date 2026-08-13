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

    /// <summary>
    /// Factory method to create an instance of User
    /// </summary>
    /// <param name="companyId">Id of the Company this user belongs to</param>
    /// <param name="firstName">User's first name</param>
    /// <param name="lastName">User's last name</param>
    /// <param name="phone">User's phone number</param>
    /// <param name="email">User's email address</param>
    /// <param name="userName">User's login name</param>
    /// <param name="passwordHash">Hashed password</param>
    /// <param name="userRole">Role of the user</param>
    /// <param name="isActive">Whether the user is active, defaults to true</param>
    /// <param name="company">Optional Company navigation property</param>
    /// <returns>A new User instance</returns>
    /// <exception cref="ArgumentException">A required field is missing</exception>
    public static User CreateUser(Guid companyId, string firstName, string lastName, string phone,
        string email, string userName, string passwordHash, UserRole userRole,
        bool isActive = true, Company? company = null)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId is required", nameof(companyId));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName is required", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName is required", nameof(lastName));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required", nameof(phone));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("UserName is required", nameof(userName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash is required", nameof(passwordHash));

        return new User
        {
            UserId = Guid.NewGuid(),
            CompanyId = companyId,
            Company = company!,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            Email = email,
            UserName = userName,
            PasswordHash = passwordHash,
            UserRole = userRole,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }
}
