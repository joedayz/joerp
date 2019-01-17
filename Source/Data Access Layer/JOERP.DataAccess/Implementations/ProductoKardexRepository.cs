
namespace JOERP.DataAccess.Implementations
{
    using System.Linq;
    using System.Data.Common;
    using Business.Entity;
    using Interfaces;
    using System.Collections.Generic;

    public class ProductoKardexRepository : Repository<ProductoKardex>, IProductoKardexRepository
    {
        #region Variables

        private readonly ICompraRepository compraRepository = new CompraRepository();
        private readonly ISaldoProductoRepository saldoProductoRepository = new SaldoProductoRepository();
        private readonly IMovimientoProductoRepository movimientoProductoRepository = new MovimientoProductoRepository();

        #endregion

        #region Metodos

        public void Insertar(Transaccion transaccion, DbTransaction transaction = null)
        {
            switch (transaccion.IdTransaccion)
            {
                case 3:     // Compras 
                case 5:     // Otros Ingresos
                    {
                        foreach (var movimientoProducto in transaccion.MovimientoProducto)
                        {
                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                var productoKardex = new ProductoKardex
                                {
                                    Periodo = transaccion.FechaDocumento.ToString("yyyyMM"),
                                    IdProducto = movimientoProducto.IdProducto,
                                    IdPresentacion = movimientoProducto.IdPresentacion,
                                    IdAlmacen = movimientoProductoStock.IdAlmacen,
                                    LoteSerie = movimientoProductoStock.LoteSerie,
                                    FechaVencimiento = movimientoProductoStock.FechaVencimiento.HasValue
                                            ? movimientoProductoStock.FechaVencimiento.Value.ToString("yyyyMMdd")
                                            : string.Empty,
                                    Cantidad = movimientoProductoStock.Cantidad,
                                    Saldo = movimientoProductoStock.Cantidad,
                                    Costo = 0,
                                    Signo = +1,
                                    IdIngreso = transaccion.IdTransaccion,
                                    IdTransaccion = transaccion.IdTransaccion
                                };

                                Add("usp_Insert_ProductoKardex", productoKardex, transaction);
                            }
                        }
                    }
                    break;
                case 13:    // Distribución
                    {
                        var movimientosCompra = movimientoProductoRepository.GetByTransaccion(transaccion.IdTransaccionReferencia.Value);

                        foreach (var movimientoProducto in transaccion.MovimientoProducto)
                        {
                            var movimientoCompra = movimientosCompra.FirstOrDefault(
                                    p => p.IdProducto == movimientoProducto.IdProducto &&
                                    p.IdPresentacion == movimientoProducto.IdPresentacion &&
                                    p.IdAlmacen == transaccion.IdAlmacen);

                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                var productoKardex = new ProductoKardex
                                {
                                    Periodo = transaccion.FechaDocumento.ToString("yyyyMM"),
                                    IdProducto = movimientoProducto.IdProducto,
                                    IdPresentacion = movimientoProducto.IdPresentacion,
                                    IdAlmacen = transaccion.IdAlmacen.Value,
                                    LoteSerie = movimientoProductoStock.LoteSerie,
                                    FechaVencimiento = movimientoProductoStock.FechaVencimiento.HasValue
                                            ? movimientoProductoStock.FechaVencimiento.Value.ToString("yyyyMMdd")
                                            : string.Empty,
                                    Cantidad = movimientoProductoStock.Cantidad,
                                    Saldo = movimientoProductoStock.Cantidad,
                                    Costo = movimientoCompra.Costo,
                                    Signo = -1,
                                    IdIngreso = transaccion.IdTransaccionReferencia.Value,
                                    IdTransaccion = transaccion.IdTransaccion
                                };

                                Add("usp_Insert_ProductoKardex", productoKardex, transaction);
                            }
                        }

                        foreach (var movimientoProducto in transaccion.MovimientoProducto)
                        {
                            var movimientoCompra = movimientosCompra.FirstOrDefault(
                                    p => p.IdProducto == movimientoProducto.IdProducto &&
                                    p.IdPresentacion == movimientoProducto.IdPresentacion &&
                                    p.IdAlmacen == transaccion.IdAlmacen);

                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                var productoKardex = new ProductoKardex
                                {
                                    Periodo = transaccion.FechaDocumento.ToString("yyyyMM"),
                                    IdProducto = movimientoProducto.IdProducto,
                                    IdPresentacion = movimientoProducto.IdPresentacion,
                                    IdAlmacen = movimientoProductoStock.IdAlmacen,
                                    LoteSerie = movimientoProductoStock.LoteSerie,
                                    FechaVencimiento = movimientoProductoStock.FechaVencimiento.HasValue
                                            ? movimientoProductoStock.FechaVencimiento.Value.ToString("yyyyMMdd")
                                            : string.Empty,
                                    Cantidad = movimientoProductoStock.Cantidad,
                                    Saldo = movimientoProductoStock.Cantidad,
                                    Costo = movimientoCompra.Costo,
                                    Signo = +1,
                                    IdIngreso = transaccion.IdTransaccionReferencia.Value,
                                    IdTransaccion = transaccion.IdTransaccion
                                };

                                Add("usp_Insert_ProductoKardex", productoKardex, transaction);
                            }
                        }
                    }
                    break;
                case 2:     // Transferencias
                    {
                        var movimientosProducto = new List<ProductoKardex>();

                        foreach (var movimientoProducto in transaccion.MovimientoProducto)
                        {
                            foreach (var movimientoProductoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                var periodo = transaccion.FechaDocumento.ToString("yyyyMM");
                                var saldoProducto = saldoProductoRepository.GetDisponible(transaccion.IdEmpresa,
                                                                                          transaccion.IdSucursal,
                                                                                          transaccion.IdAlmacen.Value,
                                                                                          movimientoProducto.IdProducto,
                                                                                          movimientoProducto.IdPresentacion,
                                                                                          movimientoProductoStock.LoteSerie, 
                                                                                          periodo,
                                                                                          transaction);
                                if (saldoProducto == null || saldoProducto.Stock == 0)
                                {
                                    
                                }
                                else
                                {
                                    if (saldoProducto.Stock < movimientoProductoStock.Cantidad)
                                    {
                                        var productoKardex = new ProductoKardex
                                        {
                                            Periodo = periodo,
                                            IdProducto = movimientoProducto.IdProducto,
                                            IdPresentacion = movimientoProducto.IdPresentacion,
                                            IdAlmacen = transaccion.IdAlmacen.Value,
                                            LoteSerie = movimientoProductoStock.LoteSerie,
                                            FechaVencimiento = movimientoProductoStock.FechaVencimiento.HasValue
                                                    ? movimientoProductoStock.FechaVencimiento.Value.ToString("yyyyMMdd")
                                                    : string.Empty,
                                            Cantidad = movimientoProductoStock.Cantidad - saldoProducto.Stock,
                                            Saldo = movimientoProductoStock.Cantidad - saldoProducto.Stock,
                                            Costo = saldoProducto.Costo,
                                            Signo = -1,
                                            IdIngreso = transaccion.IdTransaccionReferencia.Value,
                                            IdTransaccion = transaccion.IdTransaccion
                                        };

                                        movimientosProducto.Add(productoKardex);
                                    }
                                    else
                                    {
                                        var productoKardex = new ProductoKardex
                                        {
                                            Periodo = periodo,
                                            IdProducto = movimientoProducto.IdProducto,
                                            IdPresentacion = movimientoProducto.IdPresentacion,
                                            IdAlmacen = transaccion.IdAlmacen.Value,
                                            LoteSerie = movimientoProductoStock.LoteSerie,
                                            FechaVencimiento = movimientoProductoStock.FechaVencimiento.HasValue
                                                    ? movimientoProductoStock.FechaVencimiento.Value.ToString("yyyyMMdd")
                                                    : string.Empty,
                                            Cantidad = saldoProducto.Stock - movimientoProductoStock.Cantidad,
                                            Saldo = saldoProducto.Stock - movimientoProductoStock.Cantidad,
                                            Costo = saldoProducto.Costo,
                                            Signo = -1,
                                            IdIngreso = transaccion.IdTransaccionReferencia.Value,
                                            IdTransaccion = transaccion.IdTransaccion
                                        };

                                        movimientosProducto.Add(productoKardex);   
                                    }
                                }
                            }
                        }

                        foreach (var productoKardex in movimientosProducto)
                        {
                            Add("usp_Insert_ProductoKardex", productoKardex, transaction);

                            productoKardex.Signo = +1;
                            productoKardex.IdAlmacen = transaccion.IdAlmacenAlterno.Value;

                            Add("usp_Insert_ProductoKardex", productoKardex, transaction);
                        }
                    }
                    break;
                case 12:    // Ventas
                case 6:     // Otras Salidas
                    {

                    }
                    break;
            }
        }

        public void Actualizar(Transaccion transaccion, DbTransaction transaction = null)
        {
            Eliminar(transaccion.IdTransaccion, transaction);
            Insertar(transaccion, transaction);
        }

        public void Eliminar(int idTransaccion, DbTransaction transaction = null)
        {
            Delete("usp_Delete_ProductoKardex_ByIdTransaccion", idTransaccion, transaction);
        }

        #endregion
    }
}
