using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9DAL.Helpers
{
    public class ComboDataHelper
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public ComboDataHelper(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public List<ComboItemDto> LlenarCbo(string storedProcedure, params SqlParameter[] parameters)
        {
            var items = new List<ComboItemDto>();
            var dtTemp = new DataTable();
            try
            {
                using (var cn = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand(storedProcedure, cn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);
                    da.Fill(dtTemp);
                }
                // Insertar fila "Seleccione" al inicio
                items.Add(new ComboItemDto
                {
                    Value = String.Empty,
                    Text = "»--- Seleccione ---«"
                });
                if (storedProcedure == "SpEmpleadoObtenerEmpleadosCbo")
                {
                    items.Add(new ComboItemDto
                    {
                        Value = "-1",
                        Text = "N/A"
                    });
                }

                // Tomamos la primera y segunda columna sin importar el nombre
                foreach (DataRow row in dtTemp.Rows)
                {
                    items.Add(new ComboItemDto
                    {
                        Value = row[0]?.ToString() ?? string.Empty,
                        Text = row[1]?.ToString() ?? string.Empty
                    });
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al llenar el ComboBox: " + ex.Message);
            }
            return items;
        }
    }
}
