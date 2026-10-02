using NorthwindTradersV9DAL;
using System.Data;

namespace NorthwindTradersV9BLL
{
    public class PermisoBLL
    {
        private readonly IPermisoDAL _permisoDAL;
        public PermisoBLL(IPermisoDAL permisoDAL) => _permisoDAL = permisoDAL;
        public HashSet<int> ObtenerPermisosPorUsuarioId(int idUsuario)
        {
            return _permisoDAL.ObtenerPermisosPorUsuarioId(idUsuario);
        }
        public DataTable ObtenerPermisosConcedidos(int usuarioId)
        {
            return _permisoDAL.ObtenerPermisosConcedidos(usuarioId);
        }
        public DataTable ObtenerPermisosDeCatalogo()
        {
            return _permisoDAL.ObtenerPermisosDeCatalogo();
        }
        public void InsertarPermiso(int idUsuario, int permisoId)
        {
            _permisoDAL.InsertarPermiso(idUsuario, permisoId);
        }

        public void EliminarPermiso(int idUsuario, int permisoId)
        {
            _permisoDAL.EliminarPermiso(idUsuario, permisoId);
        }

        public void InsertarPermisos(int idUsuario, IEnumerable<int> permisosIds)
        {
            _permisoDAL.InsertarPermisos(idUsuario, permisosIds);
        }

        public int EliminarPermisos(int idUsuario)
        {
            return _permisoDAL.EliminarPermisos(idUsuario);
        }
    }
}
