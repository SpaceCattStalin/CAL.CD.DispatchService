namespace Infrastructure;

/// <summary>
/// Seed data for dev/demo purposes: 3 Carrier companies and 2 Shipper companies, each with
/// one Owner and two Driver users. Password is "Password123!" (same hash as TestUserSeedIds).
/// </summary>
internal static class DemoDataSeedIds
{
    public static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid ApexCarriersId = new("50000000-0000-0000-0000-000000000001");
    public static readonly Guid BlueHorizonId = new("50000000-0000-0000-0000-000000000002");
    public static readonly Guid MidwestFreightId = new("50000000-0000-0000-0000-000000000003");
    public static readonly Guid GoldenStateId = new("50000000-0000-0000-0000-000000000004");
    public static readonly Guid SummitRetailId = new("50000000-0000-0000-0000-000000000005");

    public static readonly Guid ApexCarriersOwnerId = new("51000000-0000-0000-0000-000000000001");
    public static readonly Guid ApexCarriersDriver1Id = new("52000000-0000-0000-0000-000000000001");
    public static readonly Guid ApexCarriersDriver2Id = new("53000000-0000-0000-0000-000000000001");

    public static readonly Guid BlueHorizonOwnerId = new("51000000-0000-0000-0000-000000000002");
    public static readonly Guid BlueHorizonDriver1Id = new("52000000-0000-0000-0000-000000000002");
    public static readonly Guid BlueHorizonDriver2Id = new("53000000-0000-0000-0000-000000000002");

    public static readonly Guid MidwestFreightOwnerId = new("51000000-0000-0000-0000-000000000003");
    public static readonly Guid MidwestFreightDriver1Id = new("52000000-0000-0000-0000-000000000003");
    public static readonly Guid MidwestFreightDriver2Id = new("53000000-0000-0000-0000-000000000003");

    public static readonly Guid GoldenStateOwnerId = new("51000000-0000-0000-0000-000000000004");
    public static readonly Guid GoldenStateDriver1Id = new("52000000-0000-0000-0000-000000000004");
    public static readonly Guid GoldenStateDriver2Id = new("53000000-0000-0000-0000-000000000004");

    public static readonly Guid SummitRetailOwnerId = new("51000000-0000-0000-0000-000000000005");
    public static readonly Guid SummitRetailDriver1Id = new("52000000-0000-0000-0000-000000000005");
    public static readonly Guid SummitRetailDriver2Id = new("53000000-0000-0000-0000-000000000005");
}
