
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class OrdenPedidoValidation
    {
        [DisplayName("Documento :")]
        public int IdTipoDocumento { set; get; }

        [DisplayName("Serie :")]
        public int IdSerieDocumento { set; get; }

        [DisplayName("N° Documento :")]
        public string NumeroDocumento { set; get; }

        [DisplayName("Fecha :")]
        public DateTime FechaDocumento { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Cliente RUC/DNI :")]
        [Required(ErrorMessage = "Cliente es requerido")]
        public string Proveedor { set; get; }

        [DataType(DataType.Text)]
        [DisplayName("Razón Social :")]
        public string RazonSocial { get; set; }

        [DisplayName("Dirección Entrega :")]
        public int IdSucursal { set; get; }

        [DisplayName("Almacén :")]
        public int IdAlmacen { set; get; }

        [DisplayName("Fecha entrega :")]
        public DateTime FechaEntrega { get; set; }

        [Range(1, Int16.MaxValue, ErrorMessage = "Condición de pago es requerido.")]
        [DisplayName("Condición Pago :")]
        public int CondicionPago { set; get; }

        [DisplayName("Moneda :")]
        public int IdMoneda { set; get; }

        [DisplayName("Tipo Cambio :")]
        public decimal MontoTipoCambio { set; get; }

        [DisplayName("Estado :")]
        public string EstadoOrden { set; get; }

        [DisplayName("Observaciones :")]
        public string Glosa { set; get; }
    }
}
