
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using System.Data.Common;
    using Business.Entity;

    public interface IProductoStockRepository : IRepository<ProductoStock>
    {
        ProductoStock GetByPresentacionAlmacenLote(int idPresentacion, int idAlmacen, string loteSerie);

        decimal GetStockByPresentacion(int idSucursal, int idPresentacion, DbTransaction transaction = null);
        
        void UpdateProductoStock(IList<MovimientoProducto> movimientos, DbTransaction transaction = null);

        void UpdateProductoStockFisico(IList<MovimientoProducto> movimientos, DbTransaction transaction = null);

        IList<ProductoStock> GetByAlmacenPresentacion(int idAlmacen, int idPresentacion, DbTransaction transaction = null);
    }
}
