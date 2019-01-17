
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

    public class DespachoRepository : Repository<Transaccion>, IDespachoRepository
    {
        #region Variables

        private readonly ITransaccionRepository transaccionRepository = new TransaccionRepository();
        private readonly ISerieDocumentoRepository serieDocumentoRepository = new SerieDocumentoRepository();
        private readonly IProductoPrecioRepository productoPrecioRepository = new ProductoPrecioRepository();
        private readonly IMovimientoProductoRepository movimientoProductoRepository = new MovimientoProductoRepository();

        #endregion

        #region Metodos

        public bool Insertar(Transaccion transaccion, int estadoDocAlterno)
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

                        #region Asignar el Costo actualizado a Movimientos

                        var movimientos = transaccion.MovimientoProducto.ToList();

                        foreach (var movimientoProducto in movimientos)
                        {
                            var costoActual = productoPrecioRepository.GetCostoByPresentacion(transaccion.IdEmpresa, transaccion.IdSucursal, movimientoProducto.IdPresentacion, transaction);
                            movimientoProducto.Costo = costoActual;
                            movimientoProducto.PrecioNeto = movimientoProducto.Cantidad * costoActual;
                            movimientoProducto.SubTotal = movimientoProducto.PrecioNeto;
                        }

                        #endregion

                        #region Movimiento Producto

                        foreach (var movimientoProducto in movimientos)
                        {
                            movimientoProducto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientosFisico(movimientos, transaction);

                        #endregion

                        #region Actualizar Serie Documento

                        serieDocumentoRepository.UpdateSerieDocumento(transaccion.IdSerieDocumento, transaccion.IdSucursal, transaccion.UsuarioModificacion, transaction);

                        #endregion

                        #region Actualizar Documentos Relacionados

                        if (transaccion.IdTransaccionReferencia != null)
                        {
                            var comandoOrdenCompra = Database.GetStoredProcCommand("UpdateReferenciaDespacho");
                            Database.AddInParameter(comandoOrdenCompra, "pIdTransaccion", DbType.Int32, transaccion.IdTransaccionReferencia);
                            Database.AddInParameter(comandoOrdenCompra, "pIdTransaccionReferencia", DbType.Int32, transaccion.IdTransaccion);
                            Database.AddInParameter(comandoOrdenCompra, "pEstado", DbType.Int32, (int)TipoEstadoTransaccion.Atendido);
                            Database.AddInParameter(comandoOrdenCompra, "pUsuarioModificacion", DbType.Int32, transaccion.UsuarioModificacion);

                            Database.ExecuteNonQuery(comandoOrdenCompra, transaction);
                        }

                        #endregion

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return true;
        }

        public bool Actualizar(Transaccion transaccion, int estadoDocAlterno)
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

                        #region Revertir Movimiento de Stock

                        var comandoRevertirMovimientoStock = Database.GetStoredProcCommand("RevertirMovimientoProductoStockFisico");
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pIdPresentacion", DbType.Int32);
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pIdProducto", DbType.Int32);
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pIdAlmacen", DbType.Int32);
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pLoteSerie", DbType.String);
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pSignoStock", DbType.Int32);
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pCantidad", DbType.Decimal);

                        foreach (var movimiento in movimientos)
                        {
                            foreach (var movimientoStock in movimiento.MovimientoProductoStock)
                            {
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pIdPresentacion", movimiento.IdPresentacion);
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pIdProducto", movimiento.IdProducto);
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pIdAlmacen", movimientoStock.IdAlmacen);
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pLoteSerie", movimientoStock.LoteSerie);
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pSignoStock", movimiento.SignoStock);
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pCantidad", movimientoStock.Cantidad);
                                Database.ExecuteNonQuery(comandoRevertirMovimientoStock, transaction);
                            }
                        }

                        #endregion

                        #region Eliminar Movimiento Stock

                        var comandoDeleteMovimientoStock = Database.GetStoredProcCommand("DeleteMovimientoProductoStock");
                        Database.AddInParameter(comandoDeleteMovimientoStock, "pIdMovimientoProducto", DbType.Int32);

                        foreach (var movimiento in movimientos)
                        {
                            Database.SetParameterValue(comandoDeleteMovimientoStock, "pIdMovimientoProducto", movimiento.IdMovimientoProducto);
                            Database.ExecuteNonQuery(comandoDeleteMovimientoStock, transaction);
                        }

                        #endregion

                        #region Eliminar Movimiento

                        movimientoProductoRepository.DeleteByTransaccion(transaccion.IdTransaccion, transaction);

                        #endregion

                        #region Actualizar Transaccion

                        transaccionRepository.UpdateTransaccion(transaccion, transaction);

                        #endregion

                        #region Movimiento Producto

                        foreach (var movimientoProducto in movimientos)
                        {
                            movimientoProducto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientosFisico(movimientos, transaction);

                        #endregion

                        #region Actualizar Documentos Relacionados

                        if (transaccion.IdTransaccionReferencia != null)
                        {
                            var comandoOrdenCompra = Database.GetStoredProcCommand("UpdateReferenciaDespacho");
                            Database.AddInParameter(comandoOrdenCompra, "pIdTransaccion", DbType.Int32, transaccion.IdTransaccionReferencia);
                            Database.AddInParameter(comandoOrdenCompra, "pIdTransaccionReferencia", DbType.Int32, transaccion.IdTransaccion);
                            Database.AddInParameter(comandoOrdenCompra, "pEstado", DbType.Int32, (int)TipoEstadoTransaccion.Atendido);
                            Database.AddInParameter(comandoOrdenCompra, "pUsuarioModificacion", DbType.Int32, transaccion.UsuarioModificacion);

                            Database.ExecuteNonQuery(comandoOrdenCompra, transaction);
                        }

                        #endregion

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return true;
        }

        public bool Eliminar(int idTransaccion, int idUsuario, int estado)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Obtener Datos Transacción

                        int idAlmacen = 0, idEmpresa = 0, idSucursal = 0, idTransaccionReferencia = 0;;
                        DateTime fechaDocumento = DateTime.Now.Date;

                        var comandoSelectAlmacenTrasaccion = Database.GetSqlStringCommand("SELECT IdTransaccionReferencia, IdEmpresa, IdSucursal, FechaDocumento, IdAlmacen FROM transaccion WHERE IdTransaccion = " + idTransaccion);

                        using (var dr = Database.ExecuteReader(comandoSelectAlmacenTrasaccion, transaction))
                        {
                            if (dr.Read())
                            {
                                idEmpresa = dr.GetInt32(dr.GetOrdinal("IdEmpresa"));
                                idSucursal = dr.GetInt32(dr.GetOrdinal("Idsucursal"));
                                fechaDocumento = dr.GetDateTime(dr.GetOrdinal("FechaDocumento"));
                                idAlmacen = dr.GetInt32(dr.GetOrdinal("IdAlmacen"));
                                idTransaccionReferencia = dr.GetInt32(dr.GetOrdinal("IdTransaccionReferencia"));
                            }
                        }

                        #endregion

                        #region Obtener Movimiento de Productos

                        var movimientos = movimientoProductoRepository.GetByTransaccion(idTransaccion, transaction);

                        #endregion

                        #region Revertir Movimiento de Stock

                        var comandoRevertirMovimientoStock = Database.GetStoredProcCommand("RevertirMovimientoProductoStockFisico");
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pIdPresentacion", DbType.Int32);
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pIdProducto", DbType.Int32);
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pIdAlmacen", DbType.Int32);
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pLoteSerie", DbType.String);
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pSignoStock", DbType.Int32);
                        Database.AddInParameter(comandoRevertirMovimientoStock, "pCantidad", DbType.Decimal);

                        foreach (var movimiento in movimientos)
                        {
                            foreach (var movimientoStock in movimiento.MovimientoProductoStock)
                            {
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pIdPresentacion", movimiento.IdPresentacion);
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pIdProducto", movimiento.IdProducto);
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pIdAlmacen", movimientoStock.IdAlmacen);
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pLoteSerie", movimientoStock.LoteSerie);
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pSignoStock", movimiento.SignoStock);
                                Database.SetParameterValue(comandoRevertirMovimientoStock, "pCantidad", movimientoStock.Cantidad);
                                Database.ExecuteNonQuery(comandoRevertirMovimientoStock, transaction);
                            }
                        }

                        #endregion

                        #region Actualizar Documentos Relacionados

                        if (idTransaccionReferencia != 0)
                        {
                            var comandoOrdenCompra = Database.GetStoredProcCommand("UpdateReferenciaDespacho");
                            Database.AddInParameter(comandoOrdenCompra, "pIdTransaccion", DbType.Int32, idTransaccionReferencia);
                            Database.AddInParameter(comandoOrdenCompra, "pIdTransaccionReferencia", DbType.Int32, idTransaccion);
                            Database.AddInParameter(comandoOrdenCompra, "pEstado", DbType.Int32, (int)TipoEstadoTransaccion.Pendiente);
                            Database.AddInParameter(comandoOrdenCompra, "pUsuarioModificacion", DbType.Int32, idUsuario);

                            Database.ExecuteNonQuery(comandoOrdenCompra, transaction);
                        }

                        #endregion

                        #region Eliminar Transaccion

                        transaccionRepository.DeleteTransaccion(idTransaccion, estado, idUsuario, transaction);

                        #endregion                        

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return true;
        }

        public List<Comun> GetDocumentoConDespacho(int idEmpresa, int idSucursal, int idAlmacen)
        {
            var documentos = new List<Comun>();

            var command = Database.GetStoredProcCommand("SelectDocumentoConDespacho");
            Database.AddInParameter(command, "pEmpresa", DbType.Int32, idEmpresa);
            Database.AddInParameter(command, "pIdSucursal", DbType.Int32, idSucursal);
            Database.AddInParameter(command, "pIdAlmacen", DbType.Int32, idAlmacen);
             using (var dr = Database.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    documentos.Add(new Comun
                    {
                        Id = dr.GetInt32(dr.GetOrdinal("TipoDocumentoInterno")),
                        Nombre = dr.GetString(dr.GetOrdinal("Nombre"))
                    });
                }
            }
            return documentos;
        }

        public string GetDocumentoRelacionado(int idTransaccion, int secuencia)
        {
            var command = Database.GetStoredProcCommand("SelectDocumentoTransaccionDocumentoBySecuencia");
            Database.AddInParameter(command, "pIdTransaccion", DbType.Int32, idTransaccion);
            Database.AddInParameter(command, "pSecuencia", DbType.Int32, secuencia);

            var documento = string.Empty;

            using (var dr = Database.ExecuteReader(command))
            {
                if (dr.Read())
                {
                    documento = dr.GetString(dr.GetOrdinal("Documento"));
                }
            }
            return documento;
        }

        public List<Comun> GetSerieDocumentoConDespacho(int idEmpresa, int idDocumento,int idSucursal, int idAlmacen)
        {
            var serieDocumentos = new List<Comun>();

            var command = Database.GetStoredProcCommand("SelectSerieDocumentoConDespacho");
            Database.AddInParameter(command, "pEmpresa", DbType.Int32, idEmpresa);
            Database.AddInParameter(command, "pIdDocumento", DbType.Int32, idDocumento);
            Database.AddInParameter(command, "pIdSucursal", DbType.Int32, idSucursal);
            Database.AddInParameter(command, "pIdAlmacen", DbType.Int32, idAlmacen);

            using (var dr = Database.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    serieDocumentos.Add(new Comun
                    {
                        Id = dr.GetInt32(dr.GetOrdinal("IdSerieDocumento")),
                        Nombre = dr.GetString(dr.GetOrdinal("SerieDocumento"))
                    });
                }
            }
            return serieDocumentos;
        }

        public List<Comun> GetNumeroDocumentoConDespacho(int idEmpresa, int idDocumento, int idSerie, int idSucursal, int idAlmacen)
        {
            var serieDocumentos = new List<Comun>();

            var command = Database.GetStoredProcCommand("SelectNumeroDocumentoConDespacho");
            Database.AddInParameter(command, "pEmpresa", DbType.Int32, idEmpresa);
            Database.AddInParameter(command, "pIdDocumento", DbType.Int32, idDocumento);
            Database.AddInParameter(command, "pIdSerieDocumento", DbType.Int32, idSerie);
            Database.AddInParameter(command, "pIdSucursal", DbType.Int32, idSucursal);
            Database.AddInParameter(command, "pIdAlmacen", DbType.Int32, idAlmacen);
            using (var dr = Database.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    serieDocumentos.Add(new Comun
                    {
                        Id = dr.GetInt32(dr.GetOrdinal("IdTransaccion")),
                        Nombre = dr.GetString(dr.GetOrdinal("NumeroDocumento"))
                    });
                }
            }
            return serieDocumentos;
        }

        public List<Transaccion> GetDocumentosTransaccion(int clienteId, int documentoId, int serieID, string numero, DateTime? fechaInicio, DateTime? fechaFin, int idSucursal, int idAlmacen)
        {
            //DataContext.Transaccion.MergeOption = MergeOption.NoTracking;
            //DataContext.TransaccionDocumento.MergeOption = MergeOption.NoTracking;

            //var query = DataContext.Transaccion.Include("Operacion")
            //            .Where(p => p.ConDespacho == 1 &&
            //                p.Estado != (int)TipoEstadoTransaccion.Parcial && p.Estado != (int)TipoEstadoTransaccion.Atendido &&
            //                p.IdSucursal == idSucursal &&
            //                p.IdAlmacen == idAlmacen &&
            //            (clienteId == 0 ? true : p.IdPersona == clienteId) &&
            //            (documentoId == 0 ? true : (serieID == 0 ?
            //                                        (p.Operacion.TipoDocumentoInterno == documentoId) :
            //                                         (string.IsNullOrEmpty(numero) ?
            //                                                        (p.Operacion.TipoDocumentoInterno == documentoId && p.IdSerieDocumento == serieID) :
            //                                                        (p.Operacion.TipoDocumentoInterno == documentoId && p.IdSerieDocumento == serieID && p.NumeroDocumento == numero)))) &&
            //            (fechaInicio.HasValue ? p.FechaDocumento >= fechaInicio : true) &&
            //            (fechaFin.HasValue ? p.FechaDocumento <= fechaFin : true))
            //            .OrderByDescending(p => p.IdTransaccion);

            //return query.ToList();
            return null;
        }

        public List<DespachoDTO> Buscar(int idCliente, int idTipo, string serie, string numero, int idAlmacen, DateTime? fechaInicio, DateTime? fechaFin, int idOperacion, int start, int rows, out int count)
        {
            //DataContext.Transaccion.MergeOption = MergeOption.NoTracking;
            //DataContext.TransaccionDocumento.MergeOption = MergeOption.NoTracking;

            //var query = DataContext.Transaccion
            //            .Where(p => p.IdOperacion == idOperacion &&
            //           (idCliente == 0 ? true : p.IdPersona == idCliente) &&
            //            (idAlmacen == 0 ? true : p.IdAlmacen == idAlmacen) &&
            //            (idTipo == 0 ? true :
            //                           (string.IsNullOrEmpty(serie) ?
            //                                        p.TransaccionDocumento.Any(d => d.TipoDocumento == idTipo) :
            //                                         (string.IsNullOrEmpty(numero) ?
            //                                                        p.TransaccionDocumento.Any(d => d.TipoDocumento == idTipo && d.Serie == serie) :
            //                                                        p.TransaccionDocumento.Any(d => d.TipoDocumento == idTipo && d.Serie == serie && d.Numero == numero)))) &&
            //            (fechaInicio.HasValue ? p.FechaDocumento >= fechaInicio : true) &&
            //            (fechaFin.HasValue ? p.FechaDocumento <= fechaFin : true))
            //            .OrderByDescending(p => p.IdTransaccion);

            //count = query.Count();

            //var list = query.Skip(start)
            //                .Take(rows)
            //                .Select(p => new DespachoDTO
            //                {
            //                    IdTransaccion = p.IdTransaccion,
            //                    Documento = p.SerieDocumento + "-" + p.NumeroDocumento,
            //                    IdEstado = p.Estado,
            //                    Fecha = p.FechaDocumento
            //                })
            //                .ToList();

            //foreach (var item in list)
            //{
            //    item.Estado = Enum.GetName(typeof(TipoEstadoTransaccion), item.IdEstado);
                
            //    item.DocumentoRelacionado = GetDocumentoRelacionado(item.IdTransaccion, 1); // Guia de Remisión Remitente
            //}
            //return list;
            count = 0;
            return null;
        }

        #endregion
    }
}
