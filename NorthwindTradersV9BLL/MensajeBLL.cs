using NorthwindTradersV9DAL;

namespace NorthwindTradersV9BLL
{
    public class MensajeBLL
    {
        private readonly MensajeDAL _mensajeDAL;

        public MensajeBLL(MensajeDAL mensajeDAL)
        {
            _mensajeDAL = mensajeDAL;
        }

        public string ObtenerMensaje()
        {
            return _mensajeDAL.ObtenerMensaje();
        }
    }
}
