
namespace JOERP.DataAccess.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Business.Entity;
    using Business.Entity.DTO;
    using Interfaces;

    public class CargasInventarioRepository : Repository<Transaccion>, ICargasInventarioRepository
    {
        #region Variables

        private readonly ITransaccionRepository transaccionRepository = new TransaccionRepository();
        private readonly ISerieDocumentoRepository serieDocumentoRepository = new SerieDocumentoRepository();
        private readonly ITipoCambioRepository tipoCambioRepository = new TipoCambioRepository();
        private readonly IListaPrecioRepository listaPrecioRepository = new ListaPrecioRepository();
        private readonly IProductoStockRepository productoStockRepository = new ProductoStockRepository();
        private readonly IProductoPrecioRepository productoPrecioRepository = new ProductoPrecioRepository();
        private readonly IProductoRepository productoRepository = new ProductoRepository();
        private readonly IImpuestoRepository impuestoRepository = new ImpuestoRepository();
        
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

                        #region Actualizar Serie Documento

                        serieDocumentoRepository.UpdateSerieDocumento(transaccion.IdSerieDocumento, transaccion.IdSucursal, transaccion.UsuarioModificacion, transaction);

                        #endregion

                        #region Actualizar Stock

                        var comandoUpdateInventario = Database.GetStoredProcCommand("UpdateStockProductos");
                        Database.AddInParameter(comandoUpdateInventario, "pIdProductoStock", DbType.Int32);
                        Database.AddInParameter(comandoUpdateInventario, "pStock", DbType.Decimal);
                        Database.AddInParameter(comandoUpdateInventario, "pUsuarioModificacion", DbType.Int32);

                        foreach (Inventario item in transaccion.Inventarios)
                        {
                            Database.SetParameterValue(comandoUpdateInventario, "pIdProductoStock", item.IdProductoStock);
                            Database.SetParameterValue(comandoUpdateInventario, "pStock", item.StockSistema);
                            Database.SetParameterValue(comandoUpdateInventario, "pUsuarioModificacion", transaccion.UsuarioCreacion);
                            Database.ExecuteNonQuery(comandoUpdateInventario, transaction);
                        }

                        #endregion

                        #region Actualziar Costo

                        //var igv = impuestoRepository.Single(p => p.IdImpuesto == (int)TipoImpuesto.IGV);

                        foreach (var item in transaccion.Inventarios)
                        {
                            //var producto = productoRepository.Single(p => p.Codigo == item.Codigo);
                            //var productoStock = productoStockRepository.Single(p => p.IdProductoStock == item.IdProductoStock);
                            //var productoPrecios = productoPrecioRepository.GetFiltered(p => p.IdSucursal == transaccion.IdSucursal && p.IdPresentacion == productoStock.IdPresentacion);

                            //foreach (var precio in productoPrecios)
                            //{
                            //    var listaPrecio = listaPrecioRepository.Single(p => p.IdListaPrecio == precio.IdListaPrecio);
                            //    var costo = tipoCambioRepository.GetMontoEnMonedaLocal(listaPrecio.IdMoneda, item.CostoReal, transaccion.FechaDocumento.Date, transaccion.IdEmpresa, true);

                            //    if (listaPrecio.AutoActualizar)
                            //    {
                            //        var ganancia = (costo * precio.PorcentajeGanancia).Redondear();
                            //        var valor = costo + ganancia;
                            //        var precioVenta = producto.EsExonerado ? valor : (valor * (1 + (igv.Monto / 100m))).Redondear();

                            //        #region Actualizar Costo, Ganancia, Valor y Precio de Venta

                            //        var comandoProductoPrecio = Database.GetStoredProcCommand("UpdateCostoGananciaValorPrecioVenta");
                            //        Database.AddInParameter(comandoProductoPrecio, "pIdProductoPrecio", DbType.Int32, precio.IdProductoPrecio);
                            //        Database.AddInParameter(comandoProductoPrecio, "pCosto", DbType.Decimal, costo);
                            //        Database.AddInParameter(comandoProductoPrecio, "pPorcentajeGanancia", DbType.Decimal, 0);
                            //        Database.AddInParameter(comandoProductoPrecio, "pGanancia", DbType.Decimal, ganancia);
                            //        Database.AddInParameter(comandoProductoPrecio, "pValor", DbType.Decimal, valor);
                            //        Database.AddInParameter(comandoProductoPrecio, "pPrecioVenta", DbType.Decimal, precioVenta);
                            //        Database.AddInParameter(comandoProductoPrecio, "pUsuarioModificacion", DbType.Int32, transaccion.UsuarioModificacion);
                            //        Database.ExecuteNonQuery(comandoProductoPrecio, transaction);

                            //        #endregion
                            //    }
                            //    else
                            //    {
                            //        #region Actualizar sólo Costo

                            //        var comandoProductoPrecio = Database.GetStoredProcCommand("UpdateCostoProductoPrecio");
                            //        Database.AddInParameter(comandoProductoPrecio, "pIdProductoPrecio", DbType.Int32, precio.IdProductoPrecio);
                            //        Database.AddInParameter(comandoProductoPrecio, "pCosto", DbType.Decimal, costo);
                            //        Database.AddInParameter(comandoProductoPrecio, "pUsuarioModificacion", DbType.Int32, transaccion.UsuarioCreacion);
                            //        Database.ExecuteNonQuery(comandoProductoPrecio, transaction);

                            //        #endregion
                            //    }
                            //}
                        }

                        #endregion

                        transaction.Commit();
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            return true;
        }

        public List<CargasInventarioDTO> Buscar(int idAlmacen, DateTime? fechaInicio, DateTime? fechaFin, int idOperacion, int start, int rows, out int count)
        {
            //DataContext.Transaccion.MergeOption = MergeOption.NoTracking;
            //DataContext.TransaccionDocumento.MergeOption = MergeOption.NoTracking;

            //var query = DataContext.Transaccion
            //            .Where(p => p.IdOperacion == idOperacion &&
            //            (idAlmacen == 0 ? true : p.IdAlmacen == idAlmacen) &&
            //            (fechaInicio.HasValue ? p.FechaDocumento >= fechaInicio : true) &&
            //            (fechaFin.HasValue ? p.FechaDocumento <= fechaFin : true))
            //            .OrderByDescending(p => p.FechaDocumento);

            //count = query.Count();

            //var list = query.Skip(start)
            //                .Take(rows)
            //                .Select(p => new CargasInventarioDTO
            //                {
            //                    IdTransaccion = p.IdTransaccion,
            //                    Documento = p.SerieDocumento + "-" + p.NumeroDocumento, 
            //                    Almacen = p.Almacen.Nombre,
            //                    IdEstado = p.Estado,
            //                    Fecha = p.FechaDocumento
            //                })
            //                .ToList();

            //foreach (var item in list)
            //{
            //    item.Estado = Enum.GetName(typeof(TipoEstadoTransaccion), item.IdEstado);
            //}
            //return list;
            count = 0;
            return null;
        }

        #endregion
    }
}
