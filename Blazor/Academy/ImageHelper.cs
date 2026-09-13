namespace Academy
{
    public static class ImageHelper
    {
        public static string? ToBase64ImageSrc(byte[]? bytes, string? mimeType)
        {
            if (bytes == null || mimeType == null) return null;

            string? mime = string.IsNullOrWhiteSpace(mimeType) ? "image/jpeg" : mimeType;
            string? base64 = Convert.ToBase64String(bytes);
            return $"data:{mimeType};base64,{base64}";
        }
    }
}
