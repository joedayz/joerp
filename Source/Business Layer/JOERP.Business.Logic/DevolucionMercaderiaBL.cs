
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity.DTO;
    using Helpers;

    public class DevolucionMercaderiaBL : Singleton<DevolucionMercaderiaBL>, IPaged<DevolucionMercaderia>
    {
        private readonly IDevolucionMercaderiaRepository repository = new DevolucionMercaderiaRepository();

        public bool Insertar(DevolucionMercaderia transaccion)
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

        public bool Actualizar(DevolucionMercaderia transaccion)
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

        public DevolucionMercaderia Single(int id)
        {
            try
            {
                var transaccion = repository.Single(id);

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

        public IList<DevolucionMercaderia> GetPaged(params object[] parameters)
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
