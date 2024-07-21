namespace BetaBank.Services.Implementations
{
    public static class CashBackService
    {
        public static string GenerateNumber()
        {
            Random random = new Random();
            string Number = "";
            for (int i = 0; i < 15; i++)
            {
                Number += random.Next(0, 10).ToString();
            }
            return Number;
        }
    }
}
