namespace NorthwindTradersV9Entities.DTOs
{
    public class ActualizarEncabezadoRequest
    {
        public int OrderID { get; set; }
        public string CustomerID { get; set; } = "";
        public int EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderTime { get; set; }
        public DateTime? RequiredDate { get; set; }
        public string? RequiredTime { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string? ShippedTime { get; set; }
        public string RowVersion { get; set; } = "";
    }
}
