
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Linq;
    using Business.Entity;
    using Business.Entity.DTO;
    using Interfaces;

    public class SalidaTransferenciaAlmacenRepository : Repository<Transferencia>, ISalidaTransferenciaAlmacenRepository
    {
        #region Variables

        private readonly ITransaccionRepository transaccionRepository = new TransaccionRepository();
        private readonly IProductoStockRepository productoStockRepository = new ProductoStockRepository();
        private readonly ISerieDocumentoRepository serieDocumentoRepository = new SerieDocumentoRepository();
        private readonly IMovimientoProductoRepository movimientoProductoRepository = new MovimientoProductoRepository();
        private readonly ITransaccionImpuestoRepository transaccionImpuestoRepository = new TransaccionImpuestoRepository();

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

                        //resta
                        #region Movimiento Producto

                        foreach (var movimientoProducto in transaccion.MovimientoProducto)
                        {
                            movimientoProducto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientos(transaccion.MovimientoProducto.ToList(), transaction);

                        #endregion

                        //suma
                        #region Actualizar PRoducto Stock 

                        foreach (var movimientoProducto in transaccion.MovimientoProducto)
                        {
                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                if (transaccion.IdAlmacenAlterno != null)
                                    movimientoProductoStock.IdAlmacen=transaccion.IdAlmacenAlterno.Value;
                                movimientoProductoStock.SignoStock = -1*movimientoProducto.SignoStock;
                                movimientoProductoStock.IdPresentacion = movimientoProducto.IdPresentacion;
                                movimientoProductoStock.UsuarioCreacion = movimientoProducto.UsuarioCreacion;
                                movimientoProductoStock.TipoClasificacion = movimientoProducto.TipoClasificacion;
                            }
                           
                            movimientoProducto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        productoStockRepository.UpdateProductoStock(transaccion.MovimientoProducto.ToList(), transaction);

                        #endregion

                        #region Impuestos

                        foreach (var transaccionImpuesto in transaccion.TransaccionImpuesto)
                        {
                            transaccionImpuesto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        transaccionImpuestoRepository.InsertImpuestos(transaccion.TransaccionImpuesto.ToList(), transaction);

                        #endregion

                        #region Actualizar Serie Documento

                        serieDocumentoRepository.UpdateSerieDocumento(transaccion.IdSerieDocumento, transaccion.IdSucursal, transaccion.UsuarioModificacion, transaction);

                        #endregion

                        transaction.Commit();
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
            return true;
        }

        public bool Eliminar(int idTransaccion, int idUsuario, int estado, DateTime fechaRegistro)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Obtener Datos de Transacción Actual

                        var idAlmacenAlterno = 0;

                        var comandoSelectTrasaccion = Database.GetSqlStringCommand("SELECT IdAlmacenAlterno FROM transaccion WHERE IdTransaccion = " + idTransaccion);

                        using (var dr = Database.ExecuteReader(comandoSelectTrasaccion, transaction))
                        {
                            if (dr.Read())
                            {
                                idAlmacenAlterno = dr.GetInt32(dr.GetOrdinal("IdAlmacenAlterno"));
                            }
                        }

                        #endregion

                        #region Obtener Movimiento de Productos

                        var movimientos = movimientoProductoRepository.GetByTransaccion(idTransaccion, transaction);

                        #endregion

                        #region Actualizar Proucto Stock Origen

                        foreach (var movimientoProducto in movimientos)
                        {
                            movimientoProducto.IdTransaccion = idTransaccion;

                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                movimientoProductoStock.IdAlmacen = movimientoProducto.IdAlmacen.Value;
                                movimientoProductoStock.SignoStock = movimientoProducto.SignoStock;
                                movimientoProductoStock.IdPresentacion = movimientoProducto.IdPresentacion;
                                movimientoProductoStock.UsuarioCreacion = movimientoProducto.UsuarioCreacion;
                                movimientoProductoStock.TipoClasificacion = movimientoProducto.TipoClasificacion;
                            }
                        }

                        productoStockRepository.UpdateProductoStock(movimientos, transaction);

                        #endregion

                        #region Actualizar Producto Stock Destino

                        foreach (var movimientoProducto in movimientos)
                        {
                            movimientoProducto.IdTransaccion = idTransaccion;

                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                movimientoProductoStock.IdAlmacen = idAlmacenAlterno;
                                movimientoProductoStock.SignoStock = -1*movimientoProducto.SignoStock;
                                movimientoProductoStock.IdPresentacion = movimientoProducto.IdPresentacion;
                                movimientoProductoStock.UsuarioCreacion = movimientoProducto.UsuarioCreacion;
                                movimientoProductoStock.TipoClasificacion = movimientoProducto.TipoClasificacion;
                            }
                        }

                        productoStockRepository.UpdateProductoStock(movimientos, transaction);

                        #endregion

                        #region Eliminar Transaccion

                        transaccionRepository.DeleteTransaccion(idTransaccion, estado, idUsuario, transaction);

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

        #endregion
    }
}
