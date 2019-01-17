
namespace JOERP.DataAccess.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Business.Entity;
    using Helpers;

    public interface IIngresoTransferenciaAlmacenRepository : IRepository<Transaccion>
    {
        bool Insertar(Transaccion transaccion);

        bool Actualizar(Transaccion transaccion);

        bool Eliminar(int idTransaccion, int idUsuario, int estado, DateTime fechaRegistro);

        List<SerieDocumento> GetSerieDocumentoIngresoTransferencia(int tipoDocumento, int idalmacenDestino, int idSucursal, int idEmpresa);
       
        List<Comun> GetNumerosBySerie(int idSerie, int idalmacenDestino, int idSucursal, int idEmpresa , int idOperacion);
    }
}
