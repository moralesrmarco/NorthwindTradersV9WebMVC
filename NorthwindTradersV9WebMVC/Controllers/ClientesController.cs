using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Clientes;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ClienteBLL _clienteBLL;
        private readonly AppSettings _appSettings;

        public ClientesController(
            ClienteBLL clienteBLL,
            IOptions<AppSettings> appSettings)
        {
            _clienteBLL = clienteBLL;
            _appSettings = appSettings.Value;
        }

        public IActionResult Index(
            int pageIndex = 1,
            ClientesBuscarDto? filtro = null,
            bool BuscarAbierto = false)
        {
            int pageSize = _appSettings.RowsPerPage;

            filtro ??= new ClientesBuscarDto();

            // Obtener los países para llenar el combo.
            var paises = _clienteBLL.ObtenerClientesPaisesCbo()
                .Select(p => new SelectListItem
                {
                    Value = p.Value,
                    Text = p.Text,
                    Selected = p.Value == filtro.Country
                })
                .ToList();

            ClientePaginadoDto resultado;

            if (ModelState.IsValid)
            {
                // Solamente buscamos clientes cuando el filtro es válido.
                resultado =
                    _clienteBLL.ObtenerClientesPaginadosConBusqueda(
                        pageIndex,
                        pageSize,
                        filtro);
            }
            else
            {
                // Si el filtro es inválido, no ejecutamos BLL/DAL.
                resultado = new ClientePaginadoDto
                {
                    Clientes = new List<Cliente>(),
                    PageIndex = pageIndex,
                    TotalRegistros = 0
                };
            }

            var model = new ClientesIndexViewModel
            {
                Clientes = resultado.Clientes,

                Filtro = filtro,

                Paises = paises,

                Paginacion = new PaginacionViewModel
                {
                    PageIndex = resultado.PageIndex,
                    PageSize = pageSize,
                    TotalRegistros = resultado.TotalRegistros
                },

                ParametrosPaginacion = new ParametrosPaginacionViewModel
                {
                    Controller = "Clientes",
                    Action = "Index",
                    BuscarAbierto = BuscarAbierto,
                    Parametros = new Dictionary<string, string?>
                    {
                        ["Filtro.CustomerID"] = filtro.CustomerID,
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