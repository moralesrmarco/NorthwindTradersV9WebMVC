using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public class VentaDAL : IVentaDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public VentaDAL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public VentaPaginadoDto ObtenerVentasPaginadasConBusqueda(
            int pageIndex,
            int pageSize,
            VentasBuscarDto filtro)
        {
            VentaPaginadoDto resultado = new VentaPaginadoDto();
            try
            {
                using(SqlConnection cn = _connectionFactory.CreateConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("SpVentasBuscarConPaginacion", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parámetros de paginación
                        cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);

                        // Identificadores
                        cmd.Parameters.AddWithValue(
                            "@IdIni", (object?)filtro.IdIni ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@IdFin", (object?)filtro.IdFin ?? DBNull.Value);

                        // Cliente
                        cmd.Parameters.AddWithValue(
                            "@Cliente", (object?)filtro.Cliente ?? "");

                        // Fecha de venta
                        cmd.Parameters.AddWithValue("@FVenta", filtro.FVenta);

                        cmd.Parameters.AddWithValue(
                            "@FVentaIni",
                            (object?)filtro.FVentaIni ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FVentaFin",
                            filtro.FVentaFin.HasValue
                                ? filtro.FVentaFin.Value.Date.AddDays(1)
                                : DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FVentaNull", filtro.FVentaNull);

                        // Fecha requerida
                        cmd.Parameters.AddWithValue(
                            "@FRequerido", filtro.FRequerido);

                        cmd.Parameters.AddWithValue(
                            "@FRequeridoIni",
                            (object?)filtro.FRequeridoIni ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FRequeridoFin",
                            filtro.FRequeridoFin.HasValue
                                ? filtro.FRequeridoFin.Value.Date.AddDays(1)
                                : DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FRequeridoNull", filtro.FRequeridoNull);

                        // Fecha de envío
                        cmd.Parameters.AddWithValue(
                            "@FEnvio", filtro.FEnvio);

                        cmd.Parameters.AddWithValue(
                            "@FEnvioIni",
                            (object?)filtro.FEnvioIni ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FEnvioFin",
                            filtro.FEnvioFin.HasValue
                                ? filtro.FEnvioFin.Value.Date.AddDays(1)
                                : DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@FEnvioNull", filtro.FEnvioNull);

                        // Empleado
                        cmd.Parameters.AddWithValue(
                            "@Empleado", (object?)filtro.Empleado ?? "");

                        // Compañía transportista
                        cmd.Parameters.AddWithValue(
                            "@CompañiaT", (object?)filtro.CompañiaT ?? "");

                        // Dirigido a
                        cmd.Parameters.AddWithValue(
                            "@DirigidoA", (object?)filtro.DirigidoA ?? "");
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                resultado.TotalRegistros = dr.GetInt32(dr.GetOrdinal("TotalRegistros"));
                                resultado.PageIndex = dr.GetInt32(dr.GetOrdinal("PageIndex"));
                            }
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    VentaDto venta = new VentaDto
                                    {
                                        OrderID = dr.GetInt32(
                                            dr.GetOrdinal("OrderID")),
                                        CustomerCompanyName = dr.IsDBNull(
                                            dr.GetOrdinal("CustomerCompanyName"))
                                                ? null
                                                : dr.GetString(
                                                    dr.GetOrdinal("CustomerCompanyName")),
                                        CustomerContactName = dr.IsDBNull(
                                            dr.GetOrdinal("ContactName"))
                                                ? null
                                                : dr.GetString(
                                                    dr.GetOrdinal("ContactName")),
                                        OrderDate = dr.IsDBNull(
                                            dr.GetOrdinal("OrderDate"))
                                                ? null
                                                : dr.GetDateTime(
                                                    dr.GetOrdinal("OrderDate")),
                                        RequiredDate = dr.IsDBNull(
                                            dr.GetOrdinal("RequiredDate"))
                                                ? null
                                                : dr.GetDateTime(
                                                    dr.GetOrdinal("RequiredDate")),
                                        ShippedDate = dr.IsDBNull(
                                            dr.GetOrdinal("ShippedDate"))
                                                ? null
                                                : dr.GetDateTime(
                                                    dr.GetOrdinal("ShippedDate")),
                                        RowVersion = dr.IsDBNull(
                                            dr.GetOrdinal("RowVersion"))
                                                ? null
                                                : (byte[])dr["RowVersion"]
                                    };
                                    resultado.Ventas.Add(venta);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                "Error al obtener las ventas paginadas " + ex.Message);
            }
            return resultado;
        }
    }
}
