using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9WebMVC.Models.Account;
using System.Security.Claims;

namespace NorthwindTradersV9WebMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsuarioBLL _usuarioBLL;
        private readonly PermisoBLL _permisoBLL;
        private const string SessionIntentosCambiarPassword =
            "CambiarPassword_Intentos"; 
        public AccountController(
            UsuarioBLL usuarioBLL,
            PermisoBLL permisoBLL)
        {
            _usuarioBLL = usuarioBLL;
            _permisoBLL = permisoBLL;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Usuario))
            {
                ModelState.AddModelError(
                    nameof(model.Usuario),
                    "Debe proporcionar el usuario.");
            }
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(
                    nameof(model.Password),
                    "Debe proporcionar la contraseña.");
            }
            if (!ModelState.IsValid)
                return View(model);
            // =====================================
            // GENERAR HASH SHA-256 DEL PASSWORD
            // =====================================
            string passwordHash =
                PasswordHelper.GenerarHash(model.Password);
            // =====================================
            // VALIDAR USUARIO
            // =====================================
            int idUsuario = _usuarioBLL.ValidarUsuario(
                model.Usuario,
                passwordHash,
                out string nombreUsuarioAutenticado);
            // =====================================
            // VALIDAR RESULTADO
            // =====================================
            if (idUsuario == 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "El usuario o la contraseña son incorrectos.");
                return View(model);
            }
            HashSet<int> permisosIds =
                _permisoBLL.ObtenerPermisosPorUsuarioId(idUsuario);
            // =====================================
            // CREAR LOS CLAIMS DEL USUARIO
            // =====================================
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    idUsuario.ToString()),
                new Claim(
                    ClaimTypes.Name,
                    nombreUsuarioAutenticado),
                new Claim(
                    "Usuario",
                    model.Usuario)
            };
            // =====================================
            // AGREGAR CLAIMS DE PERMISOS
            // =====================================
            foreach (int permisoId in permisosIds)
                claims.Add(
                    new Claim(
                        "Permiso",
                        permisoId.ToString()));
            // =====================================
            // CREAR LA IDENTIDAD
            // =====================================
            var claimsIdentity =
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
            // =====================================
            // CREAR EL PRINCIPAL
            // =====================================
            var claimsPrincipal =
                new ClaimsPrincipal(
                    claimsIdentity);
            // =====================================
            // PROPIEDADES DE AUTENTICACIÓN
            // =====================================
            var authProperties =
                new AuthenticationProperties
                {
                    IsPersistent = model.Recordarme
                };
             // =====================================
             // CREAR COOKIE
             // =====================================
             await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarPassword(
            [FromBody] CambiarPasswordInput input)
        {
            // =====================================================
            // Obtener usuario del usuario autenticado
            // =====================================================
            string? usuario =
                User.FindFirst("Usuario")?.Value;

            if (string.IsNullOrWhiteSpace(usuario))
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "No se pudo identificar al usuario autenticado."
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
            // =====================================================
            // Validar información recibida
            // =====================================================
            if (input == null)
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "No se recibieron los datos."
                });
            }

            if (string.IsNullOrWhiteSpace(input.ContrasenaActual))
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "Debe ingresar su contraseña actual."
                });
            }

            if (string.IsNullOrWhiteSpace(input.NuevaContrasena))
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "La nueva contraseña es obligatoria."
                });
            }

            if (string.IsNullOrWhiteSpace(input.ConfirmarContrasena))
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "La confirmación de la contraseña es obligatoria."
                });
            }
            // =====================================================
            // Validar que las contraseñas coincidan
            // =====================================================
            if (input.NuevaContrasena != input.ConfirmarContrasena)
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje =
                        "La nueva contraseña y la confirmación de la contraseña no coinciden."
                });
            }
            // =====================================================
            // Validar contraseña actual
            // =====================================================
            string passwordActualHasheada =
                PasswordHelper.GenerarHash(
                    input.ContrasenaActual.Trim());

            byte numRegs =
                _usuarioBLL.ValidarContraseñaActual(
                    usuario,
                    passwordActualHasheada);

            if (numRegs == 0)
            {
                int intentos =
                    HttpContext.Session.GetInt32(
                        SessionIntentosCambiarPassword) ?? 0;

                intentos++;

                HttpContext.Session.SetInt32(
                    SessionIntentosCambiarPassword,
                    intentos);

                if (intentos >= 3)
                {
                    HttpContext.Session.Remove(
                        SessionIntentosCambiarPassword);

                    return new JsonResult(new
                    {
                        ok = false,
                        cerrar = true,
                        mensaje =
                            "Demasiados intentos fallidos.\n\n" +
                            "Por favor, inténtelo de nuevo más tarde."
                    });
                }

                return new JsonResult(new
                {
                    ok = false,
                    cerrar = false,
                    mensaje = "La contraseña actual es incorrecta."
                });
            }

            HttpContext.Session.Remove(
                SessionIntentosCambiarPassword);
            // =====================================================
            // Actualizar contraseña
            // =====================================================
            string nuevaContrasenaHasheada =
                PasswordHelper.GenerarHash(
                    input.NuevaContrasena.Trim());

            byte registrosActualizados =
                _usuarioBLL.ActualizarContraseña(
                    usuario,
                    nuevaContrasenaHasheada);

            if (registrosActualizados > 0)
            {
                return new JsonResult(new
                {
                    ok = true,
                    mensaje = "Contraseña cambiada correctamente."
                });
            }

            return new JsonResult(new
            {
                ok = false,
                mensaje =
                    "No se pudo cambiar la contraseña. " +
                    "Verifique que su cuenta esté activa."
            });
        }
        public class CambiarPasswordInput
        {
            public string ContrasenaActual { get; set; } = string.Empty;
            public string NuevaContrasena { get; set; } = string.Empty;
            public string ConfirmarContrasena { get; set; } = string.Empty;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
