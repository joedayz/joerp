
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    using JOERP.Business.Entity.Reportes;

    public interface IReporteRepository : IRepository<Reporte>
    {
        IList<DetalleMovimiento> ListarMovimientosProductoLote(params object[] parameters);

        IList<InventarioFisico> ListarInventarioFisico(params object[] parameters);

        IList<MovimientoProducto> ListarValorizaciones(params object[] parameters);

        IList<ProductoProveedor> ListarProductoProveedor(params object[] parameters);

        IList<TransaccionDocumento> ListarDetalleValorizacion(params object[] parameters);

        IList<TransaccionDocumento> ListarDetalleValorizacionExcel(params object[] parameters);

        IList<InventarioFisico> ListarInventarioFisicoCorte(params object[] parameters);
    }
}
