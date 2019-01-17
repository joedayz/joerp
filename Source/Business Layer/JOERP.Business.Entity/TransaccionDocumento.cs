
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Validations;

    [MetadataType(typeof(TransaccionDocumentoValidation))]
    public class TransaccionDocumento
    {
        public int IdTransaccionDocumento { set; get; }
        public int IdTransaccion { set; get; }
        public int IdImpuesto { set; get; }
        public int IdTipoDocumento { set; get; }
        public string SerieDocumento { set; get; }
        public string NumeroDocumento { set; get; }
        public DateTime FechaDocumento { set; get; }
        public string FechaFormat { get; set; }
        public int IdMoneda { set; get; }
        public decimal TipoCambio { set; get; }
        public decimal Monto { set; get; }  
        public string Comentarios { set; get; }
        public DateTime FechaCreacion { set; get; }
        public int UsuarioCreacion { set; get; }
        public string Documento { set; get; }
        public string MonedaNombre { set; get; }
        public string ImpuestoNombre { set; get; }

        public IList<ItemTabla> TiposDocumentos { get; set; }
        public IList<Moneda> Monedas { get; set; }
        public IList<Impuesto> Impuestos { get; set; } 
    }
}
