
namespace JOERP.DataAccess.Interfaces
{
    using Business.Entity;
    using System.Collections.Generic;

    public interface IProductoComponenteRepository : IRepository<ProductoComponente>
    {
        IList<ProductoComponente> GetByIdProducto(int idProducto);
    }
}
