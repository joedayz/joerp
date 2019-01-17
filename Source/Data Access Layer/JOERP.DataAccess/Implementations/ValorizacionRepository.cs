
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Business.Entity;
    using Business.Entity.DTO;
    using Helpers.Enums;
    using Helpers;
    using Interfaces;

    public class ValorizacionRepository : Repository<Valorizacion>, IValorizacionRepository
    {
        #region Variables

        private readonly ICompraRepository compraRepository = new CompraRepository();
        private readonly IProductoRepository productoRepository = new ProductoRepository();
        private readonly IImpuestoRepository impuestoRepository = new ImpuestoRepository();
        private readonly ITipoCambioRepository tipoCambioRepository = new TipoCambioRepository();
        private readonly IListaPrecioRepository listaPrecioRepository = new ListaPrecioRepository();
        private readonly IPresentacionRepository presentacionRepository = new PresentacionRepository();
        private readonly IProductoStockRepository productoStockRepository = new ProductoStockRepository();
        private readonly IProductoPrecioRepository productoPrecioRepository = new ProductoPrecioRepository();
        private readonly IMovimientoProductoRepository movimientoProductoRepository = new MovimientoProductoRepository();
        private readonly ITransaccionDocumentoRepository transaccionDocumentoRepository  = new TransaccionDocumentoRepository();

        #endregion

        #region Metodos

        public Valorizacion Obtener(int idValorizacion)
        {
            return Single("usp_Single_Transaccion", idValorizacion);
        }

        public Valorizacion Insertar(Valorizacion valorizacion)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        var valorizacionCreada = Add("usp_Insert_Transaccion", valorizacion, transaction);

                        foreach (var documento in valorizacion.DocumentosRelacionados)
                        {
                            documento.IdTransaccion = valorizacionCreada.IdTransaccion;

                            AddGeneric("usp_Insert_TransaccionDocumento", documento, transaction);
                        }

                        valorizacion.IdTransaccion = valorizacion.IdTransaccion;

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            return valorizacion;
        }

        public void Actualizar(Valorizacion valorizacion)   
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        Update("usp_Update_Transaccion", valorizacion, transaction);

                        Delete("usp_Delete_TransaccionDocumento", valorizacion.IdTransaccion);

                        foreach (var documento in valorizacion.DocumentosRelacionados)
                        {
                            documento.IdTransaccion = valorizacion.IdTransaccion;

                            AddGeneric("usp_Insert_TransaccionDocumento", documento, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public void Valorizar(int idValorizacion)
        {
            using (var conection = Database.CreateConnection())
            {
                conection.Open();
                using (var transaction = conection.BeginTransaction())
                {
                    try
                    {
                        var valorizacion = Single("usp_Single_Transaccion", idValorizacion, transaction);
                        var transaccionDocumentos = transaccionDocumentoRepository.GetByTrasaccion(valorizacion.IdTransaccion);
                        var compra = compraRepository.Single(valorizacion.IdTransaccionReferencia.Value, transaction);
                        var detalle = movimientoProductoRepository.GetByTransaccion(compra.IdTransaccion, transaction);
                        var montoDetalles = 0m;

                        foreach (var movimientoProducto in detalle)
                        {
                            movimientoProducto.Cantidad = movimientoProducto.Cantidad.Redondear();
                            movimientoProducto.PrecioBase = movimientoProducto.PrecioBase.Redondear();
                            movimientoProducto.MontoDescuento = movimientoProducto.MontoDescuento.Redondear();

                            var precio = ((movimientoProducto.PrecioBase * movimientoProducto.Cantidad) - movimientoProducto.MontoDescuento).Redondear();
                            var montoDetalle = tipoCambioRepository.GetMontoEnMonedaLocal(compra.IdMoneda.Value, precio, compra.FechaDocumento, compra.IdEmpresa, true);
                            
                            movimientoProducto.PrecioNeto = montoDetalle;
                            movimientoProducto.MontoAgregado = montoDetalle;
                            montoDetalles += montoDetalle;
                        }

                        foreach (var documento in transaccionDocumentos)
                        {
                            var montoDocumento = tipoCambioRepository.GetMontoEnMonedaLocal(documento.IdMoneda, documento.Monto, documento.FechaDocumento, compra.IdEmpresa, true);
                            
                            foreach (var movimientoProducto in detalle)
                            {
                                movimientoProducto.MontoAgregado += (montoDocumento/montoDetalles*movimientoProducto.PrecioNeto).Redondear();
                            }
                        }

                        foreach (var movimientoProducto in detalle)
                        {
                            movimientoProducto.Costo = movimientoProducto.Cantidad == 0 ? 0 : (movimientoProducto.MontoAgregado / movimientoProducto.Cantidad).Redondear();
                        }

                        movimientoProductoRepository.UpdateCostoMovimientos(detalle, transaction);

                        compra.MovimientoProducto = new List<MovimientoProducto>(detalle);

                        Costear(compra, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public void Costear(Transaccion transaccion, DbTransaction transaction)
        {
            try
            {
                var igv = impuestoRepository.Single((int)TipoImpuesto.IGV);
                var listasPrecio = listaPrecioRepository.GetByEmpresa(transaccion.IdEmpresa);

                foreach (var movimientoProducto in transaccion.MovimientoProducto)
                {
                    var producto = productoRepository.Single(movimientoProducto.IdProducto);
                    var presentacionActual = presentacionRepository.Single(movimientoProducto.IdPresentacion);

                    var cantidadCompra = movimientoProducto.Cantidad.Redondear();
                    var costoCompra = movimientoProducto.Costo.Redondear();
                    var equivalencia = presentacionActual.Equivalencia.Redondear();

                    var costoActual = productoPrecioRepository.GetCostoByPresentacion(transaccion.IdEmpresa, transaccion.IdSucursal, presentacionActual.IdPresentacion, transaction);
                    var stockActual = productoStockRepository.GetStockByPresentacion(transaccion.IdSucursal, presentacionActual.IdPresentacion, transaction);

                    if (costoActual <= 0 || stockActual <= 0)
                    {
                        costoActual = costoCompra;
                        stockActual = cantidadCompra;
                    }

                    var costoPonderadoMovimiento = (((stockActual * costoActual) + (costoCompra * cantidadCompra)) / (stockActual + cantidadCompra)).Redondear();
                    var costoPonderado = (costoPonderadoMovimiento / equivalencia).Redondear();

                    var presentaciones = presentacionRepository.GetByIdProducto(movimientoProducto.IdProducto);
                    var precios = productoPrecioRepository.GetPreciosBySucursalAndProducto(transaccion.IdSucursal, movimientoProducto.IdProducto, transaction);

                    foreach (var presentacion in presentaciones)
                    {
                        var costo = (costoPonderado * presentacion.Equivalencia).Redondear();

                        foreach (var listaPrecio in listasPrecio)
                        {
                            costo = tipoCambioRepository.GetMontoEnMonedaLocal(listaPrecio.IdMoneda, costo, transaccion.FechaDocumento, transaccion.IdEmpresa, true);
                            var productoPrecio = precios.FirstOrDefault(p => p.IdListaPrecio == listaPrecio.IdListaPrecio && p.IdPresentacion == presentacion.IdPresentacion);

                            if (productoPrecio == null)
                            {
                                #region Insertar en ProductoPrecio

                                var precioVenta = producto.EsExonerado ? costo : (costo * (1 + (igv.Monto / 100))).Redondear();

                                productoPrecio = new ProductoPrecio
                                                     {
                                                         IdSucursal =  transaccion.IdSucursal,
                                                         IdPresentacion = presentacion.IdPresentacion,
                                                         IdListaPrecio = listaPrecio.IdListaPrecio,
                                                         Costo =  costo,
                                                         PorcentajeGanancia = 0m,
                                                         Ganancia = 0m,
                                                         Valor = costo,
                                                         PrecioVenta = precioVenta,
                                                         UsuarioCreacion = transaccion.UsuarioCreacion
                                                     };

                                productoPrecioRepository.Add(productoPrecio);

                                #endregion
                            }
                            else
                            {
                                if (listaPrecio.AutoActualizar)
                                {
                                    var ganancia = costo * productoPrecio.PorcentajeGanancia.Redondear();
                                    var valor = costo + ganancia;
                                    var precioVenta = producto.EsExonerado ? valor : (valor * (1 + (igv.Monto / 100m))).Redondear();

                                    #region Actualizar Costo, Ganancia, Valor y Precio de Venta
                                    
                                    productoPrecio.Costo = costo;
                                    productoPrecio.PorcentajeGanancia = 0;
                                    productoPrecio.Ganancia = ganancia;
                                    productoPrecio.Valor = valor;
                                    productoPrecio.PrecioVenta = precioVenta;
                                    productoPrecio.UsuarioModificacion = transaccion.UsuarioModificacion;
                                    
                                    productoPrecioRepository.UpdateCostoGananciaValorPrecioVenta(productoPrecio, transaction);

                                    #endregion
                                }
                                else
                                {
                                    #region Actualizar sólo Costo

                                    productoPrecio.Costo = costo;
                                    productoPrecio.UsuarioCreacion = transaccion.UsuarioCreacion;

                                    productoPrecioRepository.UpdateCostoProductoPrecio(productoPrecio, transaction);

                                    #endregion
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrio un error mientras se realizaba el Costeo, detalles: " + ex.Message);
            }
        }

        #endregion
    }
}
