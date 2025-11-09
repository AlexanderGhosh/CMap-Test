namespace CMapTest.Models
{
    public sealed class UserClaim
    {
        public required int UserId { get; init; }
        public required string Type { get; init; }
        public required string Value { get; init; }
    }
}
