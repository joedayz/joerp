
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class RolBL : Singleton<RolBL> , IPaged<Rol>
    {
        private readonly IRolRepository repository = new RolRepository();

        public IList<Rol> Get()
        {
            try
            {
                return repository.Get();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Rol Single(int idRol)
        {
            try
            {
                return repository.Single(idRol);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Rol Add(Rol rol)
        {
            try
            {
                return repository.Add(rol);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Rol Update(Rol rol)
        {
            try
            {
                return repository.Update(rol);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idRol)
        {
            try
            {
                repository.Delete(idRol);
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

        public IList<Rol> GetPaged(params object[] parameters)
        {
            try
            {
                var lista = repository.GetPaged(parameters);
                foreach (var item in lista)
                {
                    item.Empresa = EmpresaBL.Instancia.Single(item.IdEmpresa);
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
