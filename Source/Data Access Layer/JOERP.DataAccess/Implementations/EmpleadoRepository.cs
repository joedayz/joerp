
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class EmpleadoRepository : Repository<Empleado>, IEmpleadoRepository
    {
        public int MaxId()
        {
            return (int)GetScalar("usp_Select_Empleado_MaxId");
        }

        public IList<Empleado> GetByIdCargo(int idCargo)
        {
            return Get("usp_Select_Empleado_ByIdCargo", idCargo);
        }
    }
}
