using Microsoft.Extensions.Options;
using NorthwindTradersV9DAL;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9BLL
{
    public class CategoriaBLL
    {
        private readonly ICategoriaDAL _categoriaDAL;
        private readonly AppSettings _appSettings;
        public CategoriaBLL(ICategoriaDAL categoriaDAL, IOptions<AppSettings> appSettings)
        {
            _categoriaDAL = categoriaDAL;
            _appSettings = appSettings.Value;
        }
        public CategoriaPaginadoDto ObtenerCategoriasPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            CategoriasBuscarDto filtro)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _categoriaDAL.ObtenerCategoriasPaginadosConBusqueda(
                pageIndex,
                pageSize,
                filtro);
        }
        public Categoria? ObtenerCategoriaPorId(int id)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _categoriaDAL.ObtenerCategoriaPorId(id);
        }

    }
}
