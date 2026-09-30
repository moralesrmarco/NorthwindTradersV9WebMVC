using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9WebMVC.Models.Administracion;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Controllers
{
    public class AdministracionController : Controller
    {
        private readonly UsuarioBLL _usuarioBLL;
        private readonly AppSettings _appSettings;

        public AdministracionController(
            UsuarioBLL usuarioBLL,
            IOptions<AppSettings> appSettings)
        {
            _usuarioBLL = usuarioBLL;
            _appSettings = appSettings.Value;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AdministracionUsuarios(
            int pageIndex = 1,
            int? IdIni = null,
            int? IdFin = null,
            string? Paterno = null,
            string? Materno = null,
            string? Nombres = null,
            string? NombreUsuario = null,
            int? idUsuario = null,
            string? modo = null)
        {
            int pageSize = _appSettings.RowsPerPage;

            var resultado = _usuarioBLL.BuscarConPaginacion(
                IdIni ?? 0,
                IdFin ?? 0,
                Paterno ?? string.Empty,
                Materno ?? string.Empty,
                Nombres ?? string.Empty,
                NombreUsuario ?? string.Empty,
                pageIndex,
                pageSize);


            Usuario? usuarioSeleccionado = null;
            modo ??= "crear";

            var usuarioEdicion = new AdministracionUsuarioEdicionViewModel();
            var usuarioEliminar = new AdministracionUsuarioEliminarViewModel();

            if (idUsuario.HasValue)
            {
                usuarioSeleccionado = _usuarioBLL.ObtenerPorId(idUsuario.Value);

                if (usuarioSeleccionado != null)
                {
                    if (modo == "eliminar")
                    {
                        usuarioEliminar = new AdministracionUsuarioEliminarViewModel
                        {
                            Id = usuarioSeleccionado.Id,
                            Paterno = usuarioSeleccionado.Paterno,
                            Materno = usuarioSeleccionado.Materno,
                            Nombres = usuarioSeleccionado.Nombres,
                            NombreUsuario = usuarioSeleccionado.NombreUsuario,
                            Estatus = usuarioSeleccionado.Estatus,
                            RowVersion = usuarioSeleccionado.RowVersion
                        };
                    }
                    else
                    {
                        modo = "editar";

                        usuarioEdicion = new AdministracionUsuarioEdicionViewModel
                        {
                            Id = usuarioSeleccionado.Id,
                            Paterno = usuarioSeleccionado.Paterno,
                            Materno = usuarioSeleccionado.Materno,
                            Nombres = usuarioSeleccionado.Nombres,
                            NombreUsuario = usuarioSeleccionado.NombreUsuario,
                            Estatus = usuarioSeleccionado.Estatus,
                            RowVersion = usuarioSeleccionado.RowVersion
                        };
                    }
                }
            }

            var modelo = new AdministracionUsuariosViewModel
            {
                Usuarios = resultado.Usuarios,
                UsuarioSeleccionado = usuarioSeleccionado,
                UsuarioEdicion = usuarioEdicion,
                UsuarioEliminar = usuarioEliminar,
                Modo = modo,

                IdIni = IdIni,
                IdFin = IdFin,
                Paterno = Paterno,
                Materno = Materno,
                Nombres = Nombres,
                NombreUsuario = NombreUsuario,

                Paginacion = new Models.Common.PaginacionViewModel
                {
                    PageIndex = resultado.PageIndex,
                    PageSize = pageSize,
                    TotalRegistros = resultado.TotalRegistros
                },

                ParametrosPaginacion = new ParametrosPaginacionViewModel
                {
                    Controller = "Administracion",
                    Action = "AdministracionUsuarios",
                    Parametros = new Dictionary<string, string?>
                    {
                        ["IdIni"] = IdIni?.ToString(),
                        ["IdFin"] = IdFin?.ToString(),
                        ["Paterno"] = Paterno,
                        ["Materno"] = Materno,
                        ["Nombres"] = Nombres,
                        ["NombreUsuario"] = NombreUsuario
                    }
                }
            };

            return View(modelo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarUsuario(
            AdministracionUsuarioEdicionViewModel usuarioEdicion,
            string modo,
            int pageIndex = 1,
            int? IdIni = null,
            int? IdFin = null,
            string? Paterno = null,
            string? Materno = null,
            string? Nombres = null,
            string? NombreUsuario = null)
        {
            bool esNuevo = !usuarioEdicion.Id.HasValue;

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(usuarioEdicion.Nombres))
            {
                ModelState.AddModelError(
                    "UsuarioEdicion.Nombres",
                    "El nombre es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(usuarioEdicion.NombreUsuario))
            {
                ModelState.AddModelError(
                    "UsuarioEdicion.NombreUsuario",
                    "El usuario es obligatorio.");
            }

            if (esNuevo)
            {
                if (string.IsNullOrWhiteSpace(usuarioEdicion.Password))
                {
                    ModelState.AddModelError(
                        "UsuarioEdicion.Password",
                        "La contraseña es obligatoria.");
                }

                if (string.IsNullOrWhiteSpace(usuarioEdicion.ConfirmarPassword))
                {
                    ModelState.AddModelError(
                        "UsuarioEdicion.ConfirmarPassword",
                        "La confirmación de la contraseña es obligatoria.");
                }
            }

            if (!string.IsNullOrWhiteSpace(usuarioEdicion.Password))
            {
                if (string.IsNullOrWhiteSpace(usuarioEdicion.ConfirmarPassword))
                {
                    ModelState.AddModelError(
                        "UsuarioEdicion.ConfirmarPassword",
                        "Debe confirmar la nueva contraseña.");
                }
                else if (usuarioEdicion.Password != usuarioEdicion.ConfirmarPassword)
                {
                    ModelState.AddModelError(
                        "UsuarioEdicion.ConfirmarPassword",
                        "Las contraseñas no coinciden.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(usuarioEdicion.ConfirmarPassword))
            {
                ModelState.AddModelError(
                    "UsuarioEdicion.Password",
                    "Debe proporcionar la nueva contraseña.");
            }

            // Validar usuario duplicado
            if (!string.IsNullOrWhiteSpace(usuarioEdicion.NombreUsuario) &&
                _usuarioBLL.ExisteNombreUsuario(
                    usuarioEdicion.NombreUsuario.Trim(),
                    esNuevo ? null : usuarioEdicion.Id))
            {
                ModelState.AddModelError(
                    "UsuarioEdicion.NombreUsuario",
                    "El usuario ya existe.");
            }

            if (!ModelState.IsValid)
            {
                var resultado = _usuarioBLL.BuscarConPaginacion(
                    IdIni ?? 0,
                    IdFin ?? 0,
                    Paterno ?? string.Empty,
                    Materno ?? string.Empty,
                    Nombres ?? string.Empty,
                    NombreUsuario ?? string.Empty,
                    pageIndex,
                    _appSettings.RowsPerPage);

                var modelo = new AdministracionUsuariosViewModel
                {
                    Usuarios = resultado.Usuarios,
                    UsuarioEdicion = usuarioEdicion,
                    Modo = modo,

                    IdIni = IdIni,
                    IdFin = IdFin,
                    Paterno = Paterno,
                    Materno = Materno,
                    Nombres = Nombres,
                    NombreUsuario = NombreUsuario,

                    Paginacion = new PaginacionViewModel
                    {
                        PageIndex = resultado.PageIndex,
                        PageSize = _appSettings.RowsPerPage,
                        TotalRegistros = resultado.TotalRegistros
                    },

                    ParametrosPaginacion = new ParametrosPaginacionViewModel
                    {
                        Controller = "Administracion",
                        Action = "AdministracionUsuarios",
                        Parametros = new Dictionary<string, string?>
                        {
                            ["IdIni"] = IdIni?.ToString(),
                            ["IdFin"] = IdFin?.ToString(),
                            ["Paterno"] = Paterno,
                            ["Materno"] = Materno,
                            ["Nombres"] = Nombres,
                            ["NombreUsuario"] = NombreUsuario
                        }
                    }
                };

                return View("AdministracionUsuarios", modelo);
            }

            try
            {
                if (esNuevo)
                {
                    // Usuario nuevo: generar el hash de la contraseña.
                    usuarioEdicion.Password =
                        PasswordHelper.GenerarHash(
                            usuarioEdicion.Password!.Trim());
                }
                else if (string.IsNullOrWhiteSpace(usuarioEdicion.Password))
                {
                    // Edición sin cambiar contraseña:
                    // conservar el hash que ya existe en BD.
                    var usuarioActual =
                        _usuarioBLL.ObtenerPorId(usuarioEdicion.Id!.Value);

                    if (usuarioActual == null)
                    {
                        TempData["Error"] =
                            "El usuario ya no existe.";

                        return RedirectToAction(
                            nameof(AdministracionUsuarios),
                            new
                            {
                                pageIndex,
                                IdIni,
                                IdFin,
                                Paterno,
                                Materno,
                                Nombres,
                                NombreUsuario
                            });
                    }

                    usuarioEdicion.Password = usuarioActual.Password;
                }
                else
                {
                    // Edición con cambio de contraseña:
                    // generar el nuevo hash.
                    usuarioEdicion.Password =
                        PasswordHelper.GenerarHash(
                            usuarioEdicion.Password.Trim());
                }

                var usuario = new Usuario
                {
                    Id = usuarioEdicion.Id ?? 0,
                    Paterno = usuarioEdicion.Paterno,
                    Materno = usuarioEdicion.Materno,
                    Nombres = usuarioEdicion.Nombres,
                    NombreUsuario = usuarioEdicion.NombreUsuario!.Trim(),
                    Password = usuarioEdicion.Password,
                    Estatus = usuarioEdicion.Estatus,
                    RowVersion = usuarioEdicion.RowVersion
                };

                int registros = esNuevo
                    ? _usuarioBLL.Insertar(usuario)
                    : _usuarioBLL.Actualizar(usuario);

                if (registros > 0)
                {
                    TempData["Exito"] =
                        "El usuario se guardó correctamente.";
                }
                else if (registros == -1)
                {
                    TempData["Error"] =
                        "El usuario fue eliminado previamente por otro usuario de la red.";
                }
                else if (registros == -2)
                {
                    TempData["Error"] =
                        "El usuario fue modificado previamente por otro usuario de la red.";
                }
                else
                {
                    TempData["Error"] =
                        "No fue posible guardar el usuario.";
                }

                return RedirectToAction(
                    nameof(AdministracionUsuarios),
                    new
                    {
                        pageIndex,
                        IdIni,
                        IdFin,
                        Paterno,
                        Materno,
                        Nombres,
                        NombreUsuario
                    });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(
                    nameof(AdministracionUsuarios),
                    new
                    {
                        pageIndex,
                        IdIni,
                        IdFin,
                        Paterno,
                        Materno,
                        Nombres,
                        NombreUsuario,
                        idUsuario = usuarioEdicion.Id
                    });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarUsuario(
            AdministracionUsuarioEliminarViewModel usuarioEliminar,
            int pageIndex = 1,
            int? IdIni = null,
            int? IdFin = null,
            string? Paterno = null,
            string? Materno = null,
            string? Nombres = null,
            string? NombreUsuario = null)
        {
            try
            {
                var usuario = new Usuario
                {
                    Id = usuarioEliminar.Id,
                    RowVersion = usuarioEliminar.RowVersion
                };

                var registros = _usuarioBLL.Eliminar(usuario);

                if (registros > 0)
                {
                    TempData["Exito"] = "El usuario se eliminó correctamente.";

                    // Verificar cuál es la última página válida después de eliminar.
                    var resultado = _usuarioBLL.BuscarConPaginacion(
                        IdIni ?? 0,
                        IdFin ?? 0,
                        Paterno ?? string.Empty,
                        Materno ?? string.Empty,
                        Nombres ?? string.Empty,
                        NombreUsuario ?? string.Empty,
                        pageIndex,
                        _appSettings.RowsPerPage);

                    pageIndex = resultado.TotalRegistros == 0
                        ? 1
                        : Math.Min(
                            pageIndex,
                            (int)Math.Ceiling(
                                (double)resultado.TotalRegistros / _appSettings.RowsPerPage));
                }
                else if (registros == -1)
                {
                    TempData["Error"] =
                        "El usuario fue eliminado previamente por otro usuario de la red.";
                }
                else if (registros == -2)
                {
                    TempData["Error"] =
                        "El usuario fue modificado previamente por otro usuario de la red.";
                }
                else
                {
                    TempData["Error"] =
                        "No fue posible eliminar el usuario.";
                }

                return RedirectToAction(
                    nameof(AdministracionUsuarios),
                    new
                    {
                        pageIndex,
                        IdIni,
                        IdFin,
                        Paterno,
                        Materno,
                        Nombres,
                        NombreUsuario
                    });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(
                    nameof(AdministracionUsuarios),
                    new
                    {
                        pageIndex,
                        IdIni,
                        IdFin,
                        Paterno,
                        Materno,
                        Nombres,
                        NombreUsuario,
                        idUsuario = usuarioEliminar.Id,
                        modo = "eliminar"
                    });
            }
        }
    }
}
