
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Linq;
    using Business.Entity;
    using Business.Entity.DTO;
    using Interfaces;

    public class DevolucionMercaderiaRepository : Repository<DevolucionMercaderia>, IDevolucionMercaderiaRepository
    {
        #region Variables

        private readonly ITransaccionRepository transaccionRepository = new TransaccionRepository();
        private readonly ISerieDocumentoRepository serieDocumentoRepository = new SerieDocumentoRepository();
        private readonly IMovimientoProductoRepository movimientoProductoRepository = new MovimientoProductoRepository();
        private readonly ITransaccionImpuestoRepository transaccionImpuestoRepository = new TransaccionImpuestoRepository();
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

                        foreach (var movimiento in transaccion.MovimientoProducto)
                        {
                            movimiento.IdTransaccion = transaccion.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertOnlyMovimientos(transaccion.MovimientoProducto.ToList(), transaction);

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

        public bool Actualizar(Transaccion transaccion)
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

                        movimientoProductoRepository.InsertOnlyMovimientos(transaccion.MovimientoProducto.ToList(), transaction);

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

        public bool Eliminar(int idTransaccion, int idUsuario, int estado, DateTime fechaRegistro)
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

        #endregion
    }
}
