
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class OperacionDocumentoBL : Singleton<OperacionDocumentoBL>, IPaged<OperacionDocumento>
    {
        private readonly IOperacionDocumentoRepository repository = new OperacionDocumentoRepository();

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

        public IList<OperacionDocumento> GetPaged(params object[] parameters)
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

        public IList<OperacionDocumento> GetByOperacion(int idOperacion)
        {
            try
            {
                return repository.GetByOperacion(idOperacion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public OperacionDocumento Add(OperacionDocumento documento)
        {
            try
            {
                return repository.Add(documento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public OperacionDocumento Update(OperacionDocumento documento)
        {
            try
            {
                return repository.Update(documento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 
    }
}
