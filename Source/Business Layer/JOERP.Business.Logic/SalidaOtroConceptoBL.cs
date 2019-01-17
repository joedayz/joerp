
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity.DTO;
    using Helpers;

    public class SalidaOtroConceptoBL : Singleton<SalidaOtroConceptoBL>, IPaged<SalidaOtroConceptoDTO>
    {
        private readonly ISalidaOtroConceptoRepository repository = new SalidaOtroConceptoRepository();

        public bool Insertar(SalidaOtroConceptoDTO transaccion)
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

        public bool Actualizar(SalidaOtroConceptoDTO transaccion)
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

        public SalidaOtroConceptoDTO Single(int id)
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

        public IList<SalidaOtroConceptoDTO> GetPaged(params object[] parameters)
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
