
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class TipoCambioBL : Singleton<TipoCambioBL>, IPaged<TipoCambio>
    {
        private readonly ITipoCambioRepository repository = new TipoCambioRepository(); 
        
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

        public IList<TipoCambio> GetPaged(params object[] parameters)
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

        public IList<TipoCambio> GetAll()
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

        public TipoCambio Single(int idTipoCambio)
        {
            try
            {
                return repository.Single(idTipoCambio);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public TipoCambio Add(TipoCambio tipoCambio)
        {
            try
            {
                return repository.Add(tipoCambio);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public TipoCambio Update(TipoCambio tipoCambio)
        {
            try
            {
                return repository.Update(tipoCambio);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idTipoCambio)
        {
            try
            {
                repository.Delete(idTipoCambio);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public TipoCambio SingleByMonedaFecha(int idMoneda, DateTime fecha)
        {
            try
            {
                return repository.SingleByMonedaFecha(idMoneda,fecha);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
