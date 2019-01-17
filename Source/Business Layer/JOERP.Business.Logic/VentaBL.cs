
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Entity.DTO;
    using Helpers;

    public class VentaBL : Singleton <VentaBL>, IPaged<Venta>
    {
        private readonly IVentaRepository repository = new VentaRepository();

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

        public IList<Venta> GetPaged(params object[] parameters)
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

        public bool Insertar(Venta venta)
        {
            try
            {
                return repository.Insertar(venta);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Actualizar(Venta venta)
        {
            try
            {
                return repository.Actualizar(venta);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Eliminar(Venta venta)
        {
            try
            {
                return repository.Eliminar(venta);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SerieDocumento> GetSeriesOrdenPedidoAprobados(int idEmpresa, int idSucursal, int tipoDocumento, int estado)
        {
            try
            {
                return repository.GetSeriesOrdenPedidoAprobados(idEmpresa, idSucursal, tipoDocumento, estado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Comun> GetNumerosOrdenCompraAprobados(int idEmpresa, int idSucursal, int idSerie, int estado)
        {
            try
            {
                return repository.GetNumerosOrdenPedidoAprobados(idEmpresa, idSucursal, idSerie, estado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Venta Single(int id)
        {
            try
            {
                var venta = repository.Single(id);
                return venta;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

       public IList<MovimientoProducto> ObtenerVentaExportar(int id)
       {
           try
           {
               var venta = repository.ObtenerVentaExportar(id);
               return venta;
           }
           catch (Exception ex)
           {
               throw new Exception(ex.Message);
           }
       }
    }
}
