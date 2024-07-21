namespace BetaBank.Services.Implementations
{
    public static class ReceiptNumberGenerator 
    {
        private static readonly Random _random = new Random();

        public static string GenerateReceiptNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var randomPart = _random.Next(1000, 9999); 
            return $"{timestamp}{randomPart}";
        }
    }
}
