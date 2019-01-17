
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity.DTO;
    using Helpers;

    public class OrdenPedidoBL : Singleton<OrdenPedidoBL>, IPaged<OrdenPedido>
    {
       private readonly IOrdenPedidoRepository repository = new OrdenPedidoRepository();
       
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

       public IList<OrdenPedido> GetPaged(params object[] parameters)
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

       public OrdenPedido Single(int id)
       {
           try
           {
               var ordenPedido = repository.Single(id);

               return ordenPedido;
           }
           catch (Exception ex)
           {
               throw new Exception(ex.Message);
           }
       }

       public bool Insertar(OrdenPedido orden)
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

       public bool Actualizar(OrdenPedido orden)
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

       public void Delete(int idTransaccion)
       {
           try
           {
                repository.Delete(idTransaccion);
           }
           catch (Exception ex)
           {
               throw new Exception(ex.Message);
           }
       }

        public IList<OrdenPedido> BuscarOrden(int idOperacion,int idProveedor, string desde, string hasta, string documento)
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
