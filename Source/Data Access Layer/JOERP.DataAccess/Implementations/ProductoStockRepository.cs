
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Business.Entity;
    using Interfaces;
    using Helpers;

    public class ProductoStockRepository : Repository<ProductoStock>, IProductoStockRepository
    {
        public ProductoStock GetByPresentacionAlmacenLote(int idPresentacion, int idAlmacen, string loteSerie)
        {
            return Single("usp_Single_ProductoStock_ByPresentacionAlmacenLote", idPresentacion, idAlmacen, loteSerie);
        }

        public decimal GetStockByPresentacion(int idSucursal, int idPresentacion, DbTransaction transaction = null)
        {
            var stock = 0m;

            var listado = Get("usp_Select_ProductoStock_BySucursalAndPresentacion", transaction, idSucursal, idPresentacion);

            if (listado.Count() > 0)
            {
                stock = listado.Sum(p => p.StockFisico);
            }

            return stock.Redondear(); 
        }

        public IList<ProductoStock> GetByAlmacenPresentacion(int idAlmacen, int idPresentacion, DbTransaction transaction = null)
        {
            return Get("usp_Select_ProductoStock_ByAlmacenAndPresentacion", transaction, idAlmacen, idPresentacion);
        } 

        public void UpdateProductoStock(IList<MovimientoProducto> movimientos, DbTransaction transaction = null)
        {
            foreach (var movimientoProducto in movimientos)
            {
                foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                {
                    AddGeneric("usp_Update_ProductoStock", movimientoProductoStock, transaction);
                }
            }
        }

        public void UpdateProductoStockFisico(IList<MovimientoProducto> movimientos, DbTransaction transaction = null)
        {
            foreach (var movimientoProducto in movimientos)
            {
                foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                {
                    AddGeneric("usp_Update_ProductoStockFisico", movimientoProductoStock, transaction);
                }
            }
        }
    }
}
