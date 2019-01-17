
namespace JOERP.Business.Entity
{
    using System;

    public class MovimientoSaldoDocumento
    {
        public int IdMovimientoSaldoDocumento { get; set; }
        public int IdTransaccion { get; set; }
        public int IdSaldoDocumento { get; set; }
        public int Secuencia { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int IdFormaPago { get; set; }
        public int? IdEntidadFinancieraEmpresa { get; set; }
        public int? IdEntidadFinancieraPersona { get; set; }
        public decimal Importe { get; set; }
        public int? IdTipoDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public int IdMoneda { get; set; }
        public string NumeroOperacionBancaria { get; set; }
        public DateTime? FechaOperacionBancaria { get; set; }
        public int SignoOperacion { get; set; }
        public string Comentario { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
    }
}