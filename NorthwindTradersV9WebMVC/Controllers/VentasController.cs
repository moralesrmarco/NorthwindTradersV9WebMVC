using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Helpers;
using NorthwindTradersV9WebMVC.Models.Common;
using NorthwindTradersV9WebMVC.Models.Ventas;
using System.Data;
using System.Text.Json;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoVentas")]
    public class VentasController : Controller
    {
        private readonly VentaBLL _ventaBLL;
        private readonly VentaDetalleBLL _ventaDetalleBLL;
        private readonly ClienteBLL _clienteBLL;
        private readonly EmpleadoBLL _empleadoBLL;
        private readonly TransportistaBLL _transportistaBLL;
        private readonly CategoriaBLL _categoriaBLL;
        private readonly ProductoBLL _productoBLL;
        private readonly AppSettings _appSettings;
        private const string SessionDetalleVenta = "DetalleVenta";
        public VentasController(
            VentaBLL ventaBLL, 
            VentaDetalleBLL ventaDetalleBLL, 
            ClienteBLL clienteBLL,
            EmpleadoBLL empleadoBLL,
            TransportistaBLL transportistaBLL,
            CategoriaBLL categoriaBLL,
            ProductoBLL productoBLL,
            IOptions<AppSettings> appSettings)
        {
            _ventaBLL = ventaBLL;
            _ventaDetalleBLL = ventaDetalleBLL;
            _clienteBLL = clienteBLL;
            _empleadoBLL = empleadoBLL;
            _transportistaBLL = transportistaBLL;
            _categoriaBLL = categoriaBLL;
            _productoBLL = productoBLL;
            _appSettings = appSettings.Value;
        }
        public IActionResult Index(
            int pageIndex = 1,
            VentasBuscarDto? filtro = null,
            bool BuscarAbierto = false)
        {
            filtro ??= new VentasBuscarDto();
            VentaPaginadoDto resultado;
            filtro.FVenta =
                filtro.FVentaIni.HasValue ||
                filtro.FVentaFin.HasValue;

            filtro.FRequerido =
                filtro.FRequeridoIni.HasValue ||
                filtro.FRequeridoFin.HasValue;

            filtro.FEnvio =
                filtro.FEnvioIni.HasValue ||
                filtro.FEnvioFin.HasValue;

            if (filtro.IdIni.HasValue && filtro.IdIni <= 0)
            {
                ModelState.AddModelError("Filtro.IdIni",
                    "El Id inicial debe ser mayor que cero");
            }

            if (filtro.IdFin.HasValue && filtro.IdFin <= 0)
            {
                ModelState.AddModelError("Filtro.IdFin",
                    "El Id final debe ser mayor que cero");
            }

            if (filtro.IdIni.HasValue &&
                filtro.IdFin.HasValue &&
                filtro.IdIni > filtro.IdFin)
            {
                ModelState.AddModelError("Filtro.IdFin",
                    "El Id final debe ser mayor o igual al Id inicial");
            }
            if (ModelState.IsValid)
            {
                resultado = _ventaBLL.ObtenerVentasPaginadasConBusqueda(
                    pageIndex,
                    _appSettings.RowsPerPage,
                    filtro);
            }
            else
            {
                resultado = new VentaPaginadoDto
                {
                    Ventas = new List<VentaDto>(),
                    PageIndex = 1,
                    TotalRegistros = 0
                };
            }
            var model = new VentasIndexViewModel
            {
                Ventas = resultado.Ventas,
                Filtro = filtro,
                Paginacion = new PaginacionViewModel
                {
                    PageIndex = resultado.PageIndex,
                    TotalRegistros = resultado.TotalRegistros,
                    PageSize = _appSettings.RowsPerPage
                },
                ParametrosPaginacion = new ParametrosPaginacionViewModel
                {
                    Controller = "Ventas",
                    Action = "Index",
                    BuscarAbierto = BuscarAbierto,
                    Parametros = new Dictionary<string, string?>
                    {
                        ["Filtro.IdIni"] = filtro.IdIni?.ToString(),
                        ["Filtro.IdFin"] = filtro.IdFin?.ToString(),
                        ["Filtro.Cliente"] = filtro.Cliente,

                        ["Filtro.FVenta"] = filtro.FVenta.ToString(),
                        ["Filtro.FVentaIni"] = filtro.FVentaIni?.ToString("yyyy-MM-dd"),
                        ["Filtro.FVentaFin"] = filtro.FVentaFin?.ToString("yyyy-MM-dd"),
                        ["Filtro.FVentaNull"] = filtro.FVentaNull.ToString(),

                        ["Filtro.FRequerido"] = filtro.FRequerido.ToString(),
                        ["Filtro.FRequeridoIni"] = filtro.FRequeridoIni?.ToString("yyyy-MM-dd"),
                        ["Filtro.FRequeridoFin"] = filtro.FRequeridoFin?.ToString("yyyy-MM-dd"),
                        ["Filtro.FRequeridoNull"] = filtro.FRequeridoNull.ToString(),

                        ["Filtro.FEnvio"] = filtro.FEnvio.ToString(),
                        ["Filtro.FEnvioIni"] = filtro.FEnvioIni?.ToString("yyyy-MM-dd"),
                        ["Filtro.FEnvioFin"] = filtro.FEnvioFin?.ToString("yyyy-MM-dd"),
                        ["Filtro.FEnvioNull"] = filtro.FEnvioNull.ToString(),

                        ["Filtro.Empleado"] = filtro.Empleado,
                        ["Filtro.CompañiaT"] = filtro.CompañiaT,
                        ["Filtro.DirigidoA"] = filtro.DirigidoA,
                        ["BuscarAbierto"] = BuscarAbierto.ToString().ToLower()
                    }
                }
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult Insertar(string? returnUrl, bool nueva = true)
        {
            var model = new VentasInsertarViewModel
            {
                ReturnUrl = returnUrl
            };

            // 1. Limpiar el detalle cuando se inicia una venta nueva.
            if (nueva)
            {
                HttpContext.Session.Remove(SessionDetalleVenta);
            }

            // 2. Establecer la fecha y hora actuales.
            var ahora = DateTime.Now;

            model.VentaVM.OrderDate = ahora.Date;
            model.VentaVM.OrderTime = ahora.TimeOfDay;

            // 3. Obtener la tasa de IVA vigente.
            model.VentaVM.TasaIVA = ObtenerTasaIVA(
                model.VentaVM.OrderDate,
                model.VentaVM.OrderTime);

            // 4. Cargar los combos.
            var combos = CargarCombos();

            model.Clientes = combos.Clientes;
            model.Vendedores = combos.Vendedores;
            model.Transportistas = combos.Transportistas;
            model.Categorias = combos.Categorias;
            model.Productos = combos.Productos;

            // 5. Recuperar los detalles y calcular los totales.
            model.Detalles = ObtenerDetalle();

            model.VentaTieneDetalles = model.Detalles.Count > 0;

            model.Totales = CalcularTotalesVenta(model.Detalles);

            return View(model);
        }
        [HttpGet]
        public IActionResult Consultar(int id, string? returnUrl = null)
        {
            var model = new VentasConsultarViewModel
            {
                ReturnUrl = returnUrl
            };
            // Obtener la venta.
            var venta = _ventaBLL.ObtenerVentaPorId(id);
            if (venta == null)
            {
                TempData["Error"] = "<p>Venta no encontrada</p>" + StringsCommons.Nefep;
                return View(model);
            }
            model.Venta = venta;
            // Obtener los detalles de la venta.
            model.VentaDetalle =
                _ventaDetalleBLL.ObtenerVentaDetallePorVentaId(id);
            // Calcular los totales.
            CalcularTotales(model.Venta, model.VentaDetalle);
            return View(model);
        }
        private void CalcularTotales(
            VentaDto venta,
            List<VentaDetalle> ventaDetalle)
        {
            venta.NumeroProductos = ventaDetalle.Count;
            venta.TotalUnidades = ventaDetalle.Sum(d => d.Quantity);
            venta.SubtotalImporte = ventaDetalle.Sum(d => d.SubtotalDelImporteConIVAIncluido);
            venta.SubtotalImporteDescuento = ventaDetalle.Sum(d => d.SubtotalDelAhorroTotalDespuesDescuento);
            venta.SubtotalImporteConDescuento = ventaDetalle.Sum(d => d.SubtotalDelImporteConIVAConDescuento);
            venta.SubtotalImporteSinIVA = ventaDetalle.Sum(d => d.SubtotalDelImporteSinIVAConDescuento);
            venta.SubtotalImporteConIVA = ventaDetalle.Sum(d => d.SubtotalIVADespuesDelDescuento);
            venta.Total = ventaDetalle.Sum(d => d.Subtotal);
        }
        [HttpGet]
        public IActionResult NotaRemisionPdf(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("OrderId inválido.");
            // Fecha y hora del reporte.
            string strFecha = DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
            // Crear el reporte.
            LocalReport report = new LocalReport();
            string rutaReporte = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Ventas",
                "RptNotaRemision9.rdlc");
            using (var stream = System.IO.File.OpenRead(rutaReporte))
            {
                report.LoadReportDefinition(stream);
            }
            //=========================================
            // DATOS (se obtienen UNA sola vez)
            //=========================================
            DataTable dtDummy = new DataTable("DataSetDummy");
            dtDummy.Columns.Add("Dummy_", typeof(int));
            dtDummy.Rows.Add(1);
            var venta = _ventaBLL.ObtenerVentaPorIdDt(orderId);
            if (venta == null)
                return NotFound($"No se encontró la venta con OrderID = {orderId}");
            var detalle =
                _ventaDetalleBLL.ObtenerVentaDetallePorVentaId(orderId);
            //=========================================
            // REPORTE CLIENTE
            //=========================================
            report.DataSources.Add(
                new ReportDataSource("DataSetDummy", dtDummy));
            report.DataSources.Add(
                new ReportDataSource("DataSetVenta", venta));
            report.DataSources.Add(
                new ReportDataSource("DataSet1", detalle));
            report.SetParameters(new[]
            {
                new ReportParameter("PedidoId", orderId.ToString()),
                new ReportParameter("FechaHora", $"Fecha: {strFecha}"),
                new ReportParameter("Para", "Para: Cliente.")
            });
            byte[] pdfCliente = report.Render("PDF");
            //=========================================
            // REPORTE CONTROL INTERNO
            //=========================================
            report.DataSources.Clear();
            report.DataSources.Add(
                new ReportDataSource("DataSetDummy", dtDummy));
            report.DataSources.Add(
                new ReportDataSource("DataSetVenta", venta));
            report.DataSources.Add(
                new ReportDataSource("DataSet1", detalle));
            report.SetParameters(new[]
            {
                new ReportParameter("PedidoId", orderId.ToString()),
                new ReportParameter("FechaHora", $"Fecha: {strFecha}"),
                new ReportParameter("Para", "Para: Control interno.")
            });
            byte[] pdfControl = report.Render("PDF");
            //=========================================
            // COMBINAR LOS DOS PDF
            //=========================================
            var helper = new ReporteServicePdfHelper();
            byte[] pdfFinal = helper.CombinarPDFs(
                new[]
                {
                    pdfCliente,
                    pdfControl
                });
            return File(pdfFinal, "application/pdf");
        }
        public IActionResult Eliminar(int id, string? returnUrl = null)
        {
            var model = new VentasEliminarViewModel
            {
                ReturnUrl = returnUrl
            };
            var venta = _ventaBLL.ObtenerVentaPorId(id);
            if (venta == null)
            {
                TempData["Error"] = "<p>Venta no encontrada</p>" + StringsCommons.Nefep;
                return View(model);
            }
            model.Venta = venta;
            // Obtener los detalles de la venta.
            model.VentaDetalle = _ventaDetalleBLL.ObtenerVentaDetallePorVentaId(id);
            // Calcular los totales.
            CalcularTotales(model.Venta, model.VentaDetalle);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(VentasEliminarViewModel model)
        {
            if (model.Venta != null)
            {
                var resultado = _ventaBLL.Eliminar(model.Venta);
                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);
                    return RedirectToAction("Index", "Ventas");
                }
                else
                {
                    TempData["Error"] = $"<p>La venta con Id: <strong>{model.Venta.OrderID}</strong> del cliente: <strong>{model.Venta?.CustomerCompanyName}</strong>:</p>{resultado.Mensaje}";
                    if (resultado.Codigo < 0)
                        model.BloquearEliminacion = true;
                    // Recargar los datos cuando el resultado sea -2 o -7.
                    if (resultado.Codigo == -2 || resultado.Codigo == -7)
                    {
                        var venta = _ventaBLL.ObtenerVentaPorId(model.Venta.OrderID);
                        if (venta != null)
                        {
                            model.Venta = venta;
                            model.VentaDetalle =
                                _ventaDetalleBLL.ObtenerVentaDetallePorVentaId(model.Venta.OrderID);
                            CalcularTotales(model.Venta, model.VentaDetalle);
                        }
                    }
                }
            }
            return View(model);
        }
        private VentaCombosViewModel CargarCombos(int? categoriaId = null)
        {
            var combos = new VentaCombosViewModel();
            // Clientes
            combos.Clientes = _clienteBLL.ObtenerClientesCbo()
                .Select(c => new SelectListItem
                {
                    Value = c.Value,
                    Text = c.Text
                }).ToList();
            // Vendedores
            combos.Vendedores = _empleadoBLL.ObtenerEmpleadoEmpleadosCbo()
                .Where(e => e.Text != "N/A")
                .Select(e => new SelectListItem
                {
                    Value = e.Value,
                    Text = e.Text
                }).ToList();
            // Transportistas
            combos.Transportistas = _transportistaBLL.ObtenerTransportistasCbo()
                .Select(t => new SelectListItem
                {
                    Value = t.Value,
                    Text = t.Text
                }).ToList();
            // Categorías
            combos.Categorias = _categoriaBLL.ObtenerCategoriasCbo()
                .Select(c => new SelectListItem
                {
                    Value = c.Value,
                    Text = c.Text
                }).ToList();
            // Productos
            if (categoriaId.HasValue)
            {
                combos.Productos = _productoBLL
                    .ObtenerProductosPorCategoriaCbo(categoriaId.Value)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Value,
                        Text = p.Text
                    }).ToList();
            }
            else
            {
                combos.Productos = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "",
                        Text = "»--- Seleccione una categoría ---«"
                    }
                };
            }
            return combos;
        }
        private List<VentaDetalleViewModel> ObtenerDetalle()
        {
            var json = HttpContext.Session.GetString(SessionDetalleVenta);
            if (string.IsNullOrEmpty(json))
                return new List<VentaDetalleViewModel>();
            return JsonSerializer.Deserialize<List<VentaDetalleViewModel>>(json)
                   ?? new List<VentaDetalleViewModel>();
        }
        private void GuardarDetalle(List<VentaDetalleViewModel> lista)
        {
            var json = JsonSerializer.Serialize(lista);
            HttpContext.Session.SetString(SessionDetalleVenta, json);
        }
        private VentaTotalesViewModel CalcularTotalesVenta(
            List<VentaDetalleViewModel> lista)
        {
            return new VentaTotalesViewModel
            {
                NumeroProductos = lista.Count,
                TotalUnidades = lista.Sum(x => x.Quantity),
                TotalImporteConIVA =
                    lista.Sum(x => x.SubtotalDelImporteConIVAIncluido),
                TotalDescuento =
                    lista.Sum(x => x.SubtotalDelAhorroTotalDespuesDescuento),
                TotalImporteConDescuento =
                    lista.Sum(x => x.SubtotalDelImporteConIVAConDescuento),
                TotalImporteSinIVA =
                    lista.Sum(x => x.SubtotalDelImporteSinIVAConDescuento),
                TotalIVA =
                    lista.Sum(x => x.SubtotalIVADespuesDelDescuento),
                Total =
                    lista.Sum(x => x.Subtotal)
            };
        }
        private decimal ObtenerTasaIVA(DateTime? fecha, TimeSpan? hora)
        {
            var fechaHora = CombinarFechaHora(fecha, hora);
            if (!fechaHora.HasValue)
                return 0m;
            var tasa = _ventaBLL.ObtenerTasaVigente(fechaHora.Value);
            return tasa ?? 0m;
        }
        private static DateTime? CombinarFechaHora(DateTime? fecha, TimeSpan? hora)
        {
            if (!fecha.HasValue)
                return null;
            return fecha.Value.Date + (hora ?? TimeSpan.Zero);
        }
        [HttpGet]
        public IActionResult ProductosPorCategoria(int categoriaId)
        {
            var productos = _productoBLL.ObtenerProductosPorCategoriaCbo(categoriaId);
            return Json(productos);
        }
        [HttpGet]
        public IActionResult UltimaInformacionEnvio(string customerId)
        {
            var informacion = _ventaBLL.ObtenerUltimaInformacionDeEnvio(customerId);
            return Json(informacion);
        }
        [HttpGet]
        public IActionResult ProductoCostoEInventario(int productId)
        {
            var producto = _productoBLL.ObtenerProductoCostoEInventario(productId);
            return Json(producto);
        }
        [HttpGet]
        public IActionResult FormasEnvioCliente(string customerId)
        {
            var lista = _ventaBLL.ObtenerFormasEnvio(customerId);
            return Json(lista);
        }
        [HttpPost]
        public IActionResult CalcularDetalle(
            [FromBody] VentaDetalleViewModel detalle)
        {
            var ventaDetalle = detalle.ToVentaDetalle();
            // Aquí se ejecuta el cálculo de VentaDetalle
            var resultado = VentaDetalleViewModel.FromVentaDetalle(ventaDetalle);
            return Json(resultado);
        }
        [HttpPost]
        public IActionResult AgregarDetalle(
            [FromBody] VentaDetalleViewModel detalle)
        {
            if (detalle.CategoriaID <= 0)
            {
                return BadRequest("Debe seleccionar una categoría.");
            }
            if (detalle.ProductID <= 0)
            {
                return BadRequest("Debe seleccionar un producto.");
            }
            if (detalle.Quantity <= 0)
            {
                return BadRequest("La cantidad debe ser mayor que cero.");
            }
            if (detalle.Quantity > detalle.UnitsInStock)
            {
                return BadRequest(
                    "La cantidad no puede ser mayor que las unidades en inventario.");
            }
            var lista = ObtenerDetalle();
            // Validar producto duplicado
            if (lista.Any(x => x.ProductID == detalle.ProductID))
            {
                return BadRequest(
                    "No se puede agregar un mismo producto más de una vez a la venta.");
            }
            // Calcular el detalle
            var ventaDetalle = detalle.ToVentaDetalle();
            var detalleCalculado =
                VentaDetalleViewModel.FromVentaDetalle(ventaDetalle);
            // Conservar datos de la UI que no pertenecen al cálculo
            detalleCalculado.CategoriaID = detalle.CategoriaID;
            detalleCalculado.UnitsInStock = detalle.UnitsInStock;
            // Guardar en Session
            lista.Add(detalleCalculado);
            GuardarDetalle(lista);
            var totales = CalcularTotalesVenta(lista);
            return Json(new
            {
                count = lista.Count,
                lista = lista,
                totales = totales,
                ventaTieneDetalles = lista.Count > 0
            });
        }
        [HttpPost]
        public IActionResult EliminarDetalle([FromBody] int indice)
        {
            var lista = ObtenerDetalle();
            if (indice < 0 || indice >= lista.Count)
            {
                return BadRequest("Producto no encontrado.");
            }
            lista.RemoveAt(indice);
            GuardarDetalle(lista);
            var totales = CalcularTotalesVenta(lista);
            return Json(new
            {
                lista,
                totales,
                ventaTieneDetalles = lista.Count > 0
            });
        }
        [HttpPost]
        public IActionResult EditarDetalle([FromBody] int indice)
        {
            var lista = ObtenerDetalle();
            if (indice < 0 || indice >= lista.Count)
            {
                return BadRequest("Producto no encontrado.");
            }
            var detalle = lista[indice];
            // Lo quitamos temporalmente
            lista.RemoveAt(indice);
            GuardarDetalle(lista);
            var totales = CalcularTotalesVenta(lista);
            return Json(new
            {
                detalle,
                lista,
                totales
            });
        }
        [HttpPost]
        public IActionResult ObtenerTasaIVA(
            [FromBody] ObtenerTasaIVARequest request)
        {
            var fechaHora = CombinarFechaHora(
                request.OrderDate,
                request.OrderTime);
            if (!fechaHora.HasValue)
            {
                return Json(new
                {
                    ok = false,
                    mensaje = "Debe indicar la fecha y hora de la venta."
                });
            }
            var tasa = _ventaBLL.ObtenerTasaVigente(
                fechaHora.Value);
            if (!tasa.HasValue)
            {
                return Json(new
                {
                    ok = false,
                    mensaje = "No existe una tasa de IVA vigente para la fecha indicada."
                });
            }
            return Json(new
            {
                ok = true,
                tasaIVA = tasa.Value
            });
        }
        private Venta ConstruirVenta(
            VentasInsertarViewModel model,
            List<VentaDetalleViewModel> detalles)
        {
            var fechaHoraVenta = CombinarFechaHora(
                model.VentaVM.OrderDate,
                model.VentaVM.OrderTime);

            var fechaHoraRequerido = CombinarFechaHora(
                model.VentaVM.RequiredDate,
                model.VentaVM.RequiredTime);

            var fechaHoraEnvio = CombinarFechaHora(
                model.VentaVM.ShippedDate,
                model.VentaVM.ShippedTime);

            var venta = new Venta
            {
                Cliente = new Cliente
                {
                    CustomerId = model.VentaVM.CustomerID
                },
                Empleado = new Empleado
                {
                    EmployeeID = model.VentaVM.EmployeeID!.Value
                },
                Transportista = new Transportista
                {
                    ShipperID = model.VentaVM.ShipVia ?? 0
                },
                OrderDate = fechaHoraVenta,
                RequiredDate = fechaHoraRequerido,
                ShippedDate = fechaHoraEnvio,
                ShipName = model.VentaVM.ShipName,
                ShipAddress = model.VentaVM.ShipAddress,
                ShipCity = model.VentaVM.ShipCity,
                ShipRegion = model.VentaVM.ShipRegion,
                ShipPostalCode = model.VentaVM.ShipPostalCode,
                ShipCountry = model.VentaVM.ShipCountry,
                Freight = model.VentaVM.Freight,
                VentaDetalles = new List<VentaDetalle>()
            };

            foreach (var item in detalles)
            {
                venta.VentaDetalles.Add(new VentaDetalle
                {
                    Producto = new Producto
                    {
                        ProductID = item.ProductID!.Value
                    },
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TasaIVA = model.VentaVM.TasaIVA
                });
            }

            return venta;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerarVenta(VentasInsertarViewModel model)
        {
            // Cargar nuevamente los combos para la vista.
            var combos = CargarCombos(model.Detalle.CategoriaID);

            model.Clientes = combos.Clientes;
            model.Vendedores = combos.Vendedores;
            model.Transportistas = combos.Transportistas;
            model.Categorias = combos.Categorias;
            model.Productos = combos.Productos;

            if (!model.VentaVM.EmployeeID.HasValue ||
                model.VentaVM.EmployeeID <= 0)
            {
                ModelState.AddModelError(
                    "VentaVM.EmployeeID",
                    "Debe seleccionar un vendedor.");
            }

            var lista = ObtenerDetalle();

            if (lista.Count == 0)
            {
                TempData["Error"] = "Debe agregar al menos un producto a la venta.";
            }

            // Construir Fecha/Hora de la venta.
            var fechaHoraVenta = CombinarFechaHora(
                model.VentaVM.OrderDate,
                model.VentaVM.OrderTime);

            if (!fechaHoraVenta.HasValue)
            {
                ModelState.AddModelError(
                    "VentaVM.OrderDate",
                    "Debe indicar la fecha de la venta.");
            }

            decimal? tasaIVA = null;

            if (fechaHoraVenta.HasValue)
            {
                tasaIVA = _ventaBLL.ObtenerTasaVigente(
                    fechaHoraVenta.Value);
            }

            if (fechaHoraVenta.HasValue && !tasaIVA.HasValue)
            {
                ModelState.AddModelError(
                    "",
                    "No existe una tasa de IVA vigente para la fecha de la venta.");
            }

            if (tasaIVA.HasValue)
            {
                model.VentaVM.TasaIVA = tasaIVA.Value;
            }

            // Construir Fecha/Hora requerida.
            var fechaHoraRequerido = CombinarFechaHora(
                model.VentaVM.RequiredDate,
                model.VentaVM.RequiredTime);

            // Construir Fecha/Hora de envío.
            var fechaHoraEnvio = CombinarFechaHora(
                model.VentaVM.ShippedDate,
                model.VentaVM.ShippedTime);

            // 1. Validar Requerido vs Venta.
            if (fechaHoraVenta.HasValue &&
                fechaHoraRequerido.HasValue &&
                fechaHoraRequerido.Value < fechaHoraVenta.Value)
            {
                ModelState.AddModelError(
                    "VentaVM.RequiredTime",
                    "La fecha y hora requerida no pueden ser anteriores a la venta.");
            }

            // 2. Validar Envío vs Venta.
            if (fechaHoraVenta.HasValue &&
                fechaHoraEnvio.HasValue &&
                fechaHoraEnvio.Value < fechaHoraVenta.Value)
            {
                ModelState.AddModelError(
                    "VentaVM.ShippedTime",
                    "La fecha y hora de envío no pueden ser anteriores a la venta.");
            }

            // 3. Validar Envío vs Requerido.
            if (fechaHoraEnvio.HasValue &&
                fechaHoraRequerido.HasValue &&
                fechaHoraEnvio.Value > fechaHoraRequerido.Value)
            {
                ModelState.AddModelError(
                    "VentaVM.ShippedTime",
                    "La fecha y hora de envío no pueden ser posteriores a la fecha y hora requeridas.");
            }

            if (!ModelState.IsValid || lista.Count == 0)
            {
                model.Detalles = lista;
                model.Totales = CalcularTotalesVenta(lista);
                model.VentaTieneDetalles = lista.Count > 0;

                return View("Insertar", model);
            }

            // Construir la entidad Venta.
            var venta = ConstruirVenta(model, lista);

            int orderId;
            byte[] rowVersion;

            ResultadoOperacion resultado =
                _ventaBLL.InsertarVentaCompleta(
                    venta,
                    out orderId,
                    out rowVersion);

            if (!resultado.Exito)
            {
                ModelState.AddModelError("", resultado.Mensaje);

                model.Detalles = lista;
                model.Totales = CalcularTotalesVenta(lista);
                model.VentaTieneDetalles = lista.Count > 0;

                return View("Insertar", model);
            }

            // La venta se guardó correctamente.
            HttpContext.Session.Remove(SessionDetalleVenta);

            model.OrderID = orderId;

            // Elimina el valor antiguo (0).
            ModelState.Remove(nameof(model.OrderID));

            model.VentaGuardada = true;

            TempData["VentaGuardada"] = true;
            TempData["Mensaje"] = $"La venta N° {orderId} se registró correctamente.";

            model.Detalles = lista;
            model.Totales = CalcularTotalesVenta(lista);
            model.VentaTieneDetalles = lista.Count > 0;

            return View("Insertar", model);
        }

    }
}
