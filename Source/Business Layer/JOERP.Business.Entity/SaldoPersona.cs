
namespace JOERP.Business.Entity
{
    using System;

    public class SaldoPersona
    {
        public int IdEmpresa { get; set; }
        public int IdPersona { get; set; }
        public int TipoFuncion { get; set; }
        public int IdMoneda { get; set; }
        public DateTime FechaProceso { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal ImporteCargo { get; set; }
        public decimal ImporteAbono { get; set; }
        public Empresa Empresa { get; set; }
        public Moneda Moneda { get; set; }
        public Persona Persona { get; set; }
    }
}