using Microsoft.Extensions.Options;
using NorthwindTradersV9Common;
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
        public ResultadoOperacion Eliminar(Categoria categoria)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _categoriaDAL.Eliminar(categoria);
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

    }
}
