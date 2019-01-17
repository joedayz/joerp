
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Linq;
    using Business.Entity;
    using Business.Entity.DTO;
    using Interfaces;

    public class SalidaOtroConceptoRepository : Repository<SalidaOtroConceptoDTO>, ISalidaOtroConceptoRepository
    {
        #region Variables

        private readonly ITransaccionRepository transaccionRepository = new TransaccionRepository();
        private readonly ISerieDocumentoRepository serieDocumentoRepository = new SerieDocumentoRepository();
        private readonly IMovimientoProductoRepository movimientoProductoRepository = new MovimientoProductoRepository();
        private readonly IProductoStockRepository productoStockRepository = new ProductoStockRepository();
        
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

                        var movimientos = transaccion.MovimientoProducto.ToList();

                        foreach (var movimientoProducto in movimientos)
                        {
                            movimientoProducto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientos(movimientos, transaction);

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

                        var movimientosPorModificar = movimientoProductoRepository.GetByTransaccion(transaccion.IdTransaccion, transaction);

                        #endregion

                        #region Revertir ProductoStock

                        foreach (var movimientoProducto in movimientosPorModificar)
                        {
                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                movimientoProductoStock.SignoStock = -1 * movimientoProducto.SignoStock;
                                movimientoProductoStock.IdPresentacion = movimientoProducto.IdPresentacion;
                                movimientoProductoStock.UsuarioCreacion = movimientoProducto.UsuarioCreacion;
                                movimientoProductoStock.TipoClasificacion = movimientoProducto.TipoClasificacion;
                            }
                        }

                        productoStockRepository.UpdateProductoStock(movimientosPorModificar, transaction);

                        #endregion

                        #region Eliminar MovimientoProducto

                        movimientoProductoRepository.DeleteByTransaccion(transaccion.IdTransaccion, transaction);

                        #endregion

                        #region Actualizar Transaccion

                        transaccionRepository.UpdateTransaccion(transaccion, transaction);

                        #endregion

                        #region Movimiento Producto

                        var movimientos = transaccion.MovimientoProducto.ToList();

                        foreach (var movimientoProducto in movimientos)
                        {
                            movimientoProducto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientos(movimientos, transaction);

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

                        int idEmpresa = 0, idSucursal = 0, idTransaccionReferencia = 0;
                        var fechaDocumento = DateTime.Now.Date;

                        var comandoSelectTrasaccion = Database.GetSqlStringCommand("SELECT IdTransaccionReferencia, IdEmpresa, IdSucursal, FechaDocumento FROM transaccion WHERE IdTransaccion = " + idTransaccion);

                        using (var dr = Database.ExecuteReader(comandoSelectTrasaccion, transaction))
                        {
                            if (dr.Read())
                            {
                                idEmpresa = dr.GetInt32(dr.GetOrdinal("IdEmpresa"));
                                idSucursal = dr.GetInt32(dr.GetOrdinal("IdSucursal"));
                                fechaDocumento = dr.GetDateTime(dr.GetOrdinal("FechaDocumento"));
                                idTransaccionReferencia = dr.GetInt32(dr.GetOrdinal("idTransaccionReferencia"));
                            }
                        }

                        #endregion

                        #region Obtener Movimiento de Productos

                        var movimientos = movimientoProductoRepository.GetByTransaccion(idTransaccion, transaction);

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
