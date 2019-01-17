
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class EmpresaValidation
    {
        [DataType(DataType.Text)]
        [DisplayName("Razon Social :")]
        [Required(ErrorMessage = "Razon Social requerida")]
        public string RazonSocial { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("RUC :")]
        [StringLength(11, ErrorMessage = "El RUC no debe ser mas de 11 caracteres.")]
        [Required(ErrorMessage = "RUC requerido")]
        public string RUC { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Dirección :")]
        public string Direccion { get; set; }

        [DataType(DataType.Text, ErrorMessage = "El telefono solo admite números")]
        [DisplayName("Telefono :")]
        [StringLength(20, ErrorMessage = "El telefono no debe ser mas de 20 caracteres.")]
        public string Telefono { get; set; }

        [DataType(DataType.Text, ErrorMessage = "El celular solo admite números")]
        [DisplayName("Celular :")]
        [StringLength(20, ErrorMessage = "El celular no debe ser mas de 20 caracteres.")]
        public string Celular { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Fax :")]
        [StringLength(20, ErrorMessage = "El fax no debe ser mas de 20 caracteres.")]
        public string Fax { get; set; }

        [DataType(DataType.Url, ErrorMessage = "Formato de Url incorrecto")]
        [RegularExpression(@"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$", ErrorMessage = "Formato de URL inválido")]
        [DisplayName("Pagina Web :")]
        public string PaginaWeb { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Formato de Correo Electronico incorrecto")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Formato de E-mail inválido.")]
        [DisplayName("E-mail :")]
        public string CorreoElectronico { get; set; }

        [DisplayName("Código Postal :")]
        public int CodigoPostal { get; set; }

        [DisplayName("Estado :")]
        public int Estado { get; set; }

        [DisplayName("Actividad Económica :")]
        public int ActividadEconomica { get; set; }

        [DisplayName("Tipo de Contribuyente :")]
        public int TipoContribuyente { get; set; }

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
