
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IPermisoUsuarioRepository : IRepository<PermisoUsuario>
    {
        void Guardar(int idFormulario, List<PermisoUsuario> lista);

        IList<PermisoUsuario> GetFiltered(int idUsuario, int idFormulario);

        IList<PermisoUsuario> GetFiltered(int idUsuario, int idFormulario, int idRol);
    }
}
