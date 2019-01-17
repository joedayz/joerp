
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Entity.DTO;
    using Helpers;

    public class CompraBL : Singleton<CompraBL>, IPaged<Compra>
    {
        private readonly ICompraRepository repository = new CompraRepository();

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

        public IList<Compra> GetPaged(params object[] parameters)
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
        
        public bool Insertar(Compra compra)
        {
            try
            {
                return repository.Insertar(compra);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Actualizar (Compra compra)
        {
            try
            {
                return repository.Actualizar(compra);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Eliminar(Transaccion transaccion)
        {
            try
            {
                return repository.Eliminar(transaccion);
            }
            catch (Exception ex)
            {
               throw new Exception(ex.Message);
            }
        }

         public List<SerieDocumento> GetSeriesOrdenCompraAprobados(int idEmpresa, int idSucursal, int tipoDocumento, int estado)
         {
             try
             {
                 return repository.GetSeriesOrdenCompraAprobados(idEmpresa, idSucursal, tipoDocumento, estado);
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
                return repository.GetNumerosOrdenCompraAprobados(idEmpresa, idSucursal, idSerie, estado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public Compra Single(int id)
        {
            try
            {
                var compra = repository.Single(id);
                return compra;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Comun> GetNumerosDocumentosByIdSerieDocumento(int idOperacion, int idSerieDocumento)
        {
            try
            {
                return repository.GetNumerosDocumentosByIdSerieDocumento(idOperacion, idSerieDocumento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Transaccion GetDatosDocumento(int idTransaccion)
        {
            try
            {
                return repository.GetDatosDocumento(idTransaccion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Transaccion> GetAllByEmpresaSucursal(int idEmpresa, int idSucursal)
        {
            try
            {
                return repository.GetAllByEmpresaSucursal(idEmpresa,idSucursal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Compra> BuscarCompras(int idOperacion, int idProveedor, string desde, string hasta, string documento)
        {
            try
            {
                return repository.BuscarCompras(idOperacion, idProveedor, desde, hasta, documento);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
