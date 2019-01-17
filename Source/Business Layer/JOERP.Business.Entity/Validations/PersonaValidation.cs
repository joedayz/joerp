
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class PersonaValidation
    {
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "La Razon Social no debe ser más de 150 caracteres.", MinimumLength = 2)]
        [DisplayName("Razón Social :")]
        public string RazonSocial { get; set; }

        //  [Required(ErrorMessage = "Nombres requerido")]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "Los Nombres no debe ser más de 100 caracteres.", MinimumLength = 2)]
        [DisplayName("Nombres :")]
        public string Nombres { get; set; }

        //   [Required(ErrorMessage = "Apellido Paterno requerido")]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "El Apellido Paterno no debe ser más de 100 caracteres.", MinimumLength = 2)]
        [DisplayName("Apellido Paterno :")]
        public string ApellidoPaterno { get; set; }

        //    [Required(ErrorMessage = "ApellidoMaterno requerido")]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "El ApellidoMaterno no debe ser más de 100 caracteres.", MinimumLength = 2)]
        [DisplayName("Apellido Materno :")]
        public string ApellidoMaterno { get; set; }

        [DisplayName("Sexo :")]
        [DataType(DataType.Text)]
        public int SexoC { get; set; }

        [StringLength(20, ErrorMessage = "No debe exeder los 20 caracteres y debe tener al menos 6 caracteres.", MinimumLength = 6)]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Formato incorrecto de teléfono")]
        [DisplayName("Teléfono :")]
        public string Telefono { get; set; }

        [StringLength(20, ErrorMessage = "No debe exeder los 20 caracteres y debe tener al menos 9 caracteres", MinimumLength = 8)]
        [DisplayName("Celular :")]
        [DataType(DataType.PhoneNumber)]
        public string Celular { get; set; }

        [Required(ErrorMessage = "Documento requerido")]
        [DataType(DataType.Text)]
        [DisplayName("Documento :")]
        public string Documento { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Fecha de Nacimiento :")]
        public DateTime FechaNacimiento { get; set; }

        [DisplayName("Tipo Documento :")]
        public int TipoDocumento { get; set; }

        [StringLength(150, ErrorMessage = "No debe exeder los 150 caracteres.", MinimumLength = 2)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Formato Incorrecto de Email")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Formato de E-mail inválido.")]
        [DisplayName("E-mail :")]
        public string Email { get; set; }

        [DisplayName("Estado Civil :")]
        public int EstadoCivil { get; set; }

        [DisplayName("FullName :")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Codigo requerido")]
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = "El Codigo no debe ser más de 10 caracteres.")]
        [DisplayName("Código :")]
        public string Codigo { get; set; }

        [DisplayName("Estado :")]
        public int Estado { get; set; }

        [DisplayName("Licencia :")]
        public string NumeroLicencia { get; set; }

        [DisplayName("Dirección :")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Departameto requerido")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione el Departamento.")]
        [DisplayName("Departamento :")]
        public int IdDepartamento { get; set; }

        [Required(ErrorMessage = "Provincia requerido")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione la Provincia.")]
        [DisplayName("Provincia :")]
        public int IdProvincia { get; set; }

        [Required(ErrorMessage = "Distrito requerido")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione el Distrito.")]
        [DisplayName("Distrito :")]
        public int IdUbigeo { get; set; }
    }
}
