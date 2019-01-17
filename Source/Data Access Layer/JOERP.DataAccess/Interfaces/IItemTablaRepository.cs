
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IItemTablaRepository : IRepository<ItemTabla>
    {
        IList<ItemTabla> GetByTabla(int idTabla);
        int GetMaximoId();
    }
}
