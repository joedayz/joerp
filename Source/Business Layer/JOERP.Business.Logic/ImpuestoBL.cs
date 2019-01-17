
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class ImpuestoBL : Singleton<ImpuestoBL>, IPaged<Impuesto>
    {
        private readonly IImpuestoRepository repository = new ImpuestoRepository();

        public IList<Impuesto> Get()
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

        public Impuesto Single(int idImpuesto)
        {
            try
            {
                return repository.Single(idImpuesto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Impuesto Add(Impuesto impuesto)
        {
            try
            {
                return repository.Add(impuesto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Impuesto Update(Impuesto impuesto)
        {
            try
            {
                return repository.Update(impuesto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idImpuesto)
        {
            try
            {
                repository.Delete(idImpuesto);
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
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Impuesto> GetPaged(params object[] parameters)
        {
            try
            {
                return repository.GetPaged(parameters);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
