
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class OperacionImpuestoBL : Singleton<OperacionImpuestoBL>
    {
        private readonly IOperacionImpuestoRepository repository = new OperacionImpuestoRepository();

        public IList<OperacionImpuesto> GetFiltered(int idImpuesto, int idOperacion)
        {
            try
            {
                return repository.GetFiltered(idImpuesto, idOperacion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<OperacionImpuesto> GetByOperacion(int idOperacion)
        {
            try
            {
                var lista = repository.GetByOperacion(idOperacion);
                foreach (var item in lista)
                {
                    item.Impuesto = ImpuestoBL.Instancia.Single(item.IdImpuesto);
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<OperacionImpuesto> Get()
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

        public OperacionImpuesto Single(int? id)
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

        public OperacionImpuesto Add(OperacionImpuesto entity)
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

        public OperacionImpuesto Update(OperacionImpuesto entity)
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

        public void Guardar(int idOperacion, List<OperacionImpuesto> lista)
        {
            try
            {
                repository.Guardar(idOperacion, lista);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
