
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IEmpleadoRepository : IRepository<Empleado>
    {
        int MaxId();

        IList<Empleado> GetByIdCargo(int idCargo);
    }
}
