
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class TransaccionDocumentoValidation
    {
        [DisplayName("Documento :")]
        [Required(ErrorMessage = "El tipo de documento es requerido.")]
        public int IdTipoDocumento { set; get; }

        [DataType(DataType.Text)]
        [DisplayName("Serie :")]
        [Required(ErrorMessage = "La serie es requerida.")]
        public string SerieDocumento { set; get; }

        [DataType(DataType.Text)]
        [DisplayName("N° Documento :")]
        [Required(ErrorMessage = "El número de documento es requerido.")]
        public string NumeroDocumento { set; get; }

        [DisplayName("Fecha :")]
        [Required(ErrorMessage = "La fecha es requerida.")]
        public DateTime FechaDocumento { get; set; }

        [DisplayName("Concepto :")]
        [Required(ErrorMessage = "El impuesto es requerido.")]
        public int IdImpuesto { get; set; }

        [DisplayName("Moneda :")]
        [Required(ErrorMessage = "El impuesto es requerido.")]
        public int IdMoneda { get; set; }

        [DisplayName("Tipo cambio :")]
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        public decimal TipoCambio { get; set; }

        [DisplayName("Monto :")]
        [Required(ErrorMessage = "El monto es requerido.")]
        public decimal Monto { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Comentarios :")]
        public string Comentarios { set; get; }
    }
}
