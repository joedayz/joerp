
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity.DTO;
    using Helpers;

    public class OrdenCompraBL : Singleton <OrdenCompraBL>, IPaged<OrdenCompra>
    {
        private readonly IOrdenCompraRepository repository = new OrdenCompraRepository();

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

        public IList<OrdenCompra> GetPaged(params object[] parameters)
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

        public OrdenCompra Single(int id)
        {
            try
            {
                var ordenCompra = repository.Single(id);

                return ordenCompra;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Insertar(OrdenCompra orden)
        {
            try
            {
                return repository.Insertar(orden);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Actualizar(OrdenCompra orden)
        {
            try
            {
                return repository.Actualizar(orden);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Eliminar(int idTransaccion, int idUsuario, int estado)
        {
            try
            {
                return repository.Eliminar(idTransaccion, idUsuario, estado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Aprobar(int idTransaccion, int idUsuarioModificacion, int estado)
        {
            try
            {
                return repository.Aprobar(idTransaccion, idUsuarioModificacion, estado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<OrdenCompra> BuscarOrden(int idOperacion,int idProveedor, string desde, string hasta, string documento)
        {
            try
            {
                return repository.BuscarOrden(idOperacion,idProveedor, desde, hasta, documento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
