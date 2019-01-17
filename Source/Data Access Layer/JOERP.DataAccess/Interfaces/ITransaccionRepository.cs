
namespace JOERP.DataAccess.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using Business.Entity;
    using Business.Entity.DTO;

    public interface ITransaccionRepository : IRepository<Transaccion>
    {
        List<TransaccionMovimientoDTO> SelectTransaccionByParameter(DateTime? fechaInicio, DateTime? fechaFin,
                                                                    int idPersona,
                                                                    int idOperacion, int idSucursal, int idAlmacen,
                                                                    int idProducto, int idPresentacion,
                                                                    string serie, string numero);

        int InsertTransaccion(Transaccion transaccion, DbTransaction transaction = null);

        void UpdateTransaccion(Transaccion transaccion, DbTransaction transaction = null);

        void DeleteTransaccion(int idTransaccion, int estado, int usuario, DbTransaction transaction = null);

        void UpdateReferenciaTransaccion(Transaccion transaccion, DbTransaction transaction = null);
    }
}
