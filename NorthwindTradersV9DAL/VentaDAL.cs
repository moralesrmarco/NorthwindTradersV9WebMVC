using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities;
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
        public VentaDto? ObtenerVentaPorId(int id)
        {
            VentaDto? venta = null;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpVentaObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", id);
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (rdr.Read())
                        {
                            venta = new VentaDto()
                            {
                                OrderID = rdr.GetInt32(rdr.GetOrdinal("OrderID")),
                                CustomerCompanyName = rdr.IsDBNull(rdr.GetOrdinal("CustomerCompanyName")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerCompanyName")),
                                OrderDate = rdr.IsDBNull(rdr.GetOrdinal("OrderDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("OrderDate")),
                                RequiredDate = rdr.IsDBNull(rdr.GetOrdinal("RequiredDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("RequiredDate")),
                                ShippedDate = rdr.IsDBNull(rdr.GetOrdinal("ShippedDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("ShippedDate")),
                                EmployeeName =
                                    (rdr.IsDBNull(rdr.GetOrdinal("LastName")) ? "" : rdr.GetString(rdr.GetOrdinal("LastName")))
                                    + ", " +
                                    (rdr.IsDBNull(rdr.GetOrdinal("FirstName")) ? "" : rdr.GetString(rdr.GetOrdinal("FirstName"))),
                                ShipperCompanyName = rdr["ShipperCompanyName"] == DBNull.Value
                                    ? string.Empty
                                    : rdr["ShipperCompanyName"].ToString(),
                                ShipName = rdr.IsDBNull(rdr.GetOrdinal("ShipName")) ? null : rdr.GetString(rdr.GetOrdinal("ShipName")),
                                ShipAddress = rdr.IsDBNull(rdr.GetOrdinal("ShipAddress")) ? null : rdr.GetString(rdr.GetOrdinal("ShipAddress")),
                                ShipCity = rdr.IsDBNull(rdr.GetOrdinal("ShipCity")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCity")),
                                ShipRegion = rdr.IsDBNull(rdr.GetOrdinal("ShipRegion")) ? null : rdr.GetString(rdr.GetOrdinal("ShipRegion")),
                                ShipPostalCode = rdr.IsDBNull(rdr.GetOrdinal("ShipPostalCode")) ? null : rdr.GetString(rdr.GetOrdinal("ShipPostalCode")),
                                ShipCountry = rdr.IsDBNull(rdr.GetOrdinal("ShipCountry")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCountry")),
                                Freight = rdr.IsDBNull(rdr.GetOrdinal("Freight")) ? (decimal?)null : rdr.GetDecimal(rdr.GetOrdinal("Freight")),
                                RowVersion = rdr.IsDBNull(rdr.GetOrdinal("RowVersion"))
                                    ? null
                                    : (byte[])rdr["RowVersion"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la venta por ID: " + ex.Message);
            }
            return venta;
        }
        public Venta ObtenerVentaPorIdRpt(int orderId)
        {
            Venta venta = null;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpVentaObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!rdr.HasRows)
                            throw new Exception($"SpVentaObtenerPorId no encontró la venta {orderId}");
                        if (rdr.Read())
                        {
                            venta = new Venta()
                            {
                                OrderID = rdr.GetInt32(rdr.GetOrdinal("OrderID")),
                                Cliente = new Cliente
                                {
                                    CustomerId = rdr.IsDBNull(rdr.GetOrdinal("CustomerID")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerID")),
                                    CompanyName = rdr.IsDBNull(rdr.GetOrdinal("CustomerCompanyName")) ? null : rdr.GetString(rdr.GetOrdinal("CustomerCompanyName"))
                                },
                                Empleado = new Empleado
                                {
                                    EmployeeID = rdr.GetInt32(rdr.GetOrdinal("EmployeeID")),
                                    LastName = rdr["LastName"].ToString(),
                                    FirstName = rdr["FirstName"].ToString()
                                },
                                OrderDate = rdr.IsDBNull(rdr.GetOrdinal("OrderDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("OrderDate")),
                                RequiredDate = rdr.IsDBNull(rdr.GetOrdinal("RequiredDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("RequiredDate")),
                                ShippedDate = rdr.IsDBNull(rdr.GetOrdinal("ShippedDate")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("ShippedDate")),
                                Transportista = new Transportista
                                {
                                    ShipperID = rdr.GetInt32(rdr.GetOrdinal("ShipVia")),
                                    CompanyName = rdr["ShipperCompanyName"] == DBNull.Value
                                      ? string.Empty
                                      : rdr["ShipperCompanyName"].ToString()
                                },
                                Freight = rdr.IsDBNull(rdr.GetOrdinal("Freight")) ? (decimal?)null : rdr.GetDecimal(rdr.GetOrdinal("Freight")),
                                ShipName = rdr.IsDBNull(rdr.GetOrdinal("ShipName")) ? null : rdr.GetString(rdr.GetOrdinal("ShipName")),
                                ShipAddress = rdr.IsDBNull(rdr.GetOrdinal("ShipAddress")) ? null : rdr.GetString(rdr.GetOrdinal("ShipAddress")),
                                ShipCity = rdr.IsDBNull(rdr.GetOrdinal("ShipCity")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCity")),
                                ShipRegion = rdr.IsDBNull(rdr.GetOrdinal("ShipRegion")) ? null : rdr.GetString(rdr.GetOrdinal("ShipRegion")),
                                ShipPostalCode = rdr.IsDBNull(rdr.GetOrdinal("ShipPostalCode")) ? null : rdr.GetString(rdr.GetOrdinal("ShipPostalCode")),
                                ShipCountry = rdr.IsDBNull(rdr.GetOrdinal("ShipCountry")) ? null : rdr.GetString(rdr.GetOrdinal("ShipCountry")),
                                RowVersion = rdr.IsDBNull(rdr.GetOrdinal("RowVersion")) ? null : (byte[])rdr["RowVersion"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la venta por ID: " + ex.Message);
            }
            return venta;
        }
        public int Eliminar(VentaDto venta, out string productoExcede)
        {
            int numRegs = 0;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpVentaEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", venta.OrderID);
                    cmd.Parameters.AddWithValue("@RowVersion", venta.RowVersion ?? (object)DBNull.Value);
                    var paramProductoExcede = cmd.Parameters.Add("@ProductoExcede", SqlDbType.VarChar, 40);
                    paramProductoExcede.Direction = ParameterDirection.Output;
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    paramProductoExcede.Value = paramProductoExcede.Value == DBNull.Value ? string.Empty : paramProductoExcede.Value;
                    numRegs = (int)returnParameter.Value;
                    productoExcede = paramProductoExcede.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la venta " + ex.Message);
            }
            return numRegs;
        }
        public decimal? ObtenerTasaVigente(DateTime fecha)
        {
            using SqlConnection cn = _connectionFactory.CreateConnection();
            using SqlCommand cmd =
                new("SpTasaImpuestoObtenerVigente", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2)
                .Value = fecha;
            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return Convert.ToDecimal(dr["Tasa"]);
            }
            return null;
        }
        public EnvioInformacionDto? ObtenerUltimaInformacionDeEnvio(string customerId)
        {
            EnvioInformacionDto? envioInformacion = null;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpVentaObtenerUltimaInformacionDeEnvio", con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return null;
                    DataRow row = dt.Rows[0];
                    envioInformacion = new EnvioInformacionDto
                    {
                        ShipName = row["ShipName"] == DBNull.Value ? string.Empty : row["ShipName"].ToString(),
                        ShipAddress = row["ShipAddress"] == DBNull.Value ? string.Empty : row["ShipAddress"].ToString(),
                        ShipCity = row["ShipCity"] == DBNull.Value ? string.Empty : row["ShipCity"].ToString(),
                        ShipRegion = row["ShipRegion"] == DBNull.Value ? string.Empty : row["ShipRegion"].ToString(),
                        ShipPostalCode = row["ShipPostalCode"] == DBNull.Value ? string.Empty : row["ShipPostalCode"].ToString(),
                        ShipCountry = row["ShipCountry"] == DBNull.Value ? string.Empty : row["ShipCountry"].ToString(),
                        ShipVia = row["ShipVia"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(row["ShipVia"])
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la información de envío: " + ex.Message);
            }
            return envioInformacion;
        }
        public List<EnvioInformacionDto> ObtenerFormasEnvio(string customerID)
        {
            List<EnvioInformacionDto> lista = new();
            using (SqlConnection cn = _connectionFactory.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SpVentaObtenerFormasEnvio", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue(
                        "@CustomerID",
                        customerID
                    );
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new EnvioInformacionDto
                            {
                                CompanyName = dr["CompanyName"].ToString() ?? "",
                                ShipName = dr["ShipName"] == DBNull.Value
                                    ? null
                                    : dr["ShipName"].ToString(),
                                ShipAddress = dr["ShipAddress"] == DBNull.Value
                                    ? null
                                    : dr["ShipAddress"].ToString(),
                                ShipCity = dr["ShipCity"] == DBNull.Value
                                    ? null
                                    : dr["ShipCity"].ToString(),
                                ShipRegion = dr["ShipRegion"] == DBNull.Value
                                    ? null
                                    : dr["ShipRegion"].ToString(),
                                ShipPostalCode = dr["ShipPostalCode"] == DBNull.Value
                                    ? null
                                    : dr["ShipPostalCode"].ToString(),
                                ShipCountry = dr["ShipCountry"] == DBNull.Value
                                    ? null
                                    : dr["ShipCountry"].ToString(),
                                ShipVia = dr["ShipVia"] == DBNull.Value
                                    ? null
                                    : Convert.ToInt32(dr["ShipVia"])
                            });
                        }
                    }
                }
            }
            return lista;
        }
        public int InsertarVentaCompleta(Venta venta, out int orderId, out byte[] rowVersion)
        {
            orderId = 0;
            rowVersion = null;
            int filasAfectadas = 0;
            try
            {
                using (var cn = _connectionFactory.CreateConnection())
                {
                    cn.Open();
                    using (var tx = cn.BeginTransaction())
                    {
                        try
                        {
                            // 1) Insertar registro padre (SP)
                            using (var cmd = new SqlCommand("SpVentaInsertar", cn, tx))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                // Parámetro de retorno
                                var returnParam = cmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                                returnParam.Direction = ParameterDirection.ReturnValue;

                                // Parámetros de salida
                                cmd.Parameters.Add("@OrderID", SqlDbType.Int).Direction = ParameterDirection.Output;
                                // RowVersion
                                cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;
                                // Parámetros de entrada
                                cmd.Parameters.AddWithValue("@CustomerID", string.IsNullOrWhiteSpace(venta.Cliente.CustomerID) ? (object)DBNull.Value : venta.Cliente.CustomerID);
                                cmd.Parameters.AddWithValue("@EmployeeID", venta.Empleado.EmployeeID);
                                cmd.Parameters.AddWithValue("@OrderDate", venta.OrderDate.HasValue ? (object)venta.OrderDate.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@RequiredDate", venta.RequiredDate.HasValue ? (object)venta.RequiredDate.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@ShippedDate", venta.ShippedDate.HasValue ? (object)venta.ShippedDate.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@ShipVia", ((object)venta.Transportista.ShipperID == null || venta.Transportista.ShipperID.Equals(0)) ? DBNull.Value : (object)venta.Transportista.ShipperID);
                                cmd.Parameters.AddWithValue("@ShipName", string.IsNullOrWhiteSpace(venta.ShipName) ? (object)DBNull.Value : venta.ShipName);
                                cmd.Parameters.AddWithValue("@ShipAddress", string.IsNullOrWhiteSpace(venta.ShipAddress) ? (object)DBNull.Value : venta.ShipAddress);
                                cmd.Parameters.AddWithValue("@ShipCity", string.IsNullOrWhiteSpace(venta.ShipCity) ? (object)DBNull.Value : venta.ShipCity);
                                cmd.Parameters.AddWithValue("@ShipRegion", string.IsNullOrWhiteSpace(venta.ShipRegion) ? (object)DBNull.Value : venta.ShipRegion);
                                cmd.Parameters.AddWithValue("@ShipPostalCode", string.IsNullOrWhiteSpace(venta.ShipPostalCode) ? (object)DBNull.Value : venta.ShipPostalCode);
                                cmd.Parameters.AddWithValue("@ShipCountry", string.IsNullOrWhiteSpace(venta.ShipCountry) ? (object)DBNull.Value : venta.ShipCountry);
                                cmd.Parameters.AddWithValue("@Freight", (object)venta.Freight ?? DBNull.Value);

                                // Ejecutar y capturar código de retorno
                                cmd.ExecuteNonQuery();
                                // Capturar código de retorno
                                var returnValue = (int)returnParam.Value;
                                // filasAfectadas no depende de ExecuteNonQuery aquí,
                                // sino del código de retorno del SP
                                filasAfectadas = returnValue;
                                if (returnValue != 1)
                                {
                                    throw new InvalidOperationException("Error al insertar la venta. Código de error: " + returnValue);
                                }
                                // Capturar valores de salida
                                orderId = Convert.ToInt32(cmd.Parameters["@OrderID"].Value);
                                // Aquí obtienes el RowVersion como arreglo de bytes
                                rowVersion = (byte[])cmd.Parameters["@RowVersion"].Value;

                                // Si quieres guardarlo en tu objeto Venta:
                                venta.RowVersion = rowVersion;
                            }
                            // 2) Preparar comandos reutilizables para cada detalle:
                            // 2.1) Preparar SELECT UnitsInStock FOR UPDATE
                            using (var cmdCheckStock = new SqlCommand("SpProductoObtenerInventarioPorIdConBloqueo", cn, tx)) // el bloqueo persiste para las demas operaciones dentro de la transacción
                            {
                                cmdCheckStock.CommandType = CommandType.StoredProcedure;
                                cmdCheckStock.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int));

                                // 2.2) Preparar UPDATE products
                                using (var cmdUpdateStock = new SqlCommand("SpProductoActualizarInventarioPorId", cn, tx))
                                {
                                    cmdUpdateStock.CommandType = CommandType.StoredProcedure;
                                    cmdUpdateStock.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int));
                                    cmdUpdateStock.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int));

                                    // 2.3) Preparar inserción de detalle (SP)
                                    using (var cmdInsertDetail = new SqlCommand("SpVentaDetalleInsertarSinActualizarInventario2", cn, tx))
                                    {
                                        cmdInsertDetail.CommandType = CommandType.StoredProcedure;
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@UnitPrice", SqlDbType.Decimal));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.SmallInt));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@Discount", SqlDbType.Float));
                                        cmdInsertDetail.Parameters.Add(new SqlParameter("@TasaIVA", SqlDbType.Float));

                                        // 3) Procesar cada detalle
                                        foreach (var d in venta.VentaDetalles)
                                        {
                                            // 3.1) Validar existencia y bloquear fila del producto
                                            cmdCheckStock.Parameters["@ProductID"].Value = d.Producto.ProductID;
                                            var stockObj = cmdCheckStock.ExecuteScalar();
                                            if (stockObj == null || stockObj == DBNull.Value)
                                            {
                                                throw new InvalidOperationException($"Producto {d.Producto.ProductID} no existe.");
                                            }

                                            int currentStock = Convert.ToInt32(stockObj);

                                            // 3.2) Validar stock suficiente
                                            if (currentStock < d.Quantity)
                                            {
                                                throw new InvalidOperationException($"Inventario insuficiente para el producto {d.Producto.ProductID} {d.Producto.ProductName}. Disponible: {currentStock}, solicitado: {d.Quantity}.");
                                            }

                                            // 3.3) Actualizar stock
                                            cmdUpdateStock.Parameters["@Quantity"].Value = d.Quantity;
                                            cmdUpdateStock.Parameters["@ProductID"].Value = d.Producto.ProductID;
                                            var rowsUpdated = cmdUpdateStock.ExecuteNonQuery();
                                            if (rowsUpdated == 0)
                                            {
                                                throw new InvalidOperationException($"No se pudo actualizar el inventario para el producto {d.Producto.ProductID}.");
                                            }

                                            // 3.4) Insertar detalle (SP)
                                            cmdInsertDetail.Parameters["@OrderID"].Value = orderId;
                                            cmdInsertDetail.Parameters["@ProductID"].Value = d.Producto.ProductID;
                                            cmdInsertDetail.Parameters["@UnitPrice"].Value = d.UnitPrice;
                                            cmdInsertDetail.Parameters["@Quantity"].Value = d.Quantity;
                                            cmdInsertDetail.Parameters["@Discount"].Value = d.Discount;
                                            cmdInsertDetail.Parameters["@TasaIVA"].Value = d.TasaIVA;

                                            filasAfectadas += cmdInsertDetail.ExecuteNonQuery();
                                        } // foreach detalles
                                    } // cmdInsertDetail
                                } // cmdUpdateStock
                            } // cmdCheckStock
                            tx.Commit();
                        }
                        catch (Exception)
                        {
                            try { tx.Rollback(); } catch { }
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar la venta: " + ex.Message);
            }
            return filasAfectadas;
        }
    }
}
