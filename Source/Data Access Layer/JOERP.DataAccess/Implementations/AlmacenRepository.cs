
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using Business.Entity;
    using Interfaces;

    public class AlmacenRepository : Repository<Almacen>, IAlmacenRepository
    {
        public List<Almacen> GetAllAlmacenByIdSucursal(int idSucursal)
        {
            return Get("usp_Select_Almacen_ByIdSucursal", idSucursal).ToList();
        }
    }
}
