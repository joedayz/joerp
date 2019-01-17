
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class UsuarioSucursalBL : Singleton<UsuarioSucursalBL>
    {
        private readonly IUsuarioSucursalRepository repository = new UsuarioSucursalRepository();

         public IList<UsuarioSucursal> GetByUsuario(int idUsuario)
         {
             try
             {
                 return repository.GetByUsuario(idUsuario);
             }
             catch (Exception ex)
             {
                 throw new Exception(ex.Message);
             }
         }
    }
}
