
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Business.Entity;
    using Interfaces;

    public class MovimientoProductoStockRepository : Repository<MovimientoProductoStock>, IMovimientoProductoStockRepository
    {
        public List<MovimientoProductoStock> SelectMovimientoProductoStockByParameter(int idMovimientoProducto)
        {
            var listaMovimientoProductoStock = new List<MovimientoProductoStock>();

            var comandoMovimiento = Database.GetStoredProcCommand("SelectMovimientoProductoStockByParameter");
            Database.AddInParameter(comandoMovimiento, "pIdMovimientoProducto", DbType.Int32, idMovimientoProducto);

            using (var dr = Database.ExecuteReader(comandoMovimiento))
            {
                while (dr.Read())
                {
                    var movimiento = new MovimientoProductoStock();

                    movimiento.IdAlmacen = dr.IsDBNull(dr.GetOrdinal("IdAlmacen")) ? default(int) : dr.GetInt32(dr.GetOrdinal("IdAlmacen"));
                    movimiento.IdMovimientoProducto = dr.IsDBNull(dr.GetOrdinal("IdMovimientoProducto")) ? default(int) : dr.GetInt32(dr.GetOrdinal("IdMovimientoProducto"));
                    movimiento.Secuencia = dr.IsDBNull(dr.GetOrdinal("Secuencia")) ? default(int) : dr.GetInt32(dr.GetOrdinal("Secuencia"));
                    movimiento.LoteSerie = dr.IsDBNull(dr.GetOrdinal("LoteSerie")) ? string.Empty : dr.GetString(dr.GetOrdinal("LoteSerie"));
                    movimiento.FechaVencimiento = dr.IsDBNull(dr.GetOrdinal("FechaVencimiento")) ? default(DateTime) : dr.GetDateTime(dr.GetOrdinal("FechaVencimiento"));
                    movimiento.Cantidad = dr.IsDBNull(dr.GetOrdinal("Cantidad")) ? default(decimal) : dr.GetDecimal(dr.GetOrdinal("Cantidad"));

                    listaMovimientoProductoStock.Add(movimiento);
                }
            }
            return listaMovimientoProductoStock;
        }

        public List<MovimientoProductoStock> GetByMovimientoProducto(int idMovimientoProducto, DbTransaction transaction = null)
        {
            var listaMovimientoProductoStock = new List<MovimientoProductoStock>();

            using (var comandoMovimiento = Database.GetStoredProcCommand("SelectMovimientoProductoStock"))
            {
                Database.AddInParameter(comandoMovimiento, "pIdMovimientoProducto", DbType.Int32, idMovimientoProducto);

                using (var dr = transaction == null ? Database.ExecuteReader(comandoMovimiento) : Database.ExecuteReader(comandoMovimiento, transaction))
                {
                    while (dr.Read())
                    {
                        var movimiento = new MovimientoProductoStock();

                        movimiento.IdAlmacen = dr.GetInt32(dr.GetOrdinal("IdAlmacen"));
                        movimiento.IdMovimientoProducto = dr.GetInt32(dr.GetOrdinal("IdMovimientoProducto"));
                        movimiento.Secuencia = dr.GetInt32(dr.GetOrdinal("Secuencia"));
                        movimiento.LoteSerie = dr.GetString(dr.GetOrdinal("LoteSerie"));
                        movimiento.FechaVencimiento = dr.IsDBNull(dr.GetOrdinal("FechaVencimiento")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("FechaVencimiento"));
                        movimiento.Cantidad = dr.GetDecimal(dr.GetOrdinal("Cantidad"));
                        movimiento.NombreAlmacen = dr.IsDBNull(dr.GetOrdinal("NombreAlmacen")) ? string.Empty : dr.GetString(dr.GetOrdinal("NombreAlmacen"));

                        listaMovimientoProductoStock.Add(movimiento);
                    }
                }
            }

            return listaMovimientoProductoStock;
        }
    }
}
