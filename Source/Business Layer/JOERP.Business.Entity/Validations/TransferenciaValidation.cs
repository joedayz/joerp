
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;

    public class TransferenciaValidation
    {
        [DisplayName("Documento :")]
        public int IdTipoDocumento { set; get; }

        [DisplayName("Serie :")]
        public int IdSerieDocumento { set; get; }

        [DisplayName("N° Documento :")]
        public string NumeroDocumento { set; get; }

        [DisplayName("Fecha :")]
        public DateTime FechaDocumento { get; set; }

        [DisplayName("Sucursal Origen :")]
        public int IdSucursal { set; get; }

        [DisplayName("Almacén Origen:")]
        public int IdAlmacen { set; get; }

        [DisplayName("Sucursal Destino :")]
        public int IdSucursalAlterno { set; get; }
        
        [DisplayName("Almacén Destino :")]
        public int IdAlmacenAlterno { set; get; }

        [DisplayName("Fecha entrega :")]
        public DateTime FechaEntrega { get; set; }
        
        [DisplayName("Moneda :")]
        public int IdMoneda { set; get; }

        [DisplayName("Estado :")]
        public string EstadoOrden { set; get; }

        [DisplayName("Observaciones :")]
        public string Glosa { set; get; }
    }
}
