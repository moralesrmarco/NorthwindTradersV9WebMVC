namespace NorthwindTradersV9DAL
{
    public interface IUsuarioDAL
    {
        int ValidarUsuario(string usuario, string password, out string nombreUsuarioAutenticado);
        byte ValidarContraseñaActual(string usuario, string contrasenaActual);
        byte ActualizarContraseña(string usuario, string nuevaContrasena);
    }
}
