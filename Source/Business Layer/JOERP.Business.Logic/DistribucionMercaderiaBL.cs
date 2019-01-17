
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity.DTO;
    using Helpers;

    public class DistribucionMercaderiaBL : Singleton<DistribucionMercaderiaBL>, IPaged<DistribucionMercaderia>
    {
        private readonly IDistribucionMercaderiaRepository repository = new DistribucionMercaderiaRepository();

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

        public IList<DistribucionMercaderia> GetPaged(params object[] parameters)
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

        public bool Insertar(DistribucionMercaderia distribucion)
        {
            try
            {
                return repository.Insertar(distribucion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DistribucionMercaderia Single(int idTrasaccion)
        {
            try
            {
                return repository.Single(idTrasaccion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Eliminar(DistribucionMercaderia distribucion)
        {
            try
            {
                repository.Eliminar(distribucion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
