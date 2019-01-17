
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class SerieDocumentoBL : Singleton<SerieDocumentoBL>, IPaged<SerieDocumento>
    {
        private readonly ISerieDocumentoRepository repository = new SerieDocumentoRepository();

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

        public IList<SerieDocumento> GetPaged(params object[] parameters)
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

        public IList<SerieDocumento> Get()
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

        public IList<SerieDocumento> GetByTipoDocumento(int idSucursal, int tipoDocumento)
        {
            try
            {
                return repository.GetByTipoDocumento(idSucursal, tipoDocumento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SerieDocumento Single(int idSerieDocumento)
        {
            try
            {
                return repository.Single(idSerieDocumento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SerieDocumento Add(SerieDocumento serieDocumento)
        {
            try
            {
                return repository.Add(serieDocumento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SerieDocumento Update(SerieDocumento serieDocumento)
        {
            try
            {
                return repository.Update(serieDocumento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int idSerieDocumento)
        {
            try
            {
                repository.Delete(idSerieDocumento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
