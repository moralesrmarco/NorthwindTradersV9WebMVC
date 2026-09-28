using NorthwindTradersV9DAL;

namespace NorthwindTradersV9BLL
{
    public class UsuarioBLL
    {
        private readonly IUsuarioDAL _usuarioDAL;
        public UsuarioBLL(IUsuarioDAL usuarioDAL) => _usuarioDAL = usuarioDAL;
        public int ValidarUsuario(string usuario, string password, out string nombreUsuarioAutenticado) => _usuarioDAL.ValidarUsuario(usuario, password, out nombreUsuarioAutenticado);
        public byte ValidarContraseñaActual(string usuario, string contrasenaActual) => _usuarioDAL.ValidarContraseñaActual(usuario, contrasenaActual);
        public byte ActualizarContraseña(string usuario, string nuevaContrasena) => _usuarioDAL.ActualizarContraseña(usuario, nuevaContrasena);
    }
}
