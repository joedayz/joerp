
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;
    using JOERP.Business.Entity.Reportes;

    public class ReporteRepository : Repository<Reporte>, IReporteRepository
    {

        public IList<DetalleMovimiento> ListarMovimientosProductoLote(params object[] parameters)
        {
            return GetGeneric<DetalleMovimiento>("GetMovimientosProductoLote", parameters);
        }

        public IList<InventarioFisico> ListarInventarioFisico(params object[] parameters)
        {
            return GetGeneric<InventarioFisico>("GetInventarioFisico", parameters);
        }

        public IList<InventarioFisico> ListarInventarioFisicoCorte(params object[] parameters)
        {
            return GetGeneric<InventarioFisico>("GetCorteInventarioFecha", parameters);
        } 
        public IList<MovimientoProducto> ListarValorizaciones(params object[] parameters)
        {
            return GetGeneric<MovimientoProducto>("usp_GetValorizacion_ByCompra", parameters);
        }

        public IList<ProductoProveedor> ListarProductoProveedor(params object[] parameters)
        {
            return GetGeneric<ProductoProveedor>("usp_GetProductoProveedor", parameters);
        }

        public IList<TransaccionDocumento> ListarDetalleValorizacion(params object[] parameters)
        {
            return GetGeneric<TransaccionDocumento>("usp_GetDocumentosValoracion", parameters);
        }

        public IList<TransaccionDocumento> ListarDetalleValorizacionExcel(params object[] parameters)
        {
            return GetGeneric<TransaccionDocumento>("usp_GetDocumentosValoracionExcel", parameters);
        }
    }
}
	