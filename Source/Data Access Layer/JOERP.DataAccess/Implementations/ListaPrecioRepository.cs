
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class ListaPrecioRepository : Repository<ListaPrecio>, IListaPrecioRepository
    {
        public IList<ListaPrecio> GetByEmpresa(int idEmpresa)
        {
            return Get("usp_Select_ListaPrecio_ByIdEmpresa", idEmpresa);
        }

        public ListaPrecio GetListaBase(int idEmpresa)
        {
            return Single("usp_Select_ListaPrecioBase_ByIdEmpresa", idEmpresa);
        }
    }
}
