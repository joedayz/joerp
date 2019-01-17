
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class SucursalRepository : Repository<Sucursal>, ISucursalRepository
    {
        public IList<Sucursal> GetByEmpresa(int idEmpresa)
        {
            return Get("usp_Select_Sucursal_ByIdEmpresa", idEmpresa);
        }

        public IList<Sucursal> GetAutorizadas(int idEmpresa, int idEmpleado)
        {
            return Get("usp_Select_Sucursal_ByIdEmpresaIdEmpleado", idEmpresa, idEmpleado);
        }
    }
}
