
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Entity.DTO;
    using Helpers;

    public class DespachoBL : Singleton<DespachoBL>, IPaged<Transaccion>
    {
        private readonly IDespachoRepository repository = new DespachoRepository();

        public bool Insertar(Transaccion transaccion, int estadoDocAlterno)
        {
            try
            {
                return repository.Insertar(transaccion, estadoDocAlterno);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Actualizar(Transaccion transaccion, int estadoDocAlterno)
        {
            try
            {
                return repository.Actualizar(transaccion, estadoDocAlterno);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Eliminar(int idTransaccion, int idUsuarioModificacion, int estado)
        {
            try
            {
                return repository.Eliminar(idTransaccion, idUsuarioModificacion, estado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Comun> GetDocumentoConDespacho(int idEmpresa, int idSucursal, int idAlmacen)
        {
            try
            {
                return repository.GetDocumentoConDespacho(idEmpresa, idSucursal, idAlmacen);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Transaccion> GetDocumentosTransaccion(int clienteId, int documentoId, int serieID, string numero, DateTime? fechaInicio, DateTime? fechaFin, int idSucursal, int idAlmacen)
        {
            try
            {
                return repository.GetDocumentosTransaccion(clienteId, documentoId, serieID, numero, fechaInicio, fechaFin, idSucursal, idAlmacen);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Comun> GetSerieDocumentoConDespacho(int idEmpresa, int idDocumento, int idSucursal, int idAlmacen)
        {
            try
            {
                return repository.GetSerieDocumentoConDespacho(idEmpresa, idDocumento, idSucursal, idAlmacen);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Comun> GetNumeroDocumentoConDespacho(int idEmpresa, int idDocumento, int idSerie, int idSucursal, int idAlmacen)
        {
            try
            {
                return repository.GetNumeroDocumentoConDespacho(idEmpresa, idDocumento, idSerie, idSucursal, idAlmacen);
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
                var transaccion = repository.Single(id);
                transaccion.SerieDocumento = SerieDocumentoBL.Instancia.Single(id).Serie;
                return transaccion;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<DespachoDTO> Buscar(int idCliente, int idTipo, string serie, string numero, int idAlmacen, DateTime? fechaInicio, DateTime? fechaFin, int idOperacion, int start, int rows, out int count)
        {
            try
            {
                return repository.Buscar(idCliente, idTipo, serie, numero, idAlmacen, fechaInicio, fechaFin, idOperacion, start, rows, out count);
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
