using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;
using NorthwindTradersV9WebMVC.Models.Proveedores;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoProveedores")]
    public class ProveedoresController : Controller
    {
        private readonly ProveedorBLL _proveedorBLL;
        private readonly AppSettings _appSettings;
        public ProveedoresController(
            ProveedorBLL proveedorBLL,
            IOptions<AppSettings> appSettings)
        {
            _proveedorBLL = proveedorBLL;
            _appSettings = appSettings.Value;
        }
        public IActionResult Index(
            int pageIndex = 1,
            ProveedoresBuscarDto? filtro = null,
            bool BuscarAbierto = false)
        {
            filtro ??= new ProveedoresBuscarDto();
            // Obtener los países para llenar el combo.
            var paises = _proveedorBLL.ObtenerProveedoresPaisesCbo()
                .Select(p => new SelectListItem
                {
                    Value = p.Value,
                    Text = p.Text,
                    Selected = p.Value == filtro.Country
                })
                .ToList();
            ProveedorPaginadoDto resultado;
            if (ModelState.IsValid)
            {
                // Solamente buscamos proveedores cuando el filtro es válido.
                resultado = _proveedorBLL.ObtenerProveedoresPaginadosConBusqueda(
                    pageIndex,
                    _appSettings.RowsPerPage,
                    filtro);
            }
            else
            {
                // Si el filtro es inválido, no ejecutamos BLL/DAL.
                resultado = new ProveedorPaginadoDto
                {
                    Proveedores = new List<Proveedor>(),
                    PageIndex = pageIndex,
                    TotalRegistros = 0
                };
            }
            var model = new ProveedoresIndexViewModel
            {
                Proveedores = resultado.Proveedores,
                Filtro = filtro,
                Paises = paises,
                Paginacion = new PaginacionViewModel
                {
                    PageIndex = resultado.PageIndex,
                    TotalRegistros = resultado.TotalRegistros,
                    PageSize = _appSettings.RowsPerPage
                },
                ParametrosPaginacion = new ParametrosPaginacionViewModel
                {
                    Controller = "Proveedores",
                    Action = "Index",
                    BuscarAbierto = BuscarAbierto,
                    Parametros = new Dictionary<string, string?>
                    {
                        ["Filtro.IdIni"] = filtro.IdIni?.ToString(),
                        ["Filtro.IdFin"] = filtro.IdFin?.ToString(),
                        ["Filtro.CompanyName"] = filtro.CompanyName,
                        ["Filtro.ContactName"] = filtro.ContactName,
                        ["Filtro.Address"] = filtro.Address,
                        ["Filtro.City"] = filtro.City,
                        ["Filtro.Region"] = filtro.Region,
                        ["Filtro.PostalCode"] = filtro.PostalCode,
                        ["Filtro.Country"] = filtro.Country,
                        ["Filtro.Phone"] = filtro.Phone,
                        ["Filtro.Fax"] = filtro.Fax,
                        ["BuscarAbierto"] = BuscarAbierto.ToString().ToLower()
                    }
                }
            };
            return View(model);
        }
    }
}
