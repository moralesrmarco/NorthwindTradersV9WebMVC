using Microsoft.Extensions.Options;
using NorthwindTradersV9Common;
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
        public Producto? ObtenerProductoPorId(int id)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _productoDAL.ObtenerProductoPorId(id);
        }
        public ResultadoOperacion Eliminar(Producto producto)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _productoDAL.Eliminar(producto);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else if (numRegs == -1)
                resultado.Mensaje = StringsCommons.Nfefe;
            else if (numRegs == -2)
                resultado.Mensaje = StringsCommons.Nfefm;
            else if (numRegs == -3)
                resultado.Mensaje = StringsCommons.Nferr;
            else
                resultado.Mensaje = StringsCommons.Nfemd;
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return resultado;
        }
        public ResultadoOperacion Insertar(Producto producto)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _productoDAL.Insertar(producto);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return resultado;
        }
        public ResultadoOperacion Actualizar(Producto producto)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _productoDAL.Actualizar(producto);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else if (numRegs == -1)
                resultado.Mensaje = StringsCommons.Nfmfe;
            else if (numRegs == -2)
                resultado.Mensaje = StringsCommons.Nfmfm;
            else
                resultado.Mensaje = StringsCommons.Nfmmd;
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return resultado;
        }
        public List<ProductoDto> ObtenerProductosRpt(ProductosBuscarDto criterios)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _productoDAL.ObtenerProductosRpt(criterios);
        }
    }
}
