
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IDatoEstructuraProductoRepository : IRepository<DatoEstructuraProducto>
    {
        IList<DatoEstructuraProducto> GetDatosByIdProducto(int idProducto);
    }
}
