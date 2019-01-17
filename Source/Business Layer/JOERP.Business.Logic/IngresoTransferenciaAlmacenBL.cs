
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class IngresoTransferenciaAlmacenBL : Singleton<IngresoTransferenciaAlmacenBL>, IPaged<Transaccion>
    {
        private readonly IIngresoTransferenciaAlmacenRepository repository = new IngresoTransferenciaAlmacenRepository();

        public bool Insertar(Transaccion transaccion)
        {
            try
            {
                return repository.Insertar(transaccion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Actualizar(Transaccion transaccion)
        {
            try
            {
                return repository.Actualizar(transaccion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Eliminar(int idTransaccion, int idUsuario, int estado, DateTime fechaRegistro)
        {
            try
            {
                return repository.Eliminar(idTransaccion, idUsuario, estado, fechaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Comun> GetNumerosBySerie(int idSerie, int idalmacenDestino, int idSucursal, int idEmpresa, int idOperacion)
        {
            try
            {
                return repository.GetNumerosBySerie(idSerie, idalmacenDestino, idSucursal, idEmpresa, idOperacion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
      
        public Transaccion Single(int id)
        {
            try
            {
                var entidad = repository.Single(id);
                entidad.SerieDocumento = SerieDocumentoBL.Instancia.Single(entidad.IdSerieDocumento).Serie;
                return entidad;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SerieDocumento> GetSerieDocumentoIngresoTransferencia(int tipoDocumento, int idalmacenDestino, int idSucursal, int idEmpresa)
        {
            try
            {
                return repository.GetSerieDocumentoIngresoTransferencia(tipoDocumento, idalmacenDestino, idSucursal, idEmpresa);
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

        public IList<Transaccion> GetPaged(params object[] parameters)
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
