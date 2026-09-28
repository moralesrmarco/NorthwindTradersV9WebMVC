namespace NorthwindTradersV9DAL
{
    public interface IPermisoDAL
    {
        HashSet<int> ObtenerPermisosPorUsuarioId(int idUsuario);

    }
}
