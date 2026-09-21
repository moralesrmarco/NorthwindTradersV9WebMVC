using System.ComponentModel.DataAnnotations;

namespace NorthwindTradersV9Entities
{
    public class Empleado
    {
        // las siguientes 3 propiedades las dejo para que mi ejemplo de empleados funcione.
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; }

        //*******************************************************************************************************************************************
        // las siguientes propiedades son las propiedades ya para mi version final de la clase Empleado, que es la que se usará en el proyecto final.
        //*******************************************************************************************************************************************
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Ingrese apellido")]
        [StringLength(20, ErrorMessage = "Los apellidos no puede exceder de 20 caracteres")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Ingrese nombre")]
        [StringLength(10, ErrorMessage = "El nombre no puede exceder de 10 caracteres")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Ingrese título")]
        [StringLength(30, ErrorMessage = "El título no puede exceder de 30 caracteres")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Ingrese título de cortesía")]
        [StringLength(25, ErrorMessage = "El título de cortesía no puede exceder de 25 caracteres")]
        public string? TitleOfCourtesy { get; set; }

        [Required(ErrorMessage = "Ingrese la fecha de nacimiento")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Ingrese la fecha de contratación")]
        public DateTime? HireDate { get; set; }

        [Required(ErrorMessage = "Ingrese domicilio")]
        [StringLength(60, ErrorMessage = "El domicilio no puede exceder de 60 caracteres")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Ingrese ciudad")]
        [StringLength(15, ErrorMessage = "La ciudad no puede exceder de 15 caracteres")]
        public string? City { get; set; }

        [StringLength(15, ErrorMessage = "La región no puede exceder de 15 caracteres")]
        public string? Region { get; set; }

        [StringLength(10, ErrorMessage = "El código postal no puede exceder de 10 caracteres")]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "Seleccione o escriba un país")]
        [StringLength(15, ErrorMessage = "El país no puede exceder de 15 caracteres")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Ingrese teléfono")]
        [StringLength(24, ErrorMessage = "El teléfono no puede exceder de 24 caracteres")]
        public string? HomePhone { get; set; }

        [StringLength(4, ErrorMessage = "La extensión no puede exceder de 4 caracteres")]
        public string? Extension { get; set; }

        public byte[]? Photo { get; set; }

        public string? Notes { get; set; }

        public string? PhotoPath { get; set; }

        public string? ReportsToName { get; set; }

        public byte[]? RowVersion { get; set; }
        
        // Clave foránea hacia otro empleado
        [Required(ErrorMessage = "Seleccione a quién reporta el empleado")]
        public int? ReportsTo { get; set; }

        public string NameByFirstName
        {
            get { return FirstName + " " + LastName; }
        }

        public string? NameByLastName
        {
            get
            {
                return string.IsNullOrWhiteSpace(FirstName)
                    ? LastName
                    : LastName + ", " + FirstName;
            }
        }

        // Propiedades de navegación (no automáticas en ADO.NET, las llenas tú en la capa DAL/BLL)
        public Empleado? Jefe { get; set; }

        public List<Empleado> EmpleadosSubordinados { get; set; } = new List<Empleado>();

        public override string ToString()
        {
            return NameByFirstName;
        }

        // Propiedades adicionales para facilitar el acceso al nombre del jefe desde el reportviewer
        public string? JefeNameByLastName
        {
            get
            {
                if (Jefe == null)
                    return "N/A";

                if (string.IsNullOrEmpty(Jefe.FirstName))
                    return Jefe.LastName;

                return Jefe.LastName + ", " + Jefe.FirstName;
            }
        }

        public string JefeNameByFirstName
        {
            get { return Jefe != null ? Jefe.NameByFirstName : ""; }
        }

        // del diagrama entidad-relación podemos ver que 
        // un empleado puede tener muchos órdenes asociadas
        //public List<Venta> Ventas { get; set; } = new List<Venta>();

    }
}
