
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class PermisoUsuarioBL: Singleton <PermisoUsuarioBL>
    {
        private readonly IPermisoUsuarioRepository repository = new PermisoUsuarioRepository();

        public void Guardar(int idFormulario, List<PermisoUsuario> lista)
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

        public IList<PermisoUsuario> GetAll()
        {
            try
            {
                var entidades = repository.Get();
                foreach (var item in entidades)
                {
                    item.Usuario = UsuarioBL.Instancia.Single(item.IdEmpleado);
                }
                return entidades;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<PermisoUsuario> GetFiltered(int idUsuario, int idFormulario)
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

        public IList<PermisoUsuario> GetFiltered(int idUsuario, int idFormulario, int idRol)
        {
            try
            {
                return repository.GetFiltered(idUsuario, idFormulario, idRol);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public PermisoUsuario Single(int? id)
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

        public PermisoUsuario Add(PermisoUsuario entity)
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
    }
}
