namespace BetaBank.Services.Implementations
{
    public static class BankCardService
    {
        private static Random random = new Random();

        public static string GenerateCardNumber()
        {
            const string bin = "644264"; 
            string accountNumber = string.Concat(Enumerable.Range(0, 9).Select(_ => random.Next(0, 10).ToString()));
            string cardNumberWithoutCheckDigit = bin + accountNumber;
            string checkDigit = CalculateLuhnCheckDigit(cardNumberWithoutCheckDigit);

            return cardNumberWithoutCheckDigit + checkDigit;
        }

        public static string GenerateCVV()
        {
            return string.Concat(Enumerable.Range(0, 3).Select(_ => random.Next(0, 10).ToString()));
        }

        public static DateTime GenerateExpiryDate()
        {
            return DateTime.Now.AddYears(random.Next(3, 6)); // 3 ila 5 yıl arasında geçerlilik süresi
        }

        private static string CalculateLuhnCheckDigit(string number)
        {
            int sum = 0;
            bool alternate = false;

            for (int i = number.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(number[i].ToString());

                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                    {
                        n -= 9;
                    }
                }

                sum += n;
                alternate = !alternate;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            return checkDigit.ToString();
        }
    }
}
