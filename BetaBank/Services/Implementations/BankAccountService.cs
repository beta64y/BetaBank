using System.Text;

namespace BetaBank.Services.Implementations
{
    public static class BankAccountService
    {
        private static Random _random = new Random();

        public static string GenerateIBAN(string countryCode, string bankCode, string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
                throw new ArgumentException("Country code must be 2 characters.");

            if (string.IsNullOrWhiteSpace(bankCode))
                throw new ArgumentException("Bank code cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number cannot be null or empty.");

            string checkDigits = "00"; 
            string bban = bankCode + accountNumber;

            string interimIBAN = countryCode + checkDigits + bban;
            string numericIBAN = ConvertToNumericString(interimIBAN);

            int checksum = 98 - (Mod97(numericIBAN));
            checkDigits = checksum.ToString("00");

            string finalIBAN = countryCode + checkDigits + bban;
            return finalIBAN;
        }

        public static string GenerateSWIFT(string bankCode, string countryCode, string locationCode = "FF", string branchCode = "XXX")
        {
            if (string.IsNullOrWhiteSpace(bankCode) || bankCode.Length != 4)
                throw new ArgumentException("Bank code must be 4 characters.");

            if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
                throw new ArgumentException("Country code must be 2 characters.");

            if (string.IsNullOrWhiteSpace(locationCode) || locationCode.Length != 2)
                throw new ArgumentException("Location code must be 2 characters.");

            if (branchCode.Length != 3)
                throw new ArgumentException("Branch code must be 3 characters.");

            return $"{bankCode}{countryCode}{locationCode}{branchCode}";
        }
        public static string GenerateAccountNumber()
        {
            Random random = new Random();
            string accountNumber = "";
            for (int i = 0; i < 10; i++)
            {
                accountNumber += random.Next(0, 10).ToString();
            }
            return accountNumber;
        }

        private static string ConvertToNumericString(string input)
        {
            StringBuilder numericString = new StringBuilder();

            foreach (char ch in input)
            {
                if (char.IsLetter(ch))
                {
                    int numericValue = ch - 'A' + 10;
                    numericString.Append(numericValue);
                }
                else if (char.IsDigit(ch))
                {
                    numericString.Append(ch);
                }
            }

            return numericString.ToString();
        }

        private static int Mod97(string input)
        {
            int remainder = 0;
            for (int i = 0; i < input.Length; i++)
            {
                int digit = input[i] - '0';
                remainder = (remainder * 10 + digit) % 97;
            }
            return remainder;
        }
    }
}
