namespace CMapTest.Models
{
    public sealed class AuthUser
    {
        public required string Username { get; set; }
        public required byte[] Password { get; set; }
        public required int UserId { get; set; }
    }
}
