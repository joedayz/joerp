
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class UsuarioBL : Singleton<UsuarioBL> , IPaged<Usuario>
    {
        private readonly IUsuarioRepository repository = new UsuarioRepository();

        public Usuario Single(string username, string password, int idEmpresa)
        {
            try
            {
                password = Encriptador.Encriptar(password);
                var usuario = repository.GetByCredenciales(username, password, idEmpresa);

                usuario.Empleado = EmpleadoBL.Instancia.Single(usuario.IdEmpleado);
                usuario.Rol = RolBL.Instancia.Single(usuario.IdRol); 
                usuario.Empleado.Persona = PersonaBL.Instancia.Single(usuario.IdEmpleado); 
                usuario.Empleado.Cargo = CargoBL.Instancia.Single(usuario.IdCargo); 

                return usuario;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool VerificarAccesoSucusal(string username, int idSucursal)
        {
            try
            {
                return repository.VerificarAccesoSucusal(username, idSucursal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Usuario Single(int id)
        {
            try
            {
                var entidad = repository.Single(id);
                entidad.Empleado = EmpleadoBL.Instancia.Single(entidad.IdEmpleado);
                return entidad;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Usuario> Get()
        {
            try
            {
                var entidades = repository.Get();
                foreach (var item in entidades)
                {
                    item.Empleado = EmpleadoBL.Instancia.Single(item.IdEmpleado);
                    item.Empleado.Usuario = UsuarioBL.Instancia.Single(item.IdEmpleado);
                }
                return entidades;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Usuario Add(Usuario usuario)
        {
            try
            {
                
                return repository.Insertar(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Usuario Update(Usuario usuario)
        {
            try
            {
                return repository.Actualizar(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idUsuario)
        {
            try
            {
                repository.Delete(idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int Count(params object[] parameters)
        {
            try
            {
                return repository.Count(parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Usuario> GetPaged(params object[] parameters)
        {
            try
            {
                return repository.GetPaged(parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Usuario> GetUsersInRol(string roleName, string usernameToMatch)
        {
            try
            {
                return repository.GetUsersInRol(roleName, usernameToMatch);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Usuario> GetUsersInRol(string roleName)
        {
            try
            {
                return repository.GetUsersInRol(roleName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsUserInRol(string username, string roleName)
        {
            try
            {
                return repository.IsUserInRol(username, roleName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
