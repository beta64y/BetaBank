namespace BetaBank.Services.Implementations
{
    public static class StatisticsService
    {
        public static List<string> GetLastMonths(int count)
        {
            List<string> lastFiveMonths = new List<string>();
            DateTime currentMonth = DateTime.Now;
            for (int i = 0; i < count ; i++)
            {
                lastFiveMonths.Add(currentMonth.AddMonths(-i).ToString("MMMM"));
            }
            lastFiveMonths.Reverse();
            return lastFiveMonths;
        }
    }
}
