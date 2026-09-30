using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public interface IUsuarioDAL
    {
        int ValidarUsuario(string usuario, string password, out string nombreUsuarioAutenticado);
        byte ValidarContraseñaActual(string usuario, string contrasenaActual);
        byte ActualizarContraseña(string usuario, string nuevaContrasena);
        Usuario? ObtenerPorId(int id);
        int Insertar(Usuario usuario);
        int Actualizar(Usuario usuario);
        int Eliminar(Usuario usuario);
        bool ExisteNombreUsuario(string nombreUsuario, int? idExcluir);
        DataTable Buscar(UsuarioBuscarDto filtro);
        UsuarioPaginadoDto BuscarConPaginacion(
            int idIni,
            int idFin,
            string paterno,
            string materno,
            string nombres,
            string nombreUsuario,
            int pageIndex,
            int pageSize);
    }
}
