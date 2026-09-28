using NorthwindTradersV9DAL;

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

    }
}
