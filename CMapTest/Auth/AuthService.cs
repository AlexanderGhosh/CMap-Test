using CMapTest.Config;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace CMapTest.Auth
{
    public sealed class AuthService(IOptionsMonitor<AuthOptions> options) : IAuthService, IDisposable
    {
        // i know this defaults to the named option "". I dont think its likely that a dev would add multiple PasswordOptions options and at some point its not worth accounting for dev error
        private AuthOptions _config => options.CurrentValue;
        private readonly HashAlgorithm _hasher = MD5.Create();
        // at work i wrote a similar function to return a list of all the errors but i felt that it was a valid compromise to not do that so i can get this project done faster
        // it would take more time to manage multiple fail reasons at once because i would have to make a component to present them nicely which can be a can of worms
        public Task<PasswordStrengthResult> IsPasswordStrongEnough(string plainPassword, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            PasswordStrengthResult res = PasswordStrengthResult.Pass();
            if (!_config.PasswordLength.InRange(plainPassword.Length))
                res = PasswordStrengthResult.Fail($"Your password's length must be in the range: {_config.PasswordLength}");
            else if (!_config.PasswordLowerCase.InRange(plainPassword.Count(char.IsLower)))
                res = PasswordStrengthResult.Fail($"Your password's lower case characters must be in the range: {_config.PasswordLowerCase}");
            else if (!_config.PasswordUpperCase.InRange(plainPassword.Count(char.IsUpper)))
                res = PasswordStrengthResult.Fail($"Your password's upper case characters must be in the range: {_config.PasswordUpperCase}");
            else if (!_config.PasswordDigits.InRange(plainPassword.Count(char.IsDigit)))
                res = PasswordStrengthResult.Fail($"Your password must contains a number of digits in the range: {_config.PasswordDigits}");
            else if (!_config.AllowNonAlphaNumeric && plainPassword.All(char.IsLetterOrDigit))
                res = PasswordStrengthResult.Fail($"Your password must only contains a letters or numerical digits");
            return Task.FromResult(res);
        }

        public Task<bool> VerifyPassword(string plainPassword, byte[] exceptedHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            byte[] salt = new byte[_config.SaltLength];
            Buffer.BlockCopy(exceptedHash, 0, salt, 0, _config.SaltLength);
            byte[] genHash = hashText(plainPassword, salt);
            return Task.FromResult(bitWiseComparison(exceptedHash, genHash));
        }

        public Task<byte[]> GeneratePasswordHash(string plainPassword, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            byte[] password = getPasswordBytes(plainPassword);
            byte[] salt = getSalt();
            byte[] genHash = generateSecureBytes(password, salt);
            return Task.FromResult(genHash);
        }

        private byte[] getPasswordBytes(string plainText) => Encoding.UTF8.GetBytes(plainText);

        private byte[] hashText(string plain, byte[] salt)
        {
            byte[] plainBytes = getPasswordBytes(plain);
            return generateSecureBytes(plainBytes, salt);
        }

        private byte[] generateSecureBytes(byte[] data, byte[] salt)
        {
            byte[] saltData = new byte[salt.Length + data.Length];
            Buffer.BlockCopy(salt, 0, saltData, 0, salt.Length);
            Buffer.BlockCopy(data, 0, saltData, salt.Length, data.Length);
            // saltData = [..salt, ..data]
            byte[] saltDataHash = _hasher.ComputeHash(saltData);
            byte[] saltHash = new byte[salt.Length + saltDataHash.Length];
            Buffer.BlockCopy(salt, 0, saltHash, 0, salt.Length);
            Buffer.BlockCopy(saltDataHash, 0, saltHash, salt.Length, saltDataHash.Length);
            // [..salt, ..hash([..salt, ..data])]
            return saltHash;
        }

        private byte[] getSalt()
        {
            byte[] salt = new byte[_config.SaltLength];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        private bool bitWiseComparison(byte[] a, byte[] b)
        {
            return CryptographicOperations.FixedTimeEquals(a, b);
            // made this func before i knew about CryptographicOperations.FixedTimeEquals
            //int maxSize = Math.Max(a.Length, b.Length);
            //byte[] x = new byte[maxSize];
            //byte[] y = new byte[maxSize];
            //int z = 0;
            //// doesn't have explicit early out so the attacker cant read much into it
            //for (int i = 0; i < a.Length && i < b.Length; i++)
            //{
            //    z |= a[i] ^ b[i]; // are values in the same place
            //}
            //return z == 0;
        }

        public void Dispose() => _hasher.Dispose();
    }
}
