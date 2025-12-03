namespace SRP.API.Helper.Helpers
{
    public static class DateHelper
    {
        public static DateTime GetCurrentAcademicYearStart()
        {
            var today = DateTime.UtcNow;
            var year = today.Month >= 7 ? today.Year : today.Year - 1;
            return new DateTime(year, 7, 1);
        }

        public static DateTime GetCurrentAcademicYearEnd()
        {
            var start = GetCurrentAcademicYearStart();
            return start.AddYears(1).AddDays(-1);
        }

        public static string GetAcademicYearString(DateTime date)
        {
            var year = date.Month >= 7 ? date.Year : date.Year - 1;
            return $"{year}-{year + 1}";
        }

        public static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        public static bool IsDateInRange(DateTime date, DateTime start, DateTime end)
        {
            return date >= start && date <= end;
        }
    }
}
