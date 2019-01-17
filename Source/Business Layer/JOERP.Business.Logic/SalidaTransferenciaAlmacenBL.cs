
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity.DTO;
    using Helpers;

    public class SalidaTransferenciaAlmacenBL : Singleton<SalidaTransferenciaAlmacenBL>, IPaged<Transferencia>
    {
        private readonly ISalidaTransferenciaAlmacenRepository repository = new SalidaTransferenciaAlmacenRepository();

        public bool Insertar(Transferencia transaccion)
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

        public bool Eliminar(int idTransaccion, int idUsuarioModificacion, int estado, DateTime fechaRegistro)
        {
            try
            {
                return repository.Eliminar(idTransaccion, idUsuarioModificacion, estado, fechaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Transferencia Single(int id)
        {
            try
            {
                var transaccion = repository.Single(id);
                // transaccion.TransaccionImpuesto = TransaccionImpuestoBL.Instancia.GetByTransaccion(transaccion.IdTransaccion);
                // transaccion.Almacen = AlmacenBL.Instancia.Single(transaccion.IdAlmacen);
                // transaccion.TransaccionDocumento =TransaccionDocumentoBL.Instancia.GetByTransaccion(transaccion.IdTransaccion);
                return transaccion;
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

        public IList<Transferencia> GetPaged(params object[] parameters)
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
