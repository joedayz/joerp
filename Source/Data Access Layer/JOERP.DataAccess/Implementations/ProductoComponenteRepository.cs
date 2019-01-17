
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class ProductoComponenteRepository : Repository<ProductoComponente>, IProductoComponenteRepository
    {
        public IList<ProductoComponente> GetByIdProducto(int idProducto)
        {
            return  Get("usp_Select_ProductoComponente_GetByIdProducto", idProducto);
        }
    }
}
