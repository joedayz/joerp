
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface ISucursalRepository : IRepository<Sucursal>
    {
        IList<Sucursal> GetByEmpresa(int idEmpresa);

        IList<Sucursal> GetAutorizadas(int idEmpresa, int idEmpleado);
    }
}
