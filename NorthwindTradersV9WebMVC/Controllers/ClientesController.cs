using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Clientes;
using NorthwindTradersV9WebMVC.Models.Common;
using NorthwindTradersV9WebMVC.Models.Empleados;

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
        public IActionResult Consultar(string id, string? returnUrl = null)
        {
            var cliente = _clienteBLL.ObtenerClientePorId(id);
            if (cliente == null)
                TempData["Error"] = "<p>Cliente no encontrado</p>" + StringsCommons.Nefep;
            var model = new ClienteConsultarViewModel
            {
                Cliente = cliente,
                ReturnUrl = returnUrl
            };
            return View(model);
        }
        public IActionResult Eliminar(string id, string? returnUrl = null)
        {
            var cliente = _clienteBLL.ObtenerClientePorId(id);
            var model = new ClienteEliminarViewModel
            {
                ReturnUrl = returnUrl
            };
            if (cliente == null)
            {
                TempData["Error"] = "<p>Cliente no encontrado</p>" + StringsCommons.Nefep;
                model.BloquearEliminacion = true;
            }
            else
                model.Cliente = cliente;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(ClienteEliminarViewModel model)
        {
            if (model.Cliente != null)
            {
                var resultado = _clienteBLL.Eliminar(model.Cliente);
                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);
                    return RedirectToAction("Index", "Clientes");
                }
                else
                {
                    TempData["Error"] =
                        $"<p>El cliente con Id: <strong>{model.Cliente.CustomerId}</strong> " +
                        $"- Nombre de compañía: <strong>{model.Cliente.CompanyName}</strong>:</p>" +
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
            var model = new ClienteInsertarViewModel
            {
                ReturnUrl = returnUrl
            };
            model.Paises = ObtenerPaises(model.Cliente?.Country);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(ClienteInsertarViewModel model)
        {
            // Validaciones en el servidor
            if (string.IsNullOrEmpty(model.Cliente?.Country))
                ModelState.AddModelError(
                    "Cliente.Country",
                    "Seleccione o escriba un país");
            if (!ModelState.IsValid)
            {
                model.Paises = ObtenerPaises(model.Cliente?.Country);
                return View(model);
            }
            // Validar ID duplicado
            if (model.Cliente != null && _clienteBLL.ExisteCliente(model.Cliente.CustomerId))
            {
                ModelState.AddModelError(
                    "Cliente.CustomerId",
                    $"El ID del cliente {model.Cliente.CustomerId} ya existe. Proporcione un nuevo ID");
                model.Paises = ObtenerPaises(model.Cliente?.Country);
                return View(model);
            }
            try
            {
                if (model.Cliente != null)
                {
                    var resultado = _clienteBLL.Insertar(model.Cliente);
                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                            return LocalRedirect(model.ReturnUrl);
                        return RedirectToAction("Index", "Clientes");
                    }
                    TempData["Error"] =
                        $"<p>El cliente <strong>{model.Cliente.CompanyName}</strong>:</p>" +
                        resultado.Mensaje;
                    // Sólo bloquea para errores definitivos
                    if (resultado.Codigo < 0)
                        model.BloquearEdicion = true;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"<p>Error al insertar el cliente " +
                    $"<strong>{model.Cliente?.CompanyName}</strong>.</p>" +
                    $"<p>Detalles: {ex.Message}</p>";
            }
            model.Paises = ObtenerPaises(model.Cliente?.Country);
            return View(model);
        }
        public IActionResult Editar(string? id, string? returnUrl = null)
        {
            var model = new ClienteEditarViewModel
            {
                ReturnUrl = returnUrl
            };
            model.Cliente = _clienteBLL.ObtenerClientePorId(id);
            if (model.Cliente == null)
            {
                TempData["Error"] = "<p>Cliente no encontrado</p>" + StringsCommons.Nefep;
                model.BloquearEdicion = true;
            }
            model.Paises = ObtenerPaises(model.Cliente?.Country);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(ClienteEditarViewModel model)
        {
            // Validaciones en el servidor
            if (string.IsNullOrEmpty(model.Cliente?.Country))
                ModelState.AddModelError(
                    "Cliente.Country",
                    "Seleccione o escriba un país");
            if (!ModelState.IsValid)
            {
                model.Paises = ObtenerPaises(model.Cliente?.Country);
                return View(model);
            }
            try
            {
                if (model.Cliente != null)
                {
                    var resultado = _clienteBLL.Actualizar(model.Cliente);
                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                            return LocalRedirect(model.ReturnUrl);
                        return RedirectToAction("Index", "Clientes");
                    }
                    TempData["Error"] =
                        $"<p>El cliente <strong>{model.Cliente.CompanyName}</strong>:</p>" +
                        resultado.Mensaje;
                    // Sólo bloquea para errores definitivos
                    if (resultado.Codigo < 0)
                        model.BloquearEdicion = true;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"<p>Error al actualizar el cliente " +
                    $"<strong>{model.Cliente?.CompanyName}</strong>.</p>" +
                    $"<p>Detalles: {ex.Message}</p>";
            }
            model.Paises = ObtenerPaises(model.Cliente?.Country);
            return View(model);
        }
        private List<SelectListItem> ObtenerPaises(string? country)
        {
            var paises = _clienteBLL.ObtenerClientesPaisesCbo()
                .Select(p => new SelectListItem
                {
                    Value = p.Value,
                    Text = p.Text
                })
                .ToList();
            // Si el usuario escribió un país nuevo,
            // agregarlo para conservarlo en el combo.
            if (!string.IsNullOrEmpty(country)
                && !paises.Any(p => p.Value == country))
            {
                paises.Add(new SelectListItem
                {
                    Value = country,
                    Text = country
                });
            }
            return paises;
        }
    }
}