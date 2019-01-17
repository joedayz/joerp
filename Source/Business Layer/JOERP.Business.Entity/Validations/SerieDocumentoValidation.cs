
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SerieDocumentoValidation
    {
        [Required(ErrorMessage = "Numero actual requerido")]
        [DisplayName("Número Actual :")]
        [StringLength(8, ErrorMessage = "Número actual no puede superar los 8 dígitos")]
        public string NumeroActual { get; set; }

        [Required(ErrorMessage = "Número inicial requerido")]
        [DisplayName("Número Inicio :")]
        [StringLength(8, ErrorMessage = "Número inicial no puede superar los 8 dígitos")]
        public string NumeroInicio { get; set; }

        [Required(ErrorMessage = "Número final requerido")]
        [DisplayName("Número Final :")]
        [StringLength(8, ErrorMessage = "Número final no puede superar los 8 dígitos")]
        public string NumeroFinal { get; set; }

        [Required(ErrorMessage = "Serie requerido")]
        [DisplayName("Serie :")]
        [StringLength(4, ErrorMessage = "Serie no puede superar los 4 dígitos")]
        public string Serie { get; set; }

        [Required(ErrorMessage = "Tipo de Documento requerido")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione el tipo de documento.")]
        [DisplayName("Tipo Documento :")]
        public int TipoDocumento { get; set; }
    }
}
