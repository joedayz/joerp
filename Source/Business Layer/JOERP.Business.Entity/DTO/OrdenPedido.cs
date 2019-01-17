
namespace JOERP.Business.Entity.DTO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Validations;

    [MetadataType(typeof(OrdenPedidoValidation))]
    public class OrdenPedido : Transaccion
    {
        public DateTime Fecha { get; set; }

        public IList<SerieDocumento> SeriesDoc { set; get; }

        public int IdProveedor { set; get; }

        public IList<Persona> Proveedores { set; get; }

        public string NumeroSerie { set; get; }
        
        public string IdCondicionPago { set; get; }

        public string Observacion { get; set; }

        public IList<MovimientoProducto> DetalleProducto { set; get; }

        public decimal Monto { get; set; }

        public string Orden { get; set; }

        public int IdCondicion { get; set; }

        public int IdEstado { get; set; }

        public int IdConcepto { get; set; }

        public IList<MovimientoProducto> DetalleOrdenPedido { set; get; }
        
        public string FechaDocumentoFormat { get; set; }
    }
}
