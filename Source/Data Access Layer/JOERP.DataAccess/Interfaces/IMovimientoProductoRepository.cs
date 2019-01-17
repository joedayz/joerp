
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using System.Data.Common;
    using Business.Entity;

    public interface IMovimientoProductoRepository: IRepository<MovimientoProducto>
    {
        IList<MovimientoProducto> GetByTransaccion(int idTransaccion, DbTransaction transaction);

        IList<MovimientoProducto> GetByTransaccion(int idTransaccion);

        void InsertMovimientos(IList<MovimientoProducto> movimientos, DbTransaction transaction);

        void UpdateEstadosMovimientos(IList<MovimientoProducto> movimientos, int idEstado, DbTransaction transaction);

        void DeleteByTransaccion(int idTransaccion, DbTransaction transaction);

        void InsertMovimientosFisico(IList<MovimientoProducto> movimientos, DbTransaction transaction);

        void InsertMovimientosLogico(IList<MovimientoProducto> movimientos, DbTransaction transaction);

        void InsertOnlyMovimientos(IList<MovimientoProducto> movimientos, DbTransaction transaction);

        void UpdateCostoMovimientos(IList<MovimientoProducto> movimientos, DbTransaction transaction);
    }
}
