
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class CargoRepository : Repository<Cargo>, ICargoRepository
    {
        public IList<Cargo> GetByEmpresa(int id)
        {
            return Get("usp_Select_Cargo_ByIdEmpresa", id);
        }
    }
}
