
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using Business.Entity;
    using Business.Entity.DTO;
    using Helpers;
    using Helpers.Enums;
    using Interfaces;

    public class CompraRepository : Repository<Compra>, ICompraRepository
    {
        #region Variables

        private readonly ITransaccionRepository transaccionRepository = new TransaccionRepository();
        private readonly IProductoStockRepository productoStockRepository = new ProductoStockRepository();
        private readonly IProductoKardexRepository productoKardexRepository = new ProductoKardexRepository();
        private readonly ISerieDocumentoRepository serieDocumentoRepository = new SerieDocumentoRepository();
        private readonly IProductoProveedorRepository productoProveedorRepository = new ProductoProveedorRepository();
        private readonly IMovimientoProductoRepository movimientoProductoRepository = new MovimientoProductoRepository();
        private readonly ITransaccionImpuestoRepository transaccionImpuestoRepository = new TransaccionImpuestoRepository();

        #endregion

        #region Metodos

        public bool Insertar(Transaccion compra)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Transaccion

                        compra.IdTransaccion = transaccionRepository.InsertTransaccion(compra, transaction);

                        #endregion

                        #region Movimiento Producto

                        var  movimientos = compra.MovimientoProducto.ToList();

                        foreach (var movimientoProducto in movimientos)
                        {
                            movimientoProducto.IdTransaccion = compra.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientos(movimientos, transaction);
                        
                        #endregion

                        #region Impuestos

                        foreach (var transaccionImpuesto in compra.TransaccionImpuesto)
                        {
                            transaccionImpuesto.IdTransaccion = compra.IdTransaccion;
                        }

                        transaccionImpuestoRepository.InsertImpuestos(compra.TransaccionImpuesto, transaction);

                        #endregion

                        #region Actualizar Serie Documento

                        serieDocumentoRepository.UpdateSerieDocumento(compra.IdSerieDocumento, compra.IdSucursal, compra.UsuarioModificacion, transaction);

                        #endregion

                        #region Actualizar Orden de Compra

                        if (compra.IdTransaccionReferencia != null)
                        {
                            compra.Estado = (int) TipoEstadoDocumento.Aprobado;
                            transaccionRepository.UpdateReferenciaTransaccion(compra, transaction);
                        }

                        #endregion

                        #region Producto Proveedor

                        foreach (var movimientoProducto in movimientos)
                        {
                            var productoProducto = new ProductoProveedor
                                                       {
                                                           IdPersona = compra.IdPersona.Value,
                                                           IdPresentacion = movimientoProducto.IdPresentacion,
                                                           Precio = movimientoProducto.PrecioBase
                                                       };
                            productoProveedorRepository.Add(productoProducto, transaction);
                        }

                        #endregion

                        #region Registrar Kardex

                        productoKardexRepository.Insertar(compra, transaction);

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

        public bool Actualizar(Transaccion compra)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Obtener Movimiento de Productos

                        var movimientosPorModificar = movimientoProductoRepository.GetByTransaccion(compra.IdTransaccion, transaction);

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

                        movimientoProductoRepository.DeleteByTransaccion(compra.IdTransaccion, transaction);

                        #endregion

                        #region Eliminar Impuesto

                        transaccionImpuestoRepository.DeleteByTransaccion(compra.IdTransaccion, transaction);

                        #endregion

                        #region Actualizar Transaccion

                        transaccionRepository.UpdateTransaccion(compra, transaction);

                        #endregion

                        #region Movimiento Producto

                        var movimientos = compra.MovimientoProducto.ToList();

                        foreach (var movimientoProducto in movimientos)
                        {
                            movimientoProducto.IdTransaccion = compra.IdTransaccion;
                        }

                        movimientoProductoRepository.InsertMovimientos(movimientos, transaction);

                        #endregion

                        #region Impuestos

                        foreach (var transaccionImpuesto in compra.TransaccionImpuesto)
                        {
                            transaccionImpuesto.IdTransaccion = compra.IdTransaccion;
                        }

                        transaccionImpuestoRepository.InsertImpuestos(compra.TransaccionImpuesto.ToList(), transaction);

                        #endregion

                        #region Actualizar Orden de Compra

                        if (compra.IdTransaccionReferencia != null)
                        {
                            compra.Estado = (int)TipoEstadoDocumento.Atendido;
                            transaccionRepository.UpdateReferenciaTransaccion(compra, transaction);
                        }

                        #endregion

                        #region Producto Proveedor

                        foreach (var movimientoProducto in movimientos)
                        {
                            var productoProducto = new ProductoProveedor
                            {
                                IdPersona = compra.IdPersona.Value,
                                IdPresentacion = movimientoProducto.IdPresentacion,
                                Precio = movimientoProducto.PrecioBase
                            };
                            productoProveedorRepository.Add(productoProducto, transaction);
                        }

                        #endregion

                        #region Modificar Kardex

                        productoKardexRepository.Actualizar(compra, transaction);

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

        public bool Eliminar(Transaccion compra)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        #region Obtener Movimiento de Productos

                        var movimientos = movimientoProductoRepository.GetByTransaccion(compra.IdTransaccion, transaction);

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

                        transaccionRepository.DeleteTransaccion(compra.IdTransaccion, (int)TipoEstado.Inactivo, compra.UsuarioModificacion, transaction);

                        #endregion

                        #region Actualizar Orden de Compra

                        if (compra.IdTransaccionReferencia != null)
                        {
                            compra.Estado = (int)TipoEstadoDocumento.Pendiente;
                            transaccionRepository.UpdateReferenciaTransaccion(compra, transaction);
                        }

                        #endregion

                        #region Eliminar Kardex

                        productoKardexRepository.Eliminar(compra.IdTransaccion, transaction);

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

        public List<SerieDocumento> GetSeriesOrdenCompraAprobados(int idEmpresa, int idSucursal, int tipoDocumento, int estado)
        {
            var serieDocumentos = new List<SerieDocumento>();

            var command = Database.GetStoredProcCommand("SelectSerieDocumentoOrdenCompra");
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

        public List<Comun> GetNumerosOrdenCompraAprobados(int idEmpresa, int idSucursal, int idSerie, int estado)
        {
            var serieDocumentos = new List<Comun>();

            var command = Database.GetStoredProcCommand("SelectNumerosOrdenCompraBySerie");
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

        public IList<Comun> GetNumerosDocumentosByIdSerieDocumento(int idOperacion, int idSerieDocumento)
        {
            return GetGeneric<Comun>("usp_Select_NumerosDocumentos_ByIdSerieDocumento", idOperacion, idSerieDocumento);
        }

        public Transaccion GetDatosDocumento(int idTransaccion)
        {
            return SingleGeneric<Transaccion>("usp_Select_DatosDocumento_ByTransaccion", idTransaccion);
        }

        public Compra Single(int idTransaccion, DbTransaction transaction = null)
        {
            return Single("usp_Single_Compra", idTransaccion, transaction);
        }

        public IList<Transaccion> GetAllByEmpresaSucursal(int idEmpresa, int idSucursal)
        {
            return GetGeneric<Transaccion>("usp_Compras_GetByEmpresaSucursal", idEmpresa, idSucursal);
        }

        public IList<Compra> BuscarCompras(int idOperacion, int idProveedor, string desde, string hasta, string documento)
        {
            return Get("usp_Buscar_Compra", idOperacion, idProveedor, desde, hasta, documento);
        }        

        #endregion
    }
}
