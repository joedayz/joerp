
namespace JOERP.DataAccess.Interfaces
{
    using System.Data.Common;
    using Business.Entity;

    public interface ISaldoProductoRepository : IRepository<SaldoProducto>
    {
        SaldoProducto GetDisponible(int idEmpresa, int idSucursal, int idAlmacen, int idProducto, int idPresentacion,
                                    string loteSerie, string periodo, DbTransaction transaction = null);
    }
}
