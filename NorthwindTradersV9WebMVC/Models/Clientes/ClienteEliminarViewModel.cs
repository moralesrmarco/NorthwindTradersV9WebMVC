using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Clientes
{
    public class ClienteEliminarViewModel
    {
        public Cliente? Cliente { get; set; } = new();
        public string? ReturnUrl { get; set; }
        public bool BloquearEliminacion { get; set; }
    }
}
