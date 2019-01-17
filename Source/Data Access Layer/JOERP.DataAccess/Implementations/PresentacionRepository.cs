
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class PresentacionRepository : Repository<Presentacion>, IPresentacionRepository
    {
        public IList<Presentacion> GetByIdProducto(int idProducto)
        {
            return Get("usp_Select_Presentacion_GetByIdProducto", idProducto);
        }
    }
}
