
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class DatoEstructuraRepository : Repository<DatoEstructura>, IDatoEstructuraRepository
    {
        public IList<DatoEstructura> GetByIdEstructura(int idEstructura)
        {
            return Get("usp_Select_DatoEstructura_ByIdEstructuraProducto", idEstructura);
        }

        public IList<DatoEstructura> GetByIdParent(int idParent)
        {
            return Get("usp_Select_DatoEstructura_ByIdParent", idParent);
        }
    }
}
