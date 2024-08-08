namespace BetaBank.Services.Implementations
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
        
            public static string ToRelativeTime(this DateTime dateTime)
            {
                var timeSpan = DateTime.UtcNow.Subtract(dateTime);

                if (timeSpan <= TimeSpan.FromSeconds(60))
                    return "just now";

                if (timeSpan <= TimeSpan.FromMinutes(60))
                    return $"{timeSpan.Minutes} {(timeSpan.Minutes == 1 ? "minute" : "minutes")} ago";

                if (timeSpan <= TimeSpan.FromHours(24))
                    return $"{timeSpan.Hours} {(timeSpan.Hours == 1 ? "hour" : "hours")} ago";

                if (timeSpan <= TimeSpan.FromDays(30))
                    return $"{timeSpan.Days} {(timeSpan.Days == 1 ? "day" : "days")} ago";

                if (timeSpan <= TimeSpan.FromDays(365))
                {
                    int months = (int)Math.Floor((double)timeSpan.Days / 30);
                    return $"{months} {(months == 1 ? "month" : "months")} ago";
                }

                int years = (int)Math.Floor((double)timeSpan.Days / 365);
                return $"{years} {(years == 1 ? "year" : "years")} ago";
            }
        

    }

}
