using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Categorias
{
    public class CategoriasEditarViewModel
    {
        public Categoria? Categoria { get; set; } = new();
        public IFormFile? Picture { get; set; }
        public string? PictureTemporalBase64 { get; set; }
        public string? PictureMime { get; set; }
        public bool BloquearEdicion { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
