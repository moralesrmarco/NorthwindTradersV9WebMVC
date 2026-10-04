using System.ComponentModel.DataAnnotations;

namespace NorthwindTradersV9Entities.DTOs
{
    public class ClientesBuscarDto
    {
        private string? _customerID;
        [StringLength(5, ErrorMessage = "El ID no puede exceder de 5 caracteres")]
        [RegularExpression(@"^[A-Za-z0-9]{1,5}$", ErrorMessage = "El ID debe tener hasta 5 caracteres alfanuméricos")]
        public string? CustomerID
        {
            get => _customerID;
            set => _customerID = value?.ToUpper();
        }
        [StringLength(40, ErrorMessage = "El nombre de compañía no puede exceder de 40 caracteres")]
        public string? CompanyName { get; set; }
        [StringLength(30, ErrorMessage = "El nombre del contacto no puede exceder de 30 caracteres")]
        public string? ContactName { get; set; }
        [StringLength(60, ErrorMessage = "El domicilio no puede exceder de 60 caracteres")]
        public string? Address { get; set; }
        [StringLength(15, ErrorMessage = "La ciudad no puede exceder de 15 caracteres")]
        public string? City { get; set; }
        [StringLength(15, ErrorMessage = "La región no puede exceder de 15 caracteres")]
        public string? Region { get; set; }
        [StringLength(10, ErrorMessage = "El código postal no puede exceder de 10 caracteres")]
        public string? PostalCode { get; set; }
        [StringLength(15, ErrorMessage = "El país no puede exceder de 15 caracteres")]
        public string? Country { get; set; }
        [StringLength(24, ErrorMessage = "El teléfono no puede exceder de 24 caracteres")]
        public string? Phone { get; set; }
        [StringLength(24, ErrorMessage = "El fax no puede exceder de 24 caracteres")]
        public string? Fax { get; set; }
    }
}
