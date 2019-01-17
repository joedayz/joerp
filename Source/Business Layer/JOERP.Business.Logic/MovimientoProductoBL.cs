
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class MovimientoProductoBL : Singleton<MovimientoProductoBL>
    {
        private readonly  IMovimientoProductoRepository repository = new MovimientoProductoRepository();

        public IList<MovimientoProducto> GetByTransaccion(int idTransaccion)
        {
            try
            {
                return repository.GetByTransaccion(idTransaccion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
