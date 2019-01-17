
namespace JOERP.DataAccess.Implementations
{
    using System.Data.Common;
    using Business.Entity;
    using Interfaces;

    public class SaldoProductoRepository : Repository<SaldoProducto>, ISaldoProductoRepository
    {
        public SaldoProducto GetDisponible(int idEmpresa, int idSucursal, int idAlmacen, int idProducto, int idPresentacion, string loteSerie, string periodo, DbTransaction transaction = null)
        {
            return Single(transaction, idEmpresa, idSucursal, idProducto, idPresentacion, idAlmacen, loteSerie, periodo);
        }
    }
}
