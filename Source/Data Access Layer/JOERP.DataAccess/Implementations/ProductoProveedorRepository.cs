
namespace JOERP.DataAccess.Implementations
{
    using System.Data.Common;
    using Business.Entity;
    using Interfaces;

    public class ProductoProveedorRepository : Repository<ProductoProveedor>, IProductoProveedorRepository
    {
        public ProductoProveedor Add(ProductoProveedor productoProveedor, DbTransaction transaction)
        {
            return transaction == null
                       ? base.Add("Insert_ProductoProveedor", productoProveedor)
                       : base.Add("Insert_ProductoProveedor", productoProveedor, transaction);
        }
    }
}
