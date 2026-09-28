using System.Security.Cryptography;
using System.Text;

namespace NorthwindTradersV9Common
{
    public static class PasswordHelper
    {
        public static string GenerarHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(
                    "La contraseña no puede estar vacía.",
                    nameof(password));
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            StringBuilder resultado = new();
            foreach (byte b in hashBytes)
                resultado.Append(b.ToString("x2"));
            return resultado.ToString();
        }
    }
}
