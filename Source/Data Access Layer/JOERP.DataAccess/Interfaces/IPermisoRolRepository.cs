
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IPermisoRolRepository : IRepository<PermisoRol>
    {
        void Guardar(int idFormulario, List<PermisoRol> lista);

        IList<PermisoRol> GetFiltered(int idUsuario, int idFormulario);
    }
}
