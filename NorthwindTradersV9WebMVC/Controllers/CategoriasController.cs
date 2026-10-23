using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Reporting.NETCore;
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
            var model = new CategoriasConsultarViewModel
            {
                ReturnUrl = returnUrl
            };
            if (categoria == null)
                TempData["Error"] = "<p>Categoría no encontrada</p>" + StringsCommons.Nefep;
            else
                model.Categoria = categoria;
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
        public IActionResult Editar(int id, string? returnUrl = null)
        {
            var model = new CategoriasEditarViewModel
            {
                ReturnUrl = returnUrl
            };
            var categoria = _categoriaBLL.ObtenerCategoriaPorId(id);
            if (categoria == null)
            {
                TempData["Error"] = "<p>Categoría no encontrada.</p>" + StringsCommons.Nefep;
                model.BloquearEdicion = true;
            }
            else
            {
                // Preparar la imagen almacenada para mostrarla en la vista.
                PrepararImagenTemporal(model, categoria.Picture);
            }
            model.Categoria = categoria;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(CategoriasEditarViewModel model)
        {
            if (model.Categoria == null)
            {
                TempData["Error"] =
                    "No se recibieron los datos de la categoría.";
                return RedirectToAction("Index", "Categorias");
            }
            int id = model.Categoria.CategoryID;
            // Recuperar la categoría actual para conservar su imagen.
            var categoriaActual = _categoriaBLL.ObtenerCategoriaPorId(id);
            if (categoriaActual == null)
            {
                TempData["Error"] = "No se encontró la categoría que deseas editar.";
                return RedirectToAction("Index", "Categorias");
            }

            byte[]? pictureBytes = null;
            // Las imágenes de las categorías 1 a 8 están protegidas.
            if (id >= 1 && id <= 8)
            {
                pictureBytes = categoriaActual.Picture;
            }
            else if (model.Picture != null && model.Picture.Length > 0)
            {
                if (model.Picture.Length > ValidadorImagenes.MaximoImagenBytes)
                {
                    ModelState.AddModelError(
                        "Picture",
                        "La imagen no puede exceder 2 MB.");
                    PrepararImagenTemporal(model, categoriaActual.Picture);
                    return View(model);
                }
                using var ms = new MemoryStream();
                model.Picture.CopyTo(ms);
                pictureBytes = ms.ToArray();
                if (!ValidadorImagenes.EsImagenValida(
                        pictureBytes,
                        model.Picture.ContentType))
                {
                    ModelState.AddModelError(
                        "Picture",
                        "El archivo no es una imagen JPG, PNG, GIF o WEBP válida.");
                    PrepararImagenTemporal(model, categoriaActual.Picture);
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
                    pictureBytes =
                        Convert.FromBase64String(model.PictureTemporalBase64);
                    if (pictureBytes.Length == 0 ||
                        pictureBytes.Length > ValidadorImagenes.MaximoImagenBytes ||
                        !ValidadorImagenes.EsImagenValida(
                            pictureBytes,
                            model.PictureMime))
                    {
                        throw new FormatException();
                    }
                }
                catch (FormatException)
                {
                    ModelState.AddModelError(
                        "Picture",
                        "La imagen temporal no es válida o excede 2 MB.");
                    PrepararImagenTemporal(model, categoriaActual.Picture);
                    return View(model);
                }
            }
            else
            {
                pictureBytes = categoriaActual.Picture;
                PrepararImagenTemporal(model, categoriaActual.Picture);
            }
            // Validar los datos de la categoría.
            if (string.IsNullOrWhiteSpace(model.Categoria.CategoryName))
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
                model.Categoria.Picture = pictureBytes;
                var resultado = _categoriaBLL.Actualizar(model.Categoria);
                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);
                    return RedirectToAction("Index", "Categorias");
                }
                TempData["Error"] =
                    $"<p>La categoría con Id: <strong>{model.Categoria.CategoryID}</strong> - Nombre: <strong>{model.Categoria.CategoryName}</strong>:</p> " +
                    $"{resultado.Mensaje}";
                if (resultado.Codigo < 0)
                    model.BloquearEdicion = true;
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Ocurrió un error al actualizar la categoría. " +
                    "Verifica los datos e inténtalo nuevamente.";
            }
            return View(model);
        }
        private static void PrepararImagenTemporal(
            CategoriasEditarViewModel model,
            byte[]? pictureBytes)
        {
            string? mime = ObtenerMimeImagen(pictureBytes);
            if (pictureBytes != null && mime != null)
            {
                model.PictureTemporalBase64 =
                    Convert.ToBase64String(pictureBytes);
                model.PictureMime = mime;
            }
            else
            {
                model.PictureTemporalBase64 = null;
                model.PictureMime = null;
            }
        }
        private static string? ObtenerMimeImagen(byte[]? bytes)
        {
            if (bytes == null || bytes.Length < 12)
                return null;
            // Imagen BMP original de Northwind.
            // La firma BMP comienza con los bytes 0x42 y 0x4D ("BM").
            if (bytes[0] == 0x42 && bytes[1] == 0x4D)
                return "image/bmp"; 
            if (ValidadorImagenes.EsImagenValida(bytes, "image/jpeg"))
                return "image/jpeg";
            if (ValidadorImagenes.EsImagenValida(bytes, "image/png"))
                return "image/png";
            if (ValidadorImagenes.EsImagenValida(bytes, "image/gif"))
                return "image/gif";
            if (ValidadorImagenes.EsImagenValida(bytes, "image/webp"))
                return "image/webp";
            return null;
        }
        public IActionResult RptCategorias()
        {
            return View();
        }
        private LocalReport CrearRptCategorias()
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Categorias",
                "RptCategorias.rdlc");
            var categorias = _categoriaBLL.ObtenerCategoriasRpt();
            reporte.DataSources.Clear();
            reporte.DataSources.Add(new ReportDataSource("DataSet1", categorias));
            return reporte;
        }
        public IActionResult RptCategoriasPdf()
        {
            var reporte = CrearRptCategorias();
            byte[] pdf = reporte.Render("PDF");
            return File(pdf, "application/pdf");
        }
        public IActionResult RptCategoriasExcel()
        {
            var reporte = CrearRptCategorias();
            byte[] archivo = reporte.Render("EXCELOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Categorias.xlsx");
        }
        public IActionResult RptCategoriasWord()
        {
            var reporte = CrearRptCategorias();
            byte[] archivo = reporte.Render("WORDOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Categorias.docx");
        }

    }
}
