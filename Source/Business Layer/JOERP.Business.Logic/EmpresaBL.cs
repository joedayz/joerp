
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class EmpresaBL : Singleton<EmpresaBL>, IPaged<Empresa>
    {
        private readonly IEmpresaRepository repository = new EmpresaRepository();

        public IList<Empresa> Get()
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

        public Empresa Single(int idEmpresa)
        {
            try
            {
                return repository.Single(idEmpresa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Empresa SingleByRazonSocial(string razonSocial)
        {
            try
            {
                return repository.SingleByRazonSocial(razonSocial);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Empresa Add(Empresa empresa)
        {
            try
            {
                return repository.Add(empresa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Empresa Update(Empresa empresa)
        {
            try
            {
                return repository.Update(empresa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idEmpresa)
        {
            try
            {
                repository.Delete(idEmpresa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int Count()
        {
            try
            {
                return repository.Count();
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

        public IList<Empresa> GetPaged(params object[] parameters)
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
    }
}
