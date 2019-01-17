
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class UsuarioSucursalRepository : Repository<UsuarioSucursal>, IUsuarioSucursalRepository
    {
        public IList<UsuarioSucursal> GetByUsuario(int idUsuario)
        {
            return Get("usp_Select_UsuarioSucursal_GetByUsuario", idUsuario);
        }
    }
}
