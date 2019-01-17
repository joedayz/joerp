
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class PresentacionBL: Singleton <PresentacionBL>, IPaged<Presentacion>
    {
        private readonly IPresentacionRepository repository = new PresentacionRepository();

        public IList<Presentacion> GetAll()
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
        
        public IList<Presentacion> GetByIdProducto(int idProducto)
        {
            try
            {
                return repository.GetByIdProducto(idProducto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Presentacion Single(int id)
        {
            try
            {
                var entidad = repository.Single(id);
                entidad.Producto = ProductoBL.Instancia.Single(entidad.IdProducto);
                return entidad;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
      
        public Presentacion Add(Presentacion entity)
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

        public Presentacion Update(Presentacion entity)
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

        public IList<Presentacion> GetPaged(params object[] parameters)
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
