using System.Security.Cryptography;
namespace BetaBank.Services.Implementations
{
    public static class EmployeesService
    {
        private static readonly char[] _availableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+[]{}|;:,.<>?".ToCharArray();
        private static readonly int _passwordLength = 12;

        public static string GeneratePassword()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var byteArray = new byte[_passwordLength];
                rng.GetBytes(byteArray);

                var charArray = new char[_passwordLength];
                for (int i = 0; i < _passwordLength; i++)
                {
                    charArray[i] = _availableChars[byteArray[i] % _availableChars.Length];
                }

                return new string(charArray);
            }
        }
    }
}
