
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class AlmacenBL : Singleton<AlmacenBL>, IPaged<Almacen>
    {
        private readonly IAlmacenRepository repository = new AlmacenRepository();

        public IList<Almacen> GetAll()
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

        public Almacen Single(int? id)
        {
            try
            {
                var almacen = repository.Single(id);
                almacen.Sucursal = SucursalBL.Instancia.Single(almacen.IdSucursal);
                return almacen;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Almacen Add(Almacen entity)
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

        public Almacen Update(Almacen entity)
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

        public List<Almacen> GetByIdSucursal(int idSucursal)
        {
            try
            {
                return repository.GetAllAlmacenByIdSucursal(idSucursal);
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

        public IList<Almacen> GetPaged(params object[] parameters)
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