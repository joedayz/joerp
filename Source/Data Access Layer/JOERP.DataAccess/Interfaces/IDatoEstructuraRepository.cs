
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IDatoEstructuraRepository : IRepository<DatoEstructura>
    {
        IList<DatoEstructura> GetByIdEstructura(int idEstructura);
        IList<DatoEstructura> GetByIdParent(int idParent);
    }
}
