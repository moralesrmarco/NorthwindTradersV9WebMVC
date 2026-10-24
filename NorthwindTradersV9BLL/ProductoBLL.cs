using Microsoft.Extensions.Options;
using NorthwindTradersV9DAL;
using NorthwindTradersV9DAL.Helpers;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9BLL
{
    public class ProductoBLL
    {
        private readonly IProductoDAL _productoDAL;
        private readonly AppSettings _appSettings;
        public ProductoBLL(IProductoDAL productoDAL, IOptions<AppSettings> appSettings)
        {
            _productoDAL = productoDAL;
            _appSettings = appSettings.Value;
        }
        public ProductoPaginadoDto ObtenerProductosPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            ProductosBuscarDto filtro)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _productoDAL.ObtenerProductosPaginadosConBusqueda(
                pageIndex,
                pageSize,
                filtro);
        }
    }
}
