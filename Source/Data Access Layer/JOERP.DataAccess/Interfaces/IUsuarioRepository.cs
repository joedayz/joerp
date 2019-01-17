
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Usuario GetByCredenciales(string username, string password, int idEmpresa);

        Usuario Insertar(Usuario entity);

        Usuario Actualizar(Usuario entity);

        bool VerificarAccesoSucusal(string username, int idSucursal);

        IList<Usuario> GetUsersInRol(string roleName, string usernameToMatch);

        IList<Usuario> GetUsersInRol(string roleName);

        bool IsUserInRol(string username, string roleName);
    }
}