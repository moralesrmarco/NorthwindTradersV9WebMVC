using Microsoft.Extensions.Options;
using NorthwindTradersV9DAL;
using NorthwindTradersV9DAL.Helpers;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9BLL
{
    public class TransportistaBLL
    {
        private readonly ComboDataHelper _comboDataHelper;
        public TransportistaBLL(ComboDataHelper comboDataHelper)
        {
            _comboDataHelper = comboDataHelper;
        }
        public List<ComboItemDto> ObtenerTransportistasCbo()
        {
            return _comboDataHelper.LlenarCbo("SpTransportistaObtenerCbo");
        }
    }
}
