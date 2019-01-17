
namespace JOERP.Business.Entity
{
    using System;

    public class MovimientoEntidadFinanciera
    {
        public int IdMovimientoEntidadFinanciera { get; set; }
        public int IdTransaccion { get; set; }
        public int IdEntidadFinanciera { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int Secuencia { get; set; }
        public int IdTipoDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public decimal Importe { get; set; }
        public decimal Saldo { get; set; }
        public int? IdTipoDocumentoCT { get; set; }
        public string SerieDocumentoCT { get; set; }
        public string NumeroDocumentoCT { get; set; }
        public string Comentario { get; set; }
        public int SignoOperacion { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
    }
}