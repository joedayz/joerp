
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IListaPrecioRepository : IRepository<ListaPrecio>
    {
        IList<ListaPrecio> GetByEmpresa(int idEmpresa);

        ListaPrecio GetListaBase(int idEmpresa);
    }
}
