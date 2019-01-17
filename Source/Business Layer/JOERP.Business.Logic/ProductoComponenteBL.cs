
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class ProductoComponenteBL : Singleton<ProductoComponenteBL>
    {
        private readonly IProductoComponenteRepository repository = new ProductoComponenteRepository();

        public IList<ProductoComponente> GetByIdProducto(int idProducto)
        {
            try
            {   var entidades = repository.GetByIdProducto(idProducto);
                foreach (var item in entidades)
                {
                    item.Presentacion = PresentacionBL.Instancia.Single(item.IdPresentacion);
                    item.Presentacion.Producto = ProductoBL.Instancia.Single(item.Presentacion.IdProducto);
                }
                return entidades;
              
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
