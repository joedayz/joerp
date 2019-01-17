
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class SucursalBL : Singleton<SucursalBL>, IPaged<Sucursal>
    {
        private readonly ISucursalRepository repository = new SucursalRepository();

        public IList<Sucursal> Get()
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

        public int Count(object[] parametros)
        {
            try
            {
                return repository.Count(parametros);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Sucursal> GetPaged(object[] parametros)
        {
            try
            {
                return repository.GetPaged(parametros);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Sucursal Single(int id)
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

        public IList<Sucursal> GetByEmpresa(int idEmpresa)
        {
            try
            {
                return repository.GetByEmpresa(idEmpresa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Sucursal> GetAutorizadas(int idEmpresa, int idEmpleado)
        {
            try
            {
                var sucursales = repository.GetAutorizadas(idEmpresa, idEmpleado);

                foreach (var item in sucursales)
                {
                    item.Almacen = AlmacenBL.Instancia.GetByIdSucursal(item.IdSucursal);
                }
                return sucursales;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Sucursal Add(Sucursal entity)
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

        public Sucursal Update(Sucursal entity)
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
    }
}
