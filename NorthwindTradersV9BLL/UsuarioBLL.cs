using NorthwindTradersV9DAL;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9BLL
{
    public class UsuarioBLL
    {
        private readonly IUsuarioDAL _usuarioDAL;
        public UsuarioBLL(IUsuarioDAL usuarioDAL) => _usuarioDAL = usuarioDAL;
        public int ValidarUsuario(string usuario, string password, out string nombreUsuarioAutenticado) => _usuarioDAL.ValidarUsuario(usuario, password, out nombreUsuarioAutenticado);
        public byte ValidarContraseñaActual(string usuario, string contrasenaActual) => _usuarioDAL.ValidarContraseñaActual(usuario, contrasenaActual);
        public byte ActualizarContraseña(string usuario, string nuevaContrasena) => _usuarioDAL.ActualizarContraseña(usuario, nuevaContrasena);
        public Usuario? ObtenerPorId(int id) => _usuarioDAL.ObtenerPorId(id);
        public int Insertar(Usuario usuario) => _usuarioDAL.Insertar(usuario);
        public int Actualizar(Usuario usuario) => _usuarioDAL.Actualizar(usuario);
        public int Eliminar(Usuario usuario) => _usuarioDAL.Eliminar(usuario);
        public bool ExisteNombreUsuario(string nombreUsuario, int? idExcluir = null) => _usuarioDAL.ExisteNombreUsuario(nombreUsuario, idExcluir);
        public DataTable Buscar(UsuarioBuscarDto filtro) => _usuarioDAL.Buscar(filtro);
        public UsuarioPaginadoDto BuscarConPaginacion(
            int idIni,
            int idFin,
            string paterno,
            string materno,
            string nombres,
            string nombreUsuario,
            int pageIndex,
            int pageSize)
        {
            return _usuarioDAL.BuscarConPaginacion(
                idIni,
                idFin,
                paterno,
                materno,
                nombres,
                nombreUsuario,
                pageIndex,
                pageSize);
        }
    }
}
