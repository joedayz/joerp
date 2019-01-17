
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class ItemTablaRepository : Repository<ItemTabla>, IItemTablaRepository
    {
        public IList<ItemTabla> GetByTabla(int idTabla)
        {
            return Get("usp_Select_ItemTabla_ByIdTabla", idTabla);
        }

        public int GetMaximoId()
        {
            return (int)GetScalar("usp_Select_ItemTabla_GetMaximoId");
        }

        public void Delete(int idItemTabla, int idTabla)
        {
            Delete("usp_Delete_ItemTabla",idItemTabla,idTabla);
        }
    }
}
