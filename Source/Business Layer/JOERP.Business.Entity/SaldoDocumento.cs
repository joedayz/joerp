
namespace JOERP.Business.Entity
{
    using System;

    public class SaldoDocumento
    {
        public int IdSaldoDocumento { get; set; }
        public int IdTransaccion { get; set; }
        public int IdPersona { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int TipoFuncion { get; set; }
        public int IdTipoDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public decimal Importe { get; set; }
        public decimal Saldo { get; set; }
        public int IdMoneda { get; set; }
        public string NumeroLetra { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int SignoOperacion { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Moneda Moneda { get; set; }
        public Persona Persona { get; set; }
        public Transaccion Transaccion { get; set; }
    }
}