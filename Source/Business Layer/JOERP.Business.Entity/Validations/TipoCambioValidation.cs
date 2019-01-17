
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class TipoCambioValidation
    {
        [DataType(DataType.Date)]
        [DisplayName("Fecha :")]
        [Required(ErrorMessage = "Fecha requerida")]
        public DateTime Fecha { get; set; }

        [DataType(DataType.Currency)]
        [Range(0.01, 100, ErrorMessage = "Ingrese un valor de compra correcto")]
        [DisplayName("Valor Compra :")]
        public double ValorCompra { get; set; }

        [DataType(DataType.Currency)]
        [Range(0.01, 100, ErrorMessage = "Ingrese un valor de venta correcto")]
        [DisplayName("Valor Venta :")]
        public double ValorVenta { get; set; }

        [DisplayName("Moneda :")]
        public int IdMoneda { get; set; }

        [DisplayName("Tipo Calculo :")]
        public int TipoCalculo { get; set; }
    }
}
