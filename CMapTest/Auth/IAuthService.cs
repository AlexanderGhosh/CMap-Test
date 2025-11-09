namespace CMapTest.Auth
{
    public interface IAuthService
    {
        Task<bool> VerifyPassword(string plainPassword, byte[] exceptedHash, CancellationToken cancellationToken);
        Task<PasswordStrengthResult> IsPasswordStrongEnough(string plainPassword, CancellationToken cancellationToken);
        Task<byte[]> GeneratePasswordHash(string plainPassword, CancellationToken cancellationToken);
    }
    public sealed class PasswordStrengthResult
    {
        private PasswordStrengthResult(bool passed, string? failedReason = null)
        {
            IsAdequate = passed;
            FailedReason = failedReason;
        }

        public bool IsAdequate { get; }
        public string? FailedReason => IsAdequate ? throw new InvalidOperationException($"Cannot get {nameof(FailedReason)} because it didn't fail") : field;
        public static PasswordStrengthResult Pass() => new(true);
        public static PasswordStrengthResult Fail(string? why) => new(false, why);
    }
}
