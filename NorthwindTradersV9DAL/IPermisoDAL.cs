using System.Data;

namespace NorthwindTradersV9DAL
{
    public interface IPermisoDAL
    {
        HashSet<int> ObtenerPermisosPorUsuarioId(int idUsuario);
        DataTable ObtenerPermisosConcedidos(int usuarioId);
        DataTable ObtenerPermisosDeCatalogo();
        void InsertarPermiso(int idUsuario, int permisoId);
        void EliminarPermiso(int idUsuario, int permisoId);
        void InsertarPermisos(int idUsuario, IEnumerable<int> permisosIds);
        int EliminarPermisos(int idUsuario);
    }
}
