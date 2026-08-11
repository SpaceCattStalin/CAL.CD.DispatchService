namespace Infrastructure;

/// <summary>
/// Seed data for a single login-testable account. Dev/test convenience only — not meant to ship as
/// real production data. Password is "Password123!".
/// </summary>
internal static class TestUserSeedIds
{
    public static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid CompanyId = new("30000000-0000-0000-0000-000000000001");
    public static readonly Guid UserId = new("30000000-0000-0000-0000-000000000002");

    public const string PasswordHash = "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==";
}
