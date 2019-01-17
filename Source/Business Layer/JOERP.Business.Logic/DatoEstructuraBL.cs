
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class DatoEstructuraBL : Singleton <DatoEstructuraBL>, IPaged<DatoEstructura>
    {
        private readonly IDatoEstructuraRepository repository = new DatoEstructuraRepository();

        public IList<DatoEstructura> GetAll()
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

        public DatoEstructura Single(int? id)
        {
            try
            {
                return repository.Single(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<DatoEstructura> GetByIdEstructura(int idEstructura)
        {
            try
            {
                return repository.GetByIdEstructura(idEstructura);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<DatoEstructura> GetByIdParent(int idParent)
        {
            try
            {
                return repository.GetByIdParent(idParent);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DatoEstructura Single(int id)
        {
            try
            {
                return repository.Single(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DatoEstructura Add(DatoEstructura entity)
        {
            try
            {
                return repository.Add(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DatoEstructura Update(DatoEstructura entity)
        {
            try
            {
                return repository.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int id)
        {
            try
            {
               
                repository.Delete(id);
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

        public IList<DatoEstructura> GetPaged(params object[] parameters)
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
