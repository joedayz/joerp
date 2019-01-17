
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using System.Data.Common;
    using Business.Entity;
    using Interfaces;

    public class DatoEstructuraProductoRepository : Repository<DatoEstructuraProducto>, IDatoEstructuraProductoRepository
    {
        public void Add(DatoEstructuraProducto datoEstructuraProducto, DbTransaction transaction)
        {
            Add("usp_Insert_DatoEstructuraProducto", datoEstructuraProducto, transaction);
        }

        public IList<DatoEstructuraProducto> GetDatosByIdProducto(int idProducto)
        {
            return Get("usp_Select_DatoEstructuraProducto_GetByIdProducto", idProducto);
        }
    }
}
