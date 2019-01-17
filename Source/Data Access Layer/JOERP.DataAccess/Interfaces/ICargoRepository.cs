
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface ICargoRepository : IRepository<Cargo>
    {
        IList<Cargo> GetByEmpresa(int id);
    }
}
