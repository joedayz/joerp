
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Linq;
    using Business.Entity;
    using Helpers.Enums;
    using Interfaces;

    public class AjusteInventarioRepository : Repository<Transaccion>, IAjusteInventarioRepository
    {
        #region Variables

        private readonly ITransaccionRepository transaccionRepository = new TransaccionRepository();
        private readonly IProductoPrecioRepository productoPrecioRepository = new ProductoPrecioRepository();
        private readonly ISerieDocumentoRepository serieDocumentoRepository = new SerieDocumentoRepository();
        private readonly IMovimientoProductoRepository movimientoProductoRepository = new MovimientoProductoRepository();

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

                        #region Asignar el Costo actualizado

                        var movimientos = transaccion.MovimientoProducto.ToList();

                        foreach (var movimientoProducto in movimientos)
                        {
                            var costoActual = productoPrecioRepository.GetCostoByPresentacion(transaccion.IdEmpresa, transaccion.IdSucursal, movimientoProducto.IdPresentacion, transaction);
                            movimientoProducto.Costo = costoActual;
                            movimientoProducto.PrecioBase = costoActual;
                            movimientoProducto.PrecioNeto = movimientoProducto.Cantidad * costoActual;
                            movimientoProducto.SubTotal = movimientoProducto.PrecioNeto;
                        }

                        #endregion

                        #region Movimiento Producto

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

                        var movimientosModificados = movimientoProductoRepository.GetByTransaccion(transaccion.IdTransaccion, transaction);

                        #endregion

                        #region Actualizar Estado de Movimiento Producto

                        movimientoProductoRepository.UpdateEstadosMovimientos(movimientosModificados, (int)TipoEstadoTransaccion.Anulado, transaction);

                        #endregion

                        #region Asignar el Costo actualizado

                        foreach (var movimientoProducto in movimientosModificados)
                        {
                            var costoActual = productoPrecioRepository.GetCostoByPresentacion(transaccion.IdEmpresa, transaccion.IdSucursal, movimientoProducto.IdPresentacion, transaction);
                            movimientoProducto.Costo = costoActual;
                            movimientoProducto.PrecioBase = costoActual;
                            movimientoProducto.PrecioNeto = movimientoProducto.Cantidad * costoActual;
                            movimientoProducto.SubTotal = movimientoProducto.PrecioNeto;
                        }

                        #endregion

                        #region Ingresar Movimiento Producto con Signo cambiado y estado Anulado

                        foreach (var movimientoProducto in movimientosModificados)
                        {
                            movimientoProducto.SignoStock = -1 * movimientoProducto.SignoStock;
                            movimientoProducto.Estado = (int)TipoEstadoTransaccion.Anulado;
                            movimientoProducto.FechaRegistro = transaccion.FechaRegistro;
                        }

                        movimientoProductoRepository.InsertMovimientos(movimientosModificados, transaction);

                        #endregion

                        #region Asignar el Costo actualizado

                        var movimientos = transaccion.MovimientoProducto.ToList();

                        foreach (var movimientoProducto in movimientos)
                        {
                            var costoActual = productoPrecioRepository.GetCostoByPresentacion(transaccion.IdEmpresa, transaccion.IdSucursal, movimientoProducto.IdPresentacion, transaction);
                            movimientoProducto.Costo = costoActual;
                            movimientoProducto.PrecioBase = costoActual;
                            movimientoProducto.PrecioNeto = movimientoProducto.Cantidad * costoActual;
                            movimientoProducto.SubTotal = movimientoProducto.PrecioNeto;
                        }

                        #endregion

                        #region Movimiento Producto

                        foreach (var movimientoProducto in movimientos)
                        {
                            movimientoProducto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientos(movimientos, transaction);

                        #endregion

                        #region Actualizar Transaccion

                        transaccionRepository.UpdateTransaccion(transaccion, transaction);

                        #endregion

                        transaction.Commit();
                        transaccion.MovimientoProducto.Clear();
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

                        int idEmpresa = 0, idSucursal = 0;
                        DateTime fechaDocumento = DateTime.Now.Date;

                        var comandoSelectTrasaccion = Database.GetSqlStringCommand("SELECT IdEmpresa, IdSucursal, FechaDocumento FROM transaccion WHERE IdTransaccion = " + idTransaccion);

                        using (var dr = Database.ExecuteReader(comandoSelectTrasaccion, transaction))
                        {
                            if (dr.Read())
                            {
                                idEmpresa = dr.GetInt32(dr.GetOrdinal("IdEmpresa"));
                                idSucursal = dr.GetInt32(dr.GetOrdinal("IdSucursal"));
                                fechaDocumento = dr.GetDateTime(dr.GetOrdinal("FechaDocumento"));
                            }
                        }

                        #endregion

                        #region Obtener Movimiento de Productos

                        var movimientos = movimientoProductoRepository.GetByTransaccion(idTransaccion, transaction);

                        #endregion

                        #region Actualizar Estado de Movimiento Producto

                        movimientoProductoRepository.UpdateEstadosMovimientos(movimientos, (int)TipoEstadoTransaccion.Anulado, transaction);

                        #endregion

                        #region Asignar el Costo actualizado

                        foreach (var movimientoProducto in movimientos)
                        {
                            var costoActual = productoPrecioRepository.GetCostoByPresentacion(idEmpresa, idSucursal, movimientoProducto.IdPresentacion, transaction);
                            movimientoProducto.Costo = costoActual;
                            movimientoProducto.PrecioBase = costoActual;
                            movimientoProducto.PrecioNeto = movimientoProducto.Cantidad * costoActual;
                            movimientoProducto.SubTotal = movimientoProducto.PrecioNeto;
                        }

                        #endregion

                        #region Ingresar Movimiento Producto con Signo cambiado 

                        foreach (var movimientoProducto in movimientos)
                        {
                            movimientoProducto.SignoStock = -1 * movimientoProducto.SignoStock;
                            movimientoProducto.Estado = (int)TipoEstadoTransaccion.Registrado;
                            movimientoProducto.FechaRegistro = fechaRegistro;
                        }

                        movimientoProductoRepository.InsertMovimientos(movimientos, transaction);

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
