
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Business.Entity;
    using Helpers;
    using Helpers.Enums;
    using Interfaces;

    public class IngresoTransferenciaAlmacenRepository : Repository<Transaccion>, IIngresoTransferenciaAlmacenRepository
    {
        #region Variables

        private readonly ITransaccionRepository transaccionRepository = new TransaccionRepository();
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
                        
                        #region Movimiento Producto

                        foreach (var movimientoProducto in transaccion.MovimientoProducto)
                        {
                            movimientoProducto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientos(transaccion.MovimientoProducto, transaction);

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

                        #region Ingresar Movimiento Producto con Signo cambiado y estado Anulado

                        foreach (var movimientoProducto in movimientosModificados)
                        {
                            movimientoProducto.IdTransaccion = movimientoProducto.IdTransaccion;
                            movimientoProducto.SignoStock = -1 * movimientoProducto.SignoStock;
                            movimientoProducto.Estado = (int)TipoEstadoTransaccion.Anulado;
                            movimientoProducto.FechaRegistro = transaccion.FechaRegistro;
                        }

                        movimientoProductoRepository.InsertMovimientos(movimientosModificados, transaction);

                        #endregion

                        #region Eliminar Impuesto

                        transaccionImpuestoRepository.DeleteByTransaccion(transaccion.IdTransaccion, transaction);

                        #endregion

                        #region Modificar Transaccion

                        transaccionRepository.UpdateTransaccion(transaccion, transaction);

                        #endregion

                        #region Movimiento Producto

                        foreach (var movimientoProducto in transaccion.MovimientoProducto)
                        {
                            movimientoProducto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientos(transaccion.MovimientoProducto, transaction);

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

                        int? idTipoCambio = null;
                        int idEmpresa = 0, idSucursal = 0;
                        DateTime fechaDocumento = DateTime.Now.Date;

                        var comandoSelectTrasaccion = Database.GetSqlStringCommand("SELECT IdEmpresa, IdSucursal, IdTipoCambio, FechaDocumento FROM transaccion WHERE IdTransaccion = " + idTransaccion);

                        using (var dr = Database.ExecuteReader(comandoSelectTrasaccion, transaction))
                        {
                            if (dr.Read())
                            {
                                idEmpresa = dr.GetInt32(dr.GetOrdinal("IdEmpresa"));
                                idSucursal = dr.GetInt32(dr.GetOrdinal("IdSucursal"));
                                fechaDocumento = dr.GetDateTime(dr.GetOrdinal("FechaDocumento"));
                                idTipoCambio = dr.IsDBNull(dr.GetOrdinal("IdTipoCambio")) ? (int?)null : dr.GetInt32(dr.GetOrdinal("IdTipoCambio"));
                            }
                        }

                        #endregion

                        #region Obtener Movimiento de Productos

                        var movimientos = movimientoProductoRepository.GetByTransaccion(idTransaccion, transaction);

                        #endregion

                        #region Actualizar Estado de Movimiento Producto

                        movimientoProductoRepository.UpdateEstadosMovimientos(movimientos, (int)TipoEstadoTransaccion.Anulado, transaction);

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

        public List<SerieDocumento> GetSerieDocumentoIngresoTransferencia(int tipoDocumento, int idalmacenDestino,int idSucursal, int idEmpresa)
        {
            var serieDocumentos = new List<SerieDocumento>();

            var command = Database.GetStoredProcCommand("SelectSerieDocumentoIngresoTransferencia");
            Database.AddInParameter(command, "ptipodocumento", DbType.Int32, tipoDocumento);
            Database.AddInParameter(command, "pidalmacendestino", DbType.Int32, idalmacenDestino);
            Database.AddInParameter(command, "pidsucursal", DbType.Int32, idSucursal);
            Database.AddInParameter(command, "pempresa", DbType.Int32, idEmpresa);

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

        public List<Comun> GetNumerosBySerie(int idSerie, int idalmacenDestino, int idSucursal, int idEmpresa , int idOperacion)
        {
            var serieDocumentos = new List<Comun>();

            var command = Database.GetStoredProcCommand("SelectNumerosBySerie");
            Database.AddInParameter(command, "pIdserie", DbType.Int32, idSerie);
            Database.AddInParameter(command, "pidalmacendestino", DbType.Int32, idalmacenDestino);
            Database.AddInParameter(command, "pidsucursal", DbType.Int32, idSucursal);
            Database.AddInParameter(command, "pempresa", DbType.Int32, idEmpresa);
            Database.AddInParameter(command, "pidOperacion", DbType.Int32, idOperacion);

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

        #endregion
    }
}
