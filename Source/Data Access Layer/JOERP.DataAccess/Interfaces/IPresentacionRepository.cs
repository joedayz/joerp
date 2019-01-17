
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IPresentacionRepository : IRepository<Presentacion>
    {
        IList<Presentacion> GetByIdProducto(int idProducto);
    }
}
