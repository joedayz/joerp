
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class EstructuraProductoBL : Singleton <EstructuraProductoBL>
    {
        private readonly IEstructuraProductoRepository repository = new EstructuraProductoRepository();

        public IList<EstructuraProducto> GetAll()
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

        public IList<EstructuraProducto> GetParents(int idEmpresa)
        {
            try
            {
                return repository.GetParents(idEmpresa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<EstructuraProducto> GetByIdParent(int idParent)
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
        public IList<EstructuraProducto> GetByNivel(int nivel)
        {
            try
            {
                return repository.GetByNivel(nivel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public EstructuraProducto Single(int? id)
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
        
        public EstructuraProducto Add(EstructuraProducto entity)
        {
            try
            {
                var temp = repository.Add(entity);
                return temp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public EstructuraProducto Update(EstructuraProducto entity)
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
    }
}
