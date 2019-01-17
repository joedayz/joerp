
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IEstructuraProductoRepository : IRepository<EstructuraProducto>
    {
        IList<EstructuraProducto> GetParents(int idEmpresa);
        IList<EstructuraProducto> GetByIdParent(int idParent);
        IList<EstructuraProducto> GetByNivel(int nivel);
    }
}
