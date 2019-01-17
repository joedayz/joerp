
namespace JOERP.Business.Logic
{
    using System;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

	public class ProductoStockBL : Singleton<ProductoStockBL>
	{
        private  readonly IProductoStockRepository repository = new ProductoStockRepository();

        public ProductoStock Single(int idPresentacion, int idAlmacen, string lote)
        {
            try
            {
                return repository.GetByPresentacionAlmacenLote(idPresentacion, idAlmacen, lote);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
	}
}
