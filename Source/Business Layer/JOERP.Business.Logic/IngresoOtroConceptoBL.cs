
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity.DTO;
    using Helpers;

    public class IngresoOtroConceptoBL : Singleton<IngresoOtroConceptoBL>, IPaged<IngresoOtroConceptoDTO>
    {
        private readonly IIngresoOtroConceptoRepository repository = new IngresoOtroConceptoRepository();

        public bool Insertar(IngresoOtroConceptoDTO transaccion)
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

        public bool Actualizar(IngresoOtroConceptoDTO transaccion)
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

        public IngresoOtroConceptoDTO Single(int id)
        {
            try
            {
                var transaccion =  repository.Single(id);

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

        public IList<IngresoOtroConceptoDTO> GetPaged(params object[] parameters)
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
