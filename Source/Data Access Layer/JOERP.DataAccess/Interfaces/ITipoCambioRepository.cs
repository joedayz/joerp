
namespace JOERP.DataAccess.Interfaces
{
    using System;
    using System.Data.Common;
    using Business.Entity;

    public interface ITipoCambioRepository : IRepository<TipoCambio>
    {
        TipoCambio GetTipoCambio(int idTipoCambio, DbTransaction transaction = null);
        
        decimal GetMontoEnMonedaLocal(int idMoneda, decimal monto, DateTime fechaCambio, int idEmpresa, bool valorCompra);
        
        TipoCambio SingleByMonedaFecha(int idMoneda, DateTime fecha);
    }
}
