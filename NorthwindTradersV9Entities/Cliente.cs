using System.ComponentModel.DataAnnotations;

namespace NorthwindTradersV9Entities
{
    public class Cliente
    {
        private string? _customerId;
        [Required (ErrorMessage = "Ingrese el ID")]
        [StringLength(5, ErrorMessage = "El ID no puede exceder de 5 caracteres")]
        [RegularExpression(@"^[A-Za-z0-9]{5}$", ErrorMessage = "El ID debe tener exactamente 5 caracteres alfanuméricos")]
        public string? CustomerId
        {
            get => _customerId;
            set => _customerId = value?.ToUpper();
        }
        [Required(ErrorMessage = "Ingrese el nombre de compañia")]
        [StringLength(40, ErrorMessage = "El nombre de compañía no puede exceder de 40 caracteres")]
        public string? CompanyName { get; set; }
        [Required(ErrorMessage = "Ingrese el nombre del contacto")]
        [StringLength(30, ErrorMessage = "El nombre del contacto no puede exceder de 30 caracteres")]
        public string? ContactName { get; set; }
        [Required(ErrorMessage = "Ingrese el título de del contacto")]
        [StringLength(30, ErrorMessage = "El título del contacto  no puede exceder de 30 caracteres")]
        public string? ContactTitle { get; set; }
        [Required(ErrorMessage = "Ingrese el domicilio")]
        [StringLength(60, ErrorMessage = "El domicilio no puede exceder de 60 caracteres")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "Ingrese la ciudad")]
        [StringLength(15, ErrorMessage = "La ciudad no puede exceder de 15 caracteres")]
        public string? City { get; set; }
        [StringLength(15, ErrorMessage = "La región no puede exceder de 15 caracteres")]
        public string? Region { get; set; }
        [StringLength(10, ErrorMessage = "El código postal no puede exceder de 10 caracteres")]
        public string? PostalCode { get; set; }
        [Required(ErrorMessage = "Seleccione o escriba un país")]
        [StringLength(15, ErrorMessage = "El país no puede exceder de 15 caracteres")]
        public string? Country { get; set; }
        [Required(ErrorMessage = "Ingrese el teléfono")]
        [StringLength(24, ErrorMessage = "El teléfono no puede exceder de 24 caracteres")]
        public string? Phone { get; set; }
        [StringLength(24, ErrorMessage = "El fax no puede exceder de 24 caracteres")]
        public string? Fax { get; set; }
        public byte[]? RowVersion { get; set; }

        // del diagrama entidad-relación podemos ver que
        // un cliente puede tener muchas ventas asociadas
        //public List<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
