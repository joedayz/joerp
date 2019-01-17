
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class MovimientoProductoStockBL : Singleton<MovimientoProductoStockBL>
    {
        private readonly IMovimientoProductoStockRepository repository = new MovimientoProductoStockRepository();

        public IList<MovimientoProductoStock> GetByMovimientoProducto(int idMovimientoProducto)
        {
            try
            {
                var detalleLotes = repository.GetByMovimientoProducto(idMovimientoProducto);

                foreach (var movimientoProductoStock in detalleLotes)
                {
                    if (movimientoProductoStock.FechaVencimiento.HasValue)
                    {
                        movimientoProductoStock.FechaVencimientoFormato = movimientoProductoStock.FechaVencimiento.Value.ToString("dd/MM/yyyy");   
                    }
                }

                return detalleLotes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 
    } 
}
