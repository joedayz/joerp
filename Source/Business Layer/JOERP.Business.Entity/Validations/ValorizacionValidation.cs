
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class ValorizacionValidation
    {
        [DisplayName("Documento :")]
        [Required(ErrorMessage = "El tipo de documento es requerido.")]
        public int IdTipoDocumento { set; get; }

        [DisplayName("Serie :")]
        [Required(ErrorMessage = "La serie es requerido.")]
        public int IdSerieDocumento { set; get; }

        [DisplayName("Número :")]
        public int NumeroDocumento { set; get; }

        [DisplayName("Fecha :")]
        [Required(ErrorMessage = "La fecha es requerido.")]
        public DateTime FechaRegistro { get; set; }

        [DisplayName("Observaciones :")]
        public string Glosa { set; get; }
    }
}
