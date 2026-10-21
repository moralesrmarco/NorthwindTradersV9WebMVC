namespace NorthwindTradersV9Common
{
    public static class ValidadorImagenes
    {
        public const int MaximoImagenBytes = 2 * 1024 * 1024;
        public static bool EsImagenValida(
            byte[] bytes,
            string? contentType)
        {
            if (bytes == null || bytes.Length < 12)
                return false;

            return contentType switch
            {
                "image/jpeg" =>
                    bytes[0] == 0xFF &&
                    bytes[1] == 0xD8 &&
                    bytes[2] == 0xFF,

                "image/png" =>
                    bytes[0] == 0x89 &&
                    bytes[1] == 0x50 &&
                    bytes[2] == 0x4E &&
                    bytes[3] == 0x47 &&
                    bytes[4] == 0x0D &&
                    bytes[5] == 0x0A &&
                    bytes[6] == 0x1A &&
                    bytes[7] == 0x0A,

                "image/gif" =>
                    bytes[0] == 0x47 &&
                    bytes[1] == 0x49 &&
                    bytes[2] == 0x46 &&
                    bytes[3] == 0x38 &&
                    (bytes[4] == 0x37 || bytes[4] == 0x39) &&
                    bytes[5] == 0x61,

                "image/webp" =>
                    bytes[0] == 0x52 &&
                    bytes[1] == 0x49 &&
                    bytes[2] == 0x46 &&
                    bytes[3] == 0x46 &&
                    bytes[8] == 0x57 &&
                    bytes[9] == 0x45 &&
                    bytes[10] == 0x42 &&
                    bytes[11] == 0x50,

                _ => false
            };
        }
    }
}
