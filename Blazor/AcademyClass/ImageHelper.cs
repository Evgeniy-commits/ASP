namespace AcademyClass
{
    public static class ImageHelper
    {
        public static string? ToBase64ImageSrc(byte[]? bytes)
        {
            if (bytes == null ) return null;

            var base64 = Convert.ToBase64String(bytes);
            return $"base64,{base64}";
        }
    }
    public static class WeekDayHelper
    {
        public static readonly Dictionary<int, string> Days = new()
        {
            { 1, "Пн" },
            { 2, "Вт" },
            { 4, "Ср" },
            { 8, "Чт" },
            { 16, "Пт" },
            { 32, "Сб" },
            { 64, "Вс" }
        };

        public static bool IsDaySelected(int? mask, int dayValue)
        {
            return mask.HasValue && (mask.Value & dayValue) == dayValue;
        }

        public static string GetSelectedDaysString(int? mask)
        {
            if (!mask.HasValue || mask == 0) return "Не указано";
            var list = new List<string>();
            foreach (var d in Days)
            {
                if ((mask.Value & d.Key) == d.Key) list.Add(d.Value);
            }
            return string.Join(", ", list);
        }
    }
}
