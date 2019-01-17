
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class UnidadMedidaBL: Singleton<UnidadMedidaBL>, IPaged<UnidadMedida>
    {
        private readonly IUnidadMedidaRepository repository = new UnidadMedidaRepository();

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

        public IList<UnidadMedida> GetPaged(params object[] parameters)
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
        public IList<UnidadMedida> Get()
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

        public UnidadMedida Single(int idUnidadMedida)
        {
            try
            {
                return repository.Single(idUnidadMedida);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public UnidadMedida Add(UnidadMedida unidadMedida)
        {
            try
            {
                return repository.Add(unidadMedida);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public UnidadMedida Update(UnidadMedida unidadMedida)
        {
            try
            {
                return repository.Update(unidadMedida);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idUnidadMedida)
        {
            try
            {
                repository.Delete(idUnidadMedida);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
