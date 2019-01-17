
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Linq;
    using Business.Entity;
    using Business.Entity.DTO;
    using Helpers.Enums;
    using Interfaces;

    public class DistribucionMercaderiaRepository : Repository<DistribucionMercaderia>, IDistribucionMercaderiaRepository
    {
        private readonly ITransaccionRepository transaccionRepository = new TransaccionRepository();
        private readonly ISerieDocumentoRepository serieDocumentoRepository = new SerieDocumentoRepository();
        private readonly IMovimientoProductoRepository movimientoProductoRepository = new MovimientoProductoRepository();
        private readonly IProductoStockRepository productoStockRepository = new ProductoStockRepository();

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

                        var movimientos = transaccion.MovimientoProducto.ToList();

                        foreach (var movimientoProducto in movimientos)
                        {
                            movimientoProducto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientos(movimientos, transaction);

                        #endregion

                        #region Actualizar Producto Stock Origen

                        foreach (var movimientoProducto in transaccion.MovimientoProducto)
                        {
                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                movimientoProductoStock.IdAlmacen = transaccion.IdAlmacen.Value;
                                movimientoProductoStock.SignoStock =  -1*movimientoProducto.SignoStock;
                                movimientoProductoStock.IdPresentacion = movimientoProducto.IdPresentacion;
                                movimientoProductoStock.UsuarioCreacion = movimientoProducto.UsuarioCreacion;
                                movimientoProductoStock.TipoClasificacion = movimientoProducto.TipoClasificacion;
                            }

                            movimientoProducto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        productoStockRepository.UpdateProductoStock(movimientos, transaction);

                        #endregion

                        #region Actualizar Serie Documento

                        serieDocumentoRepository.UpdateSerieDocumento(transaccion.IdSerieDocumento, transaccion.IdSucursal, transaccion.UsuarioModificacion, transaction);

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

        public void Eliminar(DistribucionMercaderia distribucion)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Obtener Movimiento de Productos

                        var movimientos = movimientoProductoRepository.GetByTransaccion(distribucion.IdTransaccion, transaction);

                        #endregion

                        #region Actualizar Producto Stock Destino

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

                        productoStockRepository.UpdateProductoStock(movimientos, transaction);

                        #endregion

                        #region Actualizar Proucto Stock Origen

                        foreach (var movimientoProducto in movimientos)
                        {
                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                movimientoProductoStock.IdAlmacen = distribucion.IdAlmacen.Value;
                                movimientoProductoStock.SignoStock = movimientoProducto.SignoStock;
                                movimientoProductoStock.IdPresentacion = movimientoProducto.IdPresentacion;
                                movimientoProductoStock.UsuarioCreacion = movimientoProducto.UsuarioCreacion;
                                movimientoProductoStock.TipoClasificacion = movimientoProducto.TipoClasificacion;
                            }
                        }

                        productoStockRepository.UpdateProductoStock(movimientos, transaction);

                        #endregion

                        #region Eliminar Transaccion

                        transaccionRepository.DeleteTransaccion(distribucion.IdTransaccion, (int)TipoEstado.Inactivo, distribucion.UsuarioModificacion, transaction);

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
        }
    }
}
