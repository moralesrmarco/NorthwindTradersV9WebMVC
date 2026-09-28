using System.Security.Claims;

namespace NorthwindTradersV9Common
{
    public static class PermisoExtensions
    {
        public static bool TienePermiso(
            this ClaimsPrincipal usuario,
            int permisoId)
        {
            return usuario.HasClaim(
                "Permiso",
                permisoId.ToString());
        }

    }
}
