
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity.DTO;
    using Helpers;

    public class ValorizacionBL : Singleton<ValorizacionBL>, IPaged<Valorizacion>
    {
        private readonly IValorizacionRepository repository = new ValorizacionRepository();
        
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

        public IList<Valorizacion> GetPaged(params object[] parameters)
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

        public Valorizacion Add(Valorizacion valorizacion)
        {
            try
            {
                return repository.Insertar(valorizacion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(Valorizacion valorizacion)
        {
            try
            {
                repository.Actualizar(valorizacion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Valorizacion Single(int idValorizacion)
        {
            try
            {
                return repository.Obtener(idValorizacion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Valorizar(int idValorizacion)
        {
            try
            {
                repository.Valorizar(idValorizacion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
