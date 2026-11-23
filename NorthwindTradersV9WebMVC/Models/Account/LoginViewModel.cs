namespace NorthwindTradersV9WebMVC.Models.Account
{
    public class LoginViewModel
    {
        public string Usuario { get; set; } = "Guest";
        public string Password { get; set; } = "123456";
        public bool Recordarme { get; set; }
    }
}
