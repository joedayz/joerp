
namespace JOERP.DataAccess.Interfaces
{
    using Business.Entity;
    using System.Data.Common;

    public interface IProductoProveedorRepository : IRepository<ProductoProveedor>
    {
        ProductoProveedor Add(ProductoProveedor productoProveedor, DbTransaction transaction);
    }
}
