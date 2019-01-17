
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Business.Entity.DTO;
    using Interfaces;

    public class OrdenPedidoRepository : Repository<OrdenPedido>, IOrdenPedidoRepository
    {
        #region Variables

        private readonly ITransaccionRepository transaccionRepository = new TransaccionRepository();
        private readonly ISerieDocumentoRepository serieDocumentoRepository = new SerieDocumentoRepository();
        private readonly IMovimientoProductoRepository movimientoProductoRepository = new MovimientoProductoRepository();
        private readonly ITransaccionImpuestoRepository transaccionImpuestoRepository = new TransaccionImpuestoRepository();

        #endregion

        #region Metodos

        public bool Insertar(OrdenPedido transaccion)
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

                        foreach (var movimiento in transaccion.MovimientoProducto)
                        {
                            movimiento.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientosLogico(transaccion.MovimientoProducto.ToList(), transaction);

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
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
            return true;
        }

        public bool Actualizar(OrdenPedido transaccion)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Actualizar Transaccion

                        transaccionRepository.UpdateTransaccion(transaccion, transaction);

                        #endregion

                        #region Eliminar Movimiento

                        movimientoProductoRepository.DeleteByTransaccion(transaccion.IdTransaccion, transaction);

                        #endregion

                        #region Movimiento Producto

                        foreach (var movimiento in transaccion.MovimientoProducto)
                        {
                            movimiento.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientosLogico(transaccion.MovimientoProducto.ToList(), transaction);

                        #endregion

                        #region Eliminar Impuesto

                        transaccionImpuestoRepository.DeleteByTransaccion(transaccion.IdTransaccion, transaction);

                        #endregion

                        #region Impuestos

                        foreach (var transaccionImpuesto in transaccion.TransaccionImpuesto)
                        {
                            transaccionImpuesto.IdTransaccion = transaccion.IdTransaccion;
                        }

                        transaccionImpuestoRepository.InsertImpuestos(transaccion.TransaccionImpuesto.ToList(), transaction);

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

        public bool Eliminar(int idTransaccion, int idUsuario, int estado)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        transaccionRepository.DeleteTransaccion(idTransaccion, estado, idUsuario, transaction);
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

        public bool Aprobar(int idTransaccion, int idUsuarioModificacion, int estado)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        var comandoDeleteTransaccion = Database.GetStoredProcCommand("DeleteTransaccion");
                        Database.AddInParameter(comandoDeleteTransaccion, "pIdTransaccion", DbType.Int32, idTransaccion);
                        Database.AddInParameter(comandoDeleteTransaccion, "pUsuarioModificacion", DbType.Int32, idUsuarioModificacion);
                        Database.AddInParameter(comandoDeleteTransaccion, "pEstado", DbType.Int32, estado);
                        Database.ExecuteNonQuery(comandoDeleteTransaccion, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
            return true;
        }

        public IList<OrdenPedido> BuscarOrden(int idOperacion, int idProveedor, string desde, string hasta, string documento)
        {
            return Get("usp_Buscar_OrdenCompra", idOperacion, idProveedor, desde, hasta, documento);
        }

        #endregion
    }
}
