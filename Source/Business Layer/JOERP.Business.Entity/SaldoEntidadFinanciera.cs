
namespace JOERP.Business.Entity
{
    using System;

    public class SaldoEntidadFinanciera
    {
        public int IdEmpresa { get; set; }
        public int IdSucursal { get; set; }
        public int IdEntidadFinanciera { get; set; }
        public DateTime FechaProceso { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal Ingreso { get; set; }
        public decimal Egreso { get; set; }
        public Empresa Empresa { get; set; }
        public EntidadFinanciera EntidadFinanciera { get; set; }
        public Sucursal Sucursal { get; set; }
    }
}