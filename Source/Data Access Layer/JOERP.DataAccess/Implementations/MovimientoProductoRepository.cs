
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Business.Entity;
    using Interfaces;

    public class MovimientoProductoRepository : Repository<MovimientoProducto>, IMovimientoProductoRepository
    {
        public IList<MovimientoProducto> GetByTransaccion(int idTransaccion)
        {
            return Get("usp_Select_MovimientoProducto_ByTransaccion", idTransaccion);
        }

        public IList<MovimientoProducto> GetByTransaccion(int idTransaccion, DbTransaction transaction)
        {
            var movimientoStockRepository = new MovimientoProductoStockRepository();
            var listaMovimientoProducto = Get("usp_Select_MovimientoProducto_ByTransaccion", idTransaccion, transaction);

            foreach (var movimiento in listaMovimientoProducto)
            {
                var movimientosStock = movimientoStockRepository.GetByMovimientoProducto(movimiento.IdMovimientoProducto, transaction);
                movimiento.MovimientoProductoStock = new List<MovimientoProductoStock>();

                foreach (var item in movimientosStock)
                {
                    movimiento.MovimientoProductoStock.Add(item);
                }
            }

            return listaMovimientoProducto;
        }

        public void InsertOnlyMovimientos(IList<MovimientoProducto> movimientos, DbTransaction transaction)
        {
            foreach (var movimientoProducto in movimientos)
            {
                Add("usp_Insert_MovimientoProducto", movimientoProducto, transaction);
            }
        }

        public void InsertMovimientos(IList<MovimientoProducto> movimientos, DbTransaction transaction)
        {
            foreach (var movimientoProducto in movimientos)
            {
                var movimientoProductoNuevo = Add("usp_Insert_MovimientoProducto", movimientoProducto, transaction);

                foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                {
                    movimientoProductoStock.SignoStock = movimientoProducto.SignoStock;
                    movimientoProductoStock.IdPresentacion = movimientoProductoNuevo.IdPresentacion;
                    movimientoProductoStock.UsuarioCreacion = movimientoProductoNuevo.UsuarioCreacion;
                    movimientoProductoStock.TipoClasificacion = movimientoProductoNuevo.TipoClasificacion;
                    movimientoProductoStock.IdMovimientoProducto = movimientoProductoNuevo.IdMovimientoProducto;

                    AddGeneric("usp_Insert_MovimientoProductoStock", movimientoProductoStock, transaction);
                }
            }
        }

        public void InsertMovimientosLogico(IList<MovimientoProducto> movimientos, DbTransaction transaction)
        {
            foreach (var movimientoProducto in movimientos)
            {
                var movimientoProductoNuevo = Add("usp_Insert_MovimientoProducto", movimientoProducto, transaction);

                foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                {
                    movimientoProductoStock.SignoStock = movimientoProducto.SignoStock;
                    movimientoProductoStock.IdPresentacion = movimientoProductoNuevo.IdPresentacion;
                    movimientoProductoStock.UsuarioCreacion = movimientoProductoNuevo.UsuarioCreacion;
                    movimientoProductoStock.TipoClasificacion = movimientoProductoNuevo.TipoClasificacion;
                    movimientoProductoStock.IdMovimientoProducto = movimientoProductoNuevo.IdMovimientoProducto;

                    AddGeneric("usp_Insert_MovimientoProductoStockLogico", movimientoProductoStock, transaction);
                }
            }
        }
        
        public void UpdateEstadosMovimientos(IList<MovimientoProducto> movimientos, int idEstado, DbTransaction transaction)
        {
            var comandoUpdateEstadoMovimientoProducto = Database.GetStoredProcCommand("UpdateEstadoMovimientoProducto");
            Database.AddInParameter(comandoUpdateEstadoMovimientoProducto, "pIdMovimientoProducto", DbType.Int32);
            Database.AddInParameter(comandoUpdateEstadoMovimientoProducto, "pEstado", DbType.Int32);

            foreach (var movimiento in movimientos)
            {
                Database.SetParameterValue(comandoUpdateEstadoMovimientoProducto, "pIdMovimientoProducto", movimiento.IdMovimientoProducto);
                Database.SetParameterValue(comandoUpdateEstadoMovimientoProducto, "pEstado", idEstado);
                Database.ExecuteNonQuery(comandoUpdateEstadoMovimientoProducto, transaction);
            }
        }

        public void DeleteByTransaccion(int idTransaccion, DbTransaction transaction)
        {
            var comandoDeleteMovimiento = Database.GetStoredProcCommand("DeleteMovimientoProducto");
            Database.AddInParameter(comandoDeleteMovimiento, "pIdTransaccion", DbType.Int32, idTransaccion);
            Database.ExecuteNonQuery(comandoDeleteMovimiento, transaction);
        }

        public void InsertMovimientosFisico(IList<MovimientoProducto> movimientos, DbTransaction transaction)
        {
            foreach (var movimientoProducto in movimientos)
            {
                var movimientoProductoNuevo = Add("usp_Insert_MovimientoProducto", movimientoProducto, transaction);

                foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                {
                    movimientoProductoStock.SignoStock = movimientoProducto.SignoStock;
                    movimientoProductoStock.IdPresentacion = movimientoProductoNuevo.IdPresentacion;
                    movimientoProductoStock.UsuarioCreacion = movimientoProductoNuevo.UsuarioCreacion;
                    movimientoProductoStock.TipoClasificacion = movimientoProductoNuevo.TipoClasificacion;
                    movimientoProductoStock.IdMovimientoProducto = movimientoProductoNuevo.IdMovimientoProducto;

                    AddGeneric("usp_Insert_MovimientoProductoStockFisico", movimientoProductoStock, transaction);
                }
            }
        }

        public void UpdateCostoMovimientos(IList<MovimientoProducto> movimientos, DbTransaction transaction)
        {
            foreach (var movimiento in movimientos)
            {
                Update("usp_UpdateCosto_MovimientoProducto", movimiento, transaction);
            }
        }
    }
}
