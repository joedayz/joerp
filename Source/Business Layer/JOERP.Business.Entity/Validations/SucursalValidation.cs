
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SucursalValidation
    {
        [DataType(DataType.Text)]
        [DisplayName("Nombre :")]
        [StringLength(100, ErrorMessage = "No debe exeder los 100 caracteres.", MinimumLength = 2)]
        [Required(ErrorMessage = "Nombre requerido")]
        public string Nombre { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Empresa :")]
        [Required(ErrorMessage = "Empresa requerida")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione la Empresa.")]
        public string IdEmpresa { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Dirección :")]
        [StringLength(100, ErrorMessage = "No debe exeder los 100 caracteres.", MinimumLength = 2)]
        public string Direccion { get; set; }

        [DisplayName("Telefono :")]
        [StringLength(20, ErrorMessage = "El telefono no debe ser mas de 20 caracteres.")]
        public string Telefono { get; set; }

        [DisplayName("E-mail :")]
        [StringLength(100, ErrorMessage = "No debe exeder los 100 caracteres.", MinimumLength = 2)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Formato de E-mail inválido.")]
        public string Email { get; set; }

        [DisplayName("Es Sucursal Central :")]
        public string EsPrincipal { get; set; }

        [DisplayName("Estado :")]
        public int Estado { get; set; }

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
