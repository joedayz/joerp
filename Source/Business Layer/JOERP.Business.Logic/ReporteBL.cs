
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    using JOERP.Business.Entity.Reportes;

    public class ReporteBL : Singleton<ReporteBL>
    {
        private readonly IReporteRepository repository = new ReporteRepository();

        public IList<DetalleMovimiento> ListarMovimientosProductoLote(string codigoProducto, int almacen, int sucursal)
        {
            try
            {
                return repository.ListarMovimientosProductoLote(codigoProducto, almacen, sucursal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<InventarioFisico> ListarInventarioFisico(int tipo, int linea, int sublinea,int categoria,
            string codigoProducto, int almacen, int sucursal, string fechaVencimiento, int idPresentacion)
        {
            try
            {
                return repository.ListarInventarioFisico(tipo,linea,sublinea,categoria,codigoProducto,almacen,sucursal,fechaVencimiento,idPresentacion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<InventarioFisico> ListarInventarioFisicoCorte(int linea, int sublinea, int categoria,
        string codigoProducto, int almacen,  string fechaCorte)
        {
            try
            {
                return repository.ListarInventarioFisicoCorte(linea, sublinea, categoria, codigoProducto, almacen, fechaCorte);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IList<MovimientoProducto> ListarValorizaciones(string serie, string documento, int idEmpresa, int idSucursal)
        {
            try
            {
                return repository.ListarValorizaciones(serie, documento, idEmpresa, idSucursal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<ProductoProveedor> ListarProductoProveedor(int linea,int sublinea,int categoria,int idProveedor,int idProducto)
        {
            try
            {
                return repository.ListarProductoProveedor(linea, sublinea, categoria, idProveedor, idProducto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IList<TransaccionDocumento> ListarDetalleValorizacion(int idTransaccion)
        {
            try
            {
                return repository.ListarDetalleValorizacion(idTransaccion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<TransaccionDocumento> ListarDetalleValorizacionExcel(int idEmpresa, int idSucursal)
        {
            try
            {
                return repository.ListarDetalleValorizacionExcel(idEmpresa, idSucursal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 
    }
}
