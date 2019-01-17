
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IAlmacenRepository : IRepository<Almacen>
    {
        List<Almacen> GetAllAlmacenByIdSucursal(int idSucursal);
    }
}
