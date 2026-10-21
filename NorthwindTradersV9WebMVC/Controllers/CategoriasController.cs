using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Categorias;
using NorthwindTradersV9WebMVC.Models.Common;
using NorthwindTradersV9WebMVC.Models.Empleados;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoCategorias")]
    public class CategoriasController : Controller
    {
        private readonly CategoriaBLL _categoriaBLL;
        private readonly AppSettings _appSettings;
        public CategoriasController(CategoriaBLL categoriaBLL, IOptions<AppSettings> appSettings)
        {
            _categoriaBLL = categoriaBLL;
            _appSettings = appSettings.Value;
        }
        public IActionResult Index(
            int pageIndex = 1,
            CategoriasBuscarDto? filtro = null,
            bool BuscarAbierto = false)
        {
            filtro ??= new CategoriasBuscarDto();
            CategoriaPaginadoDto resultado;
            if (ModelState.IsValid)
            {
                resultado = _categoriaBLL.ObtenerCategoriasPaginadosConBusqueda(
                    pageIndex,
                    _appSettings.RowsPerPage,
                    filtro);
            }
            else
            {
                // Si el filtro es inválido, no ejecutamos BLL/DAL.
                resultado = new CategoriaPaginadoDto
                {
                    Categorias = new List<Categoria>(),
                    PageIndex = pageIndex,
                    TotalRegistros = 0
                };
            }
            var model = new CategoriasIndexViewModel
            {
                Categorias = resultado.Categorias,
                Filtro = filtro,
                Paginacion = new PaginacionViewModel
                {
                    PageIndex = resultado.PageIndex,
                    TotalRegistros = resultado.TotalRegistros,
                    PageSize = _appSettings.RowsPerPage
                },
                ParametrosPaginacion = new ParametrosPaginacionViewModel
                {
                    Controller = "Categorias",
                    Action = "Index",
                    BuscarAbierto = BuscarAbierto,
                    Parametros = new Dictionary<string, string?>
                    {
                        ["Filtro.IdIni"] = filtro.IdIni?.ToString(),
                        ["Filtro.IdFin"] = filtro.IdFin?.ToString(),
                        ["Filtro.CategoryName"] = filtro.CategoryName,
                        ["BuscarAbierto"] = BuscarAbierto.ToString().ToLower()
                    }
                }
            };
            return View(model);
        }
        public IActionResult Consultar(int id, string? returnUrl = null)
        {
            var categoria = _categoriaBLL.ObtenerCategoriaPorId(id);
            if (categoria == null)
                TempData["Error"] = "<p>Categoría no encontrada</p>" + StringsCommons.Nefep;
            var model = new CategoriasConsultarViewModel
            {
                Categoria = categoria,
                ReturnUrl = returnUrl
            };
            return View(model);
        }
        public IActionResult Eliminar(int id, string? returnUrl = null)
        {
            var categoria = _categoriaBLL.ObtenerCategoriaPorId(id);
            var model = new CategoriasEliminarViewModel
            {
                ReturnUrl = returnUrl
            };
            if (categoria == null)
            {
                TempData["Error"] = "<p>Categoría no encontrada</p>" + StringsCommons.Nefep;
                model.BloquearEliminacion = true;
            }
            else
                model.Categoria = categoria;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(CategoriasEliminarViewModel model)
        {
            if (model.Categoria != null)
            {
                var resultado = _categoriaBLL.Eliminar(model.Categoria);
                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);
                    return RedirectToAction("Index", "Categorias");
                }
                else
                {
                    TempData["Error"] =
                        $"<p>La categoría con Id: <strong>{model.Categoria.CategoryID}</strong> " +
                        $"- Nombre de categoría: <strong>{model.Categoria.CategoryName}</strong>:</p>" +
                        resultado.Mensaje;
                    // Sólo bloquea para errores definitivos
                    if (resultado.Codigo < 0)
                        model.BloquearEliminacion = true;
                }
            }
            return View(model);
        }
        public IActionResult Insertar(string? returnUrl = null)
        {
            var model = new CategoriasInsertarViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(CategoriasInsertarViewModel model)
        {
            byte[]? pictureBytes = null;
            // Procesar una imagen nueva, si se seleccionó.
            if (model.Picture != null && model.Picture.Length > 0)
            {
                // Validar el tamaño antes de leer el archivo.
                if (model.Picture.Length > ValidadorImagenes.MaximoImagenBytes)
                {
                    ModelState.AddModelError(
                        "Picture",
                        "La imagen no puede exceder 2 MB.");

                    model.PictureTemporalBase64 = null;
                    model.PictureMime = null;
                    return View(model);
                }
                using var ms = new MemoryStream();
                model.Picture.CopyTo(ms);
                pictureBytes = ms.ToArray();
                if (!ValidadorImagenes.EsImagenValida(pictureBytes, model.Picture.ContentType))
                {
                    ModelState.AddModelError(
                        "Picture",
                        "El archivo no es una imagen JPG, PNG, GIF o WEBP válida.");
                    model.PictureTemporalBase64 = null;
                    model.PictureMime = null;
                    return View(model);
                }
                model.PictureTemporalBase64 =
                    Convert.ToBase64String(pictureBytes);
                model.PictureMime = model.Picture.ContentType;
            }
            else if (!string.IsNullOrWhiteSpace(model.PictureTemporalBase64))
            {
                try
                {
                    pictureBytes = Convert.FromBase64String(
                        model.PictureTemporalBase64);
                    if (pictureBytes.Length == 0 ||
                        pictureBytes.Length > ValidadorImagenes.MaximoImagenBytes ||
                        !ValidadorImagenes.EsImagenValida(pictureBytes, model.PictureMime))
                    {
                        throw new FormatException();
                    }
                }
                catch (FormatException)
                {
                    ModelState.AddModelError(
                        "Picture",
                        "La imagen temporal no es válida o excede 2 MB.");
                    model.PictureTemporalBase64 = null;
                    model.PictureMime = null;
                    return View(model);
                }
            }
            // Validar el nombre antes de continuar.
            if (model.Categoria == null)
            {
                ModelState.AddModelError(
                    "Categoria",
                    "No se recibieron los datos de la categoría.");
            }
            else if (string.IsNullOrWhiteSpace(model.Categoria.CategoryName))
            {
                ModelState.AddModelError(
                    "Categoria.CategoryName",
                    "El nombre de la categoría es obligatorio.");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                if (model.Categoria != null)
                {
                    if (pictureBytes != null)
                        model.Categoria.Picture = pictureBytes;
                    else
                    {
                        var defaultImagePath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images",
                            "Categorias.png");
                        if (System.IO.File.Exists(defaultImagePath))
                            model.Categoria.Picture = System.IO.File.ReadAllBytes(defaultImagePath);
                    }
                    var resultado = _categoriaBLL.Insertar(model.Categoria);
                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                            return LocalRedirect(model.ReturnUrl);
                        return RedirectToAction("Index", "Categorias");
                    }
                    TempData["Error"] =
                        $"<p>La categoría <strong>{model.Categoria.CategoryName}</strong>:</p>" +
                        $"{resultado.Mensaje}";
                    if (resultado.Codigo < 0)
                        model.BloquearEdicion = true;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Ocurrió un error al insertar la categoría. " +
                    "Verifica los datos e inténtalo nuevamente.";
            }
            return View(model);
        }
    }
}
