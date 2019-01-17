
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class UsuarioValidation
    {
        [Required(ErrorMessage = "Cargo requerido")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione el cargo.")]
        [DisplayName("Cargo :")]
        public int IdCargo { get; set; }

        [Required(ErrorMessage = "Empleado requerido")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione el cargo.")]
        [DisplayName("Empleado :")]
        public string IdEmpleado { get; set; }

        [Required(ErrorMessage = "Username requerido")]
        [DisplayName("UserName :")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password requerido")]
        [DisplayName("Password :")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Rol requerido")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione un Rol.")]
        [DisplayName("Rol :")]
        public int IdRol { get; set; }

        [DisplayName("Estado :")]
        public int Estado { get; set; }
    }
}
