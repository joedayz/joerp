
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Business.Entity;
    using Business.Entity.DTO;
    using Helpers;
    using Helpers.Enums;
    using Interfaces;

    public class VentaRepository : Repository<Venta>, IVentaRepository
    {
        #region Variables

        private readonly ITransaccionRepository transaccionRepository = new TransaccionRepository();
        private readonly IProductoStockRepository productoStockRepository = new ProductoStockRepository();
        private readonly ISerieDocumentoRepository serieDocumentoRepository = new SerieDocumentoRepository();
        private readonly IMovimientoProductoRepository movimientoProductoRepository = new MovimientoProductoRepository();
        private readonly ITransaccionImpuestoRepository transaccionImpuestoRepository = new TransaccionImpuestoRepository();
        private readonly IProductoPrecioRepository productoPrecioRepository = new ProductoPrecioRepository();

        #endregion

        #region Metodos

        public bool Insertar(Transaccion transaccion)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Transaccion

                        transaccion.IdTransaccion = transaccionRepository.InsertTransaccion(transaccion, transaction);

                        #endregion

                        #region Movimiento Producto

                        if (transaccion.IdTransaccionReferencia == null)
                        {
                            foreach (var movimientoProducto in transaccion.MovimientoProducto)
                            {
                                movimientoProducto.IdTransaccion = transaccion.IdTransaccion;

                                var productos = productoStockRepository.GetByAlmacenPresentacion(movimientoProducto.IdAlmacen.Value, movimientoProducto.IdPresentacion, transaction);
                                var stockTotal = productos.Sum(p => p.StockFisico);

                                if (stockTotal < movimientoProducto.Cantidad)
                                {
                                    throw new Exception(string.Format("No existe Stock suficiente para el producto {0}.", movimientoProducto.CodigoProducto));
                                }

                                var costo = productoPrecioRepository.GetCostoByPresentacion(transaccion.IdEmpresa, transaccion.IdSucursal, movimientoProducto.IdPresentacion);
                                movimientoProducto.Costo = costo;
                                
                                var cantidad = movimientoProducto.Cantidad;
                                var secuencia = 0;

                                foreach (var productoStock in productos)
                                {
                                    cantidad -= productoStock.StockLogico;

                                    var movimientoProductoStock = new MovimientoProductoStock
                                                                      {
                                                                          Secuencia = ++secuencia,
                                                                          LoteSerie = productoStock.Lote,
                                                                          Cantidad = productoStock.StockLogico,
                                                                          IdAlmacen = movimientoProducto.IdAlmacen.Value,
                                                                          FechaVencimiento = productoStock.FechaVencimiento
                                                                      };
                                    if (cantidad > 0)
                                    {
                                        movimientoProducto.MovimientoProductoStock.Add(movimientoProductoStock);
                                    }
                                    else if (cantidad == 0)
                                    {
                                        movimientoProducto.MovimientoProductoStock.Add(movimientoProductoStock);
                                        break;
                                    }
                                    else
                                    {
                                        cantidad = cantidad + productoStock.StockLogico;
                                        movimientoProductoStock.Cantidad = cantidad;
                                        movimientoProducto.MovimientoProductoStock.Add(movimientoProductoStock);
                                        break;
                                    }
                                }
                            }

                            movimientoProductoRepository.InsertMovimientos(transaccion.MovimientoProducto, transaction);
                        }
                        else
                        {
                            foreach (var movimientoProducto in transaccion.MovimientoProducto)
                            {
                                movimientoProducto.IdTransaccion = transaccion.IdTransaccion;

                                foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                                {
                                    var productoStock = productoStockRepository.GetByPresentacionAlmacenLote(
                                            movimientoProducto.IdPresentacion, 
                                            movimientoProductoStock.IdAlmacen,
                                            movimientoProductoStock.LoteSerie);
                                    
                                    if (productoStock.StockFisico < movimientoProductoStock.Cantidad)
                                    {
                                        throw new Exception(string.Format("No existe Stock suficiente para el lote {0} del producto {1}.", movimientoProductoStock.LoteSerie, movimientoProducto.CodigoProducto));   
                                    }
                                }
                            }

                            movimientoProductoRepository.InsertMovimientosFisico(transaccion.MovimientoProducto, transaction);
                        }

                        #endregion

                        #region Impuestos

                        foreach (var transaccionImpuesto in transaccion.TransaccionImpuesto)
                        {
                            transaccionImpuesto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        transaccionImpuestoRepository.InsertImpuestos(transaccion.TransaccionImpuesto, transaction);

                        #endregion

                        #region Actualizar Serie Documento

                        serieDocumentoRepository.UpdateSerieDocumento(transaccion.IdSerieDocumento, transaccion.IdSucursal, transaccion.UsuarioModificacion, transaction);

                        #endregion

                        #region Actualizar Orden de Pedido

                        if (transaccion.IdTransaccionReferencia != null)
                        {
                            transaccion.Estado = (int)TipoEstadoDocumento.Aprobado;
                            transaccionRepository.UpdateReferenciaTransaccion(transaccion, transaction);
                        }

                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
            return true;
        }

        public bool Actualizar(Transaccion transaccion)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Obtener Movimiento de Productos

                        var movimientosModificados = movimientoProductoRepository.GetByTransaccion(transaccion.IdTransaccion, transaction);

                        #endregion

                        #region Revertir ProductoStock

                        foreach (var movimientoProducto in movimientosModificados)
                        {
                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                movimientoProductoStock.SignoStock = -1 * movimientoProducto.SignoStock;
                                movimientoProductoStock.IdPresentacion = movimientoProducto.IdPresentacion;
                                movimientoProductoStock.UsuarioCreacion = movimientoProducto.UsuarioCreacion;
                                movimientoProductoStock.TipoClasificacion = movimientoProducto.TipoClasificacion;
                            }
                        }

                        if (transaccion.IdTransaccionReferencia.HasValue)
                        {
                            productoStockRepository.UpdateProductoStockFisico(movimientosModificados, transaction);
                        }
                        else
                        {
                            productoStockRepository.UpdateProductoStock(movimientosModificados, transaction);
                        }

                        #endregion

                        #region Eliminar MovimientoProducto

                        movimientoProductoRepository.DeleteByTransaccion(transaccion.IdTransaccion, transaction);

                        #endregion

                        #region Eliminar Impuesto

                        transaccionImpuestoRepository.DeleteByTransaccion(transaccion.IdTransaccion, transaction);

                        #endregion

                        #region Actualizar Transaccion

                        transaccionRepository.UpdateTransaccion(transaccion, transaction);

                        #endregion

                        #region Movimiento Producto

                        if (transaccion.IdTransaccionReferencia == null)
                        {
                            foreach (var movimientoProducto in transaccion.MovimientoProducto)
                            {
                                movimientoProducto.IdTransaccion = transaccion.IdTransaccion;

                                var productos = productoStockRepository.GetByAlmacenPresentacion(movimientoProducto.IdAlmacen.Value, movimientoProducto.IdPresentacion, transaction);
                                var cantidad = movimientoProducto.Cantidad;
                                var secuencia = 0;

                                foreach (var productoStock in productos)
                                {
                                    cantidad -= productoStock.StockLogico;

                                    var movimientoProductoStock = new MovimientoProductoStock
                                    {
                                        Secuencia = ++secuencia,
                                        LoteSerie = productoStock.Lote,
                                        Cantidad = productoStock.StockLogico,
                                        IdAlmacen = movimientoProducto.IdAlmacen.Value,
                                        FechaVencimiento = productoStock.FechaVencimiento
                                    };
                                    if (cantidad > 0)
                                    {
                                        movimientoProducto.MovimientoProductoStock.Add(movimientoProductoStock);
                                    }
                                    else if (cantidad == 0)
                                    {
                                        movimientoProducto.MovimientoProductoStock.Add(movimientoProductoStock);
                                        break;
                                    }
                                    else
                                    {
                                        movimientoProductoStock.Cantidad = cantidad + productoStock.StockLogico;
                                        movimientoProducto.MovimientoProductoStock.Add(movimientoProductoStock);
                                        break;
                                    }
                                }
                            }

                            movimientoProductoRepository.InsertMovimientos(transaccion.MovimientoProducto, transaction);
                        }
                        else
                        {
                            foreach (var movimientoProducto in transaccion.MovimientoProducto)
                            {
                                movimientoProducto.IdTransaccion = transaccion.IdTransaccion;

                                foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                                {
                                    var productoStock = productoStockRepository.GetByPresentacionAlmacenLote(
                                            movimientoProducto.IdPresentacion,
                                            movimientoProductoStock.IdAlmacen,
                                            movimientoProductoStock.LoteSerie);

                                    if (productoStock.StockFisico < movimientoProductoStock.Cantidad)
                                    {
                                        throw new Exception(string.Format("No existe Stock suficiente para el lote {0} del producto {1}.", movimientoProductoStock.LoteSerie, movimientoProducto.CodigoProducto));
                                    }
                                }
                            }

                            movimientoProductoRepository.InsertMovimientosFisico(transaccion.MovimientoProducto, transaction);
                        }

                        #endregion

                        #region Impuestos

                        foreach (var transaccionImpuesto in transaccion.TransaccionImpuesto)
                        {
                            transaccionImpuesto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        transaccionImpuestoRepository.InsertImpuestos(transaccion.TransaccionImpuesto, transaction);

                        #endregion

                        #region Actualizar Orden de Pedido

                        if (transaccion.IdTransaccionReferencia != null)
                        {
                            transaccion.Estado = (int)TipoEstadoDocumento.Aprobado;
                            transaccionRepository.UpdateReferenciaTransaccion(transaccion, transaction);
                        }

                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
            return true;
        }

        public bool Eliminar(Transaccion transaccion)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Obtener Movimiento de Productos

                        var movimientos = movimientoProductoRepository.GetByTransaccion(transaccion.IdTransaccion, transaction);

                        #endregion

                        #region Revertir ProductoStock

                        foreach (var movimientoProducto in movimientos)
                        {
                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                movimientoProductoStock.SignoStock = -1 * movimientoProducto.SignoStock;
                                movimientoProductoStock.IdPresentacion = movimientoProducto.IdPresentacion;
                                movimientoProductoStock.UsuarioCreacion = movimientoProducto.UsuarioCreacion;
                                movimientoProductoStock.TipoClasificacion = movimientoProducto.TipoClasificacion;
                            }
                        }

                        if (transaccion.IdTransaccionReferencia == null)
                        {
                            productoStockRepository.UpdateProductoStock(movimientos, transaction);
                        }
                        else
                        {
                            productoStockRepository.UpdateProductoStockFisico(movimientos, transaction);
                        }

                        #endregion

                        #region Eliminar Transaccion

                        transaccionRepository.DeleteTransaccion(transaccion.IdTransaccion, (int)TipoEstado.Inactivo, transaccion.UsuarioModificacion, transaction);

                        #endregion

                        #region Actualizar Orden de Pedido

                        if (transaccion.IdTransaccionReferencia != null)
                        {
                            transaccion.Estado = (int)TipoEstadoDocumento.Pendiente;
                            transaccionRepository.UpdateReferenciaTransaccion(transaccion, transaction);
                        }

                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
            return true;
        }

        public List<SerieDocumento> GetSeriesOrdenPedidoAprobados(int idEmpresa, int idSucursal, int tipoDocumento, int estado)
        {
            var serieDocumentos = new List<SerieDocumento>();

            var command = Database.GetStoredProcCommand("SelectSerieDocumentoOrdenPedido");
            Database.AddInParameter(command, "pEmpresa", DbType.Int32, idEmpresa);
            Database.AddInParameter(command, "pIdSucursal", DbType.Int32, idSucursal);
            Database.AddInParameter(command, "pTipoDocumento", DbType.Int32, tipoDocumento);
            Database.AddInParameter(command, "pEstado", DbType.Int32, estado);

            using (var dr = Database.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    serieDocumentos.Add(new SerieDocumento
                    {
                        IdSerieDocumento = dr.GetInt32(dr.GetOrdinal("idseriedocumento")),
                        Serie = dr.GetString(dr.GetOrdinal("Serie"))
                    });
                }
            }
            return serieDocumentos;
        }

        public List<Comun> GetNumerosOrdenPedidoAprobados(int idEmpresa, int idSucursal, int idSerie, int estado)
        {
            var serieDocumentos = new List<Comun>();

            var command = Database.GetStoredProcCommand("SelectNumerosOrdenPedidoBySerie");
            Database.AddInParameter(command, "pEmpresa", DbType.Int32, idEmpresa);
            Database.AddInParameter(command, "pIdserie", DbType.Int32, idSerie);
            Database.AddInParameter(command, "pIdSucursal", DbType.Int32, idSucursal);
            Database.AddInParameter(command, "pEstado", DbType.Int32, estado);

            using (var dr = Database.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    serieDocumentos.Add(new Comun
                    {
                        Id = dr.GetInt32(dr.GetOrdinal("idTransaccion")),
                        Nombre = dr.GetString(dr.GetOrdinal("NumeroDocumento")),
                    });
                }
            }
            return serieDocumentos;
        }

        public IList<MovimientoProducto> ObtenerVentaExportar(int idTransaccion)
        {
            return GetGeneric<MovimientoProducto>("usp_ObtenerVentaExcel", idTransaccion);
        }

        #endregion

    }
}
