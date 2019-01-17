
namespace JOERP.DataAccess.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Business.Entity;
    using Business.Entity.DTO;
    using Helpers;

    public interface IDespachoRepository : IRepository<Transaccion>
    {
        bool Insertar(Transaccion transaccion, int estadoDocAlterno);

        bool Actualizar(Transaccion transaccion, int estadoDocAlterno);

        bool Eliminar(int idTransaccion, int idUsuarioModificacion, int estado);
        List<Comun> GetDocumentoConDespacho(int idEmpresa, int idSucursal, int idAlmacen);
        List<Comun> GetSerieDocumentoConDespacho(int idEmpresa, int idDocumento, int idSucursal, int idAlmacen);
        List<Comun> GetNumeroDocumentoConDespacho(int idEmpresa, int idDocumento, int idSerie, int idSucursal, int idAlmacen);
        List<Transaccion> GetDocumentosTransaccion(int clienteId, int documentoId, int serieID, string numero, DateTime? fechaInicio, DateTime? fechaFin, int idSucursal, int idAlmacen);
        List<DespachoDTO> Buscar(int idCliente, int idTipo, string serie, string numero, int idAlmacen, DateTime? fechaInicio, DateTime? fechaFin, int idOperacion, int start, int rows, out int count);
    }
}
