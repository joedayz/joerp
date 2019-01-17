
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public  class OperacionBL : Singleton<OperacionBL>, IPaged<Operacion>
    {
        private readonly IOperacionRepository repository = new OperacionRepository();

        public IList<Operacion> Get()
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

        public Operacion Single(int idOperacion)
        {
            try
            {
                return repository.Single(idOperacion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Operacion Add(Operacion operacion)
        {
            try
            {
                return repository.Add(operacion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Operacion Update(Operacion operacion)
        {
            try
            {
                return repository.Update(operacion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idOperacion)
        {
            try
            {
                repository.Delete(idOperacion);
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

        public IList<Operacion> GetPaged(params object[] parameters)
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

        public int MaxId()
        {
            try
            {
                return repository.MaxId();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool ExisteCodigo(string codigo, int id)
        {
            try
            {
                return repository.ExisteCodigo(codigo, id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 
    }
}
