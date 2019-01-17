
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;
    using Business.Entity.DTO;
    using Helpers;

    public interface IVentaRepository : IRepository<Venta>
    {
        bool Insertar(Transaccion transaccion);

        bool Actualizar(Transaccion transaccion);

        bool Eliminar(Transaccion transaccion);

        List<SerieDocumento> GetSeriesOrdenPedidoAprobados(int idEmpresa, int idSucursal, int tipoDocumento, int estado);

        List<Comun> GetNumerosOrdenPedidoAprobados(int idEmpresa, int idSucursal, int idSerie, int estado);

        IList<MovimientoProducto> ObtenerVentaExportar(int id);
    }
}
