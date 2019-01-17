
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class EstructuraProductoRepository : Repository<EstructuraProducto>, IEstructuraProductoRepository
    {
        public IList<EstructuraProducto> GetParents(int idEmpresa)
        {
            return Get("usp_Select_EstructuraProducto_GetParents", idEmpresa);
        }

        public IList<EstructuraProducto> GetByIdParent(int idParent)
        {
            return Get("usp_Select_EstructuraProducto_GetByIdParent", idParent);
        }

        public IList<EstructuraProducto> GetByNivel(int nivel)
        {
            return Get("usp_Select_EstructuraProducto_GetByNivel", nivel);
        }
    }
}
