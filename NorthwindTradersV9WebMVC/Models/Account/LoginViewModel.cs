namespace NorthwindTradersV9WebMVC.Models.Account
{
    public class LoginViewModel
    {
        public string Usuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Recordarme { get; set; }
    }
}
