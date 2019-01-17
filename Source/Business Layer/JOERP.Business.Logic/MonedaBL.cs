
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class MonedaBL : Singleton <MonedaBL>, IPaged<Moneda>
    {
        private readonly IMonedaRepository repository = new MonedaRepository(); 

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

        public IList<Moneda> GetPaged(params object[] parameters)
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

        public IList<Moneda> GetAll()
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

        public Moneda Single(int idMoneda)
        {
            try
            {
                return repository.Single(idMoneda);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Moneda Add(Moneda moneda)
        {
            try
            {
                return repository.Add(moneda);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Moneda Update(Moneda moneda)
        {
            try
            {
                return repository.Update(moneda);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idMoneda)
        {
            try
            {
                repository.Delete(idMoneda);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Moneda MonedaLocal(int idEmpresa)
        {
            try
            {
                return repository.GetModenaLocal(idEmpresa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
