
namespace JOERP.DataAccess.Interfaces
{
    using Business.Entity;
    using System.Collections.Generic;

    public interface IUsuarioSucursalRepository : IRepository<UsuarioSucursal>
    {
        IList<UsuarioSucursal> GetByUsuario(int idUsuario);
    }
}
