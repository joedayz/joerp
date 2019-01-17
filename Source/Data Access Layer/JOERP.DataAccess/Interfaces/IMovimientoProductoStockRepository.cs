
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;
    using System.Data.Common;

    public interface IMovimientoProductoStockRepository : IRepository<MovimientoProductoStock>
    {
        List<MovimientoProductoStock> GetByMovimientoProducto(int idMovimientoProducto, DbTransaction transaction = null);
        List<MovimientoProductoStock> SelectMovimientoProductoStockByParameter(int idMovimientoProducto);
    }
}
