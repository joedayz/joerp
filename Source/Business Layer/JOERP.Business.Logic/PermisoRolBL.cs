
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class PermisoRolBL : Singleton<PermisoRolBL>
    {
        private readonly IPermisoRolRepository repository = new PermisoRolRepository();

        public void Guardar(int idFormulario, List<PermisoRol> lista)
        {
            try
            {
                repository.Guardar(idFormulario, lista);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<PermisoRol> GetAll()
        {
            try
            {
                var entidades = repository.Get();
                foreach (var item in entidades)
                {
                    item.Rol = RolBL.Instancia.Single(item.IdRol);
                }
                return entidades;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public PermisoRol Single(int? id)
        {
            try
            {
                return repository.Single(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public PermisoRol Add(PermisoRol entity)
        {
            try
            {
                return repository.Add(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<PermisoRol> GetFiltered(int idUsuario, int idFormulario)
        {
            try
            {
                return repository.GetFiltered(idUsuario, idFormulario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
