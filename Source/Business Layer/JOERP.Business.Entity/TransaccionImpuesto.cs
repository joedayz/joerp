
namespace JOERP.Business.Entity
{
    using System;

    public class TransaccionImpuesto
    {
        public int IdTransaccion { get; set; }
        public int IdImpuesto { get; set; }
        public string NombreImpuesto { get; set; }
        public int Secuencia { get; set; }
        public decimal Tasa { get; set; }
        public decimal Valor { get; set; }
        public bool EsEditable { get; set; }
        public int Signo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Impuesto Impuesto { get; set; }
        public Transaccion Transaccion { get; set; }
    }
}