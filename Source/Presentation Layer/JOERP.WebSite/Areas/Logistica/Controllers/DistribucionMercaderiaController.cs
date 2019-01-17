
namespace JOERP.WebSite.Areas.Logistica.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Entity.DTO;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.Enums;
    using Helpers.JqGrid;

    public class DistribucionMercaderiaController : BaseController
    {
        public Compra CompraSeleccionada
        {
            get { return (Compra)Session["CompraSeleccionada"]; }
            set { Session["CompraSeleccionada"] = value; }
        }

        public IList<MovimientoProducto> DetalleTransferencia
        {
            get { return (IList<MovimientoProducto>)Session["DetalleDistribucion"]; }
            set { Session["DetalleDistribucion"] = value; }
        }

        public ActionResult Index(int id)
        {
            var formulario = FormularioBL.Instancia.Single(id);
            if (formulario != null)
            {
                if (formulario.IdOperacion.HasValue)
                {
                    ViewData["Operacion"] = formulario.IdOperacion;
                    IdOperacion = formulario.IdOperacion.Value;
                }
            }
            return PartialView("DistribucionMercaderiaListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "IdOperacion", "Documento", "FechaDocumento", "Estado" };
                var lista = CrearJGrid(DistribucionMercaderiaBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdTransaccion.ToString(),
                    cell = new[]
                                {   item.IdTransaccion.ToString(),
                                    item.Documento,
                                    item.NumeroDocumentoRef,
                                    item.FechaDocumento.ToShortDateString(),
                                    item.NombreEstado
                                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }

            return Json(jqgrid);
        }

        public ActionResult Crear()
        {
            ViewData["Accion"] = "Crear";

            var distribucion = new DistribucionMercaderia
            {
                IdOperacion = IdOperacion,
                FechaDocumento = FechaSistema,
                FechaEntrega = FechaSistema.AddDays(30)
            };

            CompraSeleccionada = new Compra();
            DetalleTransferencia = new List<MovimientoProducto>();

            PrepararDatos(ref distribucion);
            return PartialView("DistribucionMercaderiaPanel", distribucion);
        }

        [HttpPost]
        public JsonResult Crear(DistribucionMercaderia distribucionMercaderia)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    distribucionMercaderia.IdEmpresa = IdEmpresa;
                    distribucionMercaderia.IdSucursal = IdSucursal;
                    distribucionMercaderia.IdOperacion = IdOperacion;
                    distribucionMercaderia.IdEmpleado = UsuarioActual.IdEmpleado;
                    distribucionMercaderia.Estado = (int)TipoEstadoTransaccion.Registrado;
                    distribucionMercaderia.FechaCreacion = FechaCreacion;
                    distribucionMercaderia.FechaModificacion = FechaModificacion;
                    distribucionMercaderia.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    distribucionMercaderia.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    distribucionMercaderia.MovimientoProducto = new List<MovimientoProducto>();
                    distribucionMercaderia.TransaccionImpuesto = new List<TransaccionImpuesto>();

                    var compra = CompraBL.Instancia.Single((int) distribucionMercaderia.IdTransaccionReferencia);

                    var serieDocumento = SerieDocumentoBL.Instancia.Single(distribucionMercaderia.IdSerieDocumento);
                    distribucionMercaderia.SerieDocumento = serieDocumento.Serie;
                    distribucionMercaderia.CondicionPago = 0;
                    distribucionMercaderia.IdAlmacen = compra.IdAlmacen;

                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    var secuencia = 0;
                    foreach (var movimientoProducto in DetalleTransferencia)
                    {
                        var idAlmacen = movimientoProducto.IdAlmacen;
                        movimientoProducto.Secuencia = ++secuencia;
                        movimientoProducto.FechaRegistro = distribucionMercaderia.FechaDocumento;
                        movimientoProducto.SignoStock = operacion.SignoStock;
                        movimientoProducto.Estado = (int)TipoEstadoTransaccion.Registrado;
                        movimientoProducto.FechaCreacion = FechaCreacion;
                        movimientoProducto.FechaModificacion = FechaModificacion;
                        movimientoProducto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        movimientoProducto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                        var secuenciaDetalle = 0;
                        foreach (var productoStock in movimientoProducto.MovimientoProductoStock)
                        {
                            productoStock.Secuencia = ++secuenciaDetalle;
                        }

                        if (movimientoProducto.MovimientoProductoStock.Count == 0)
                        {
                            movimientoProducto.MovimientoProductoStock.Add(
                                new MovimientoProductoStock
                                {
                                    Secuencia = 1,
                                    IdAlmacen = (int) idAlmacen,
                                    Cantidad = movimientoProducto.Cantidad,
                                    LoteSerie = string.Empty
                                });
                        }
                        distribucionMercaderia.MovimientoProducto.Add(movimientoProducto);
                    }

                    DistribucionMercaderiaBL.Instancia.Insertar(distribucionMercaderia);

                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se Proceso con exito.";
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
            }
            else
            {
                jsonResponse.Message = "Por favor ingrese todos los campos requeridos";
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CrearDetalle(int idMovimientoProducto, string lote, int idAlmacen, int cantidad)
        {
            var jsonResponse = new JsonResponse();

            try
            {
                foreach (var movimientoProducto in CompraSeleccionada.MovimientoProducto)
                {
                    if (movimientoProducto.IdMovimientoProducto == idMovimientoProducto)
                    {
                        var movimientoProductoStock = movimientoProducto.MovimientoProductoStock.FirstOrDefault(p => p.LoteSerie == lote);
                        if (movimientoProductoStock != null)
                        {
                            movimientoProductoStock.Cantidad -= cantidad;
                            movimientoProducto.Cantidad -= cantidad;

                            var detalle = DetalleTransferencia.FirstOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                            if (detalle == null)
                            {
                                detalle = new MovimientoProducto
                                {
                                    IdMovimientoProducto = movimientoProducto.IdMovimientoProducto,
                                    Secuencia = DetalleTransferencia.Count + 1,
                                    IdProducto = movimientoProducto.IdProducto,
                                    IdPresentacion = movimientoProducto.IdPresentacion,
                                    TipoClasificacion = movimientoProducto.TipoClasificacion,
                                    TipoProducto = movimientoProducto.TipoProducto,
                                    SignoStock = -1 * movimientoProducto.SignoStock,
                                    CodigoProducto = movimientoProducto.CodigoProducto,
                                    NombreProducto = movimientoProducto.NombreProducto,
                                    MovimientoProductoStock = new List<MovimientoProductoStock>(),
                                    IdAlmacen = idAlmacen
                                };
                                DetalleTransferencia.Add(detalle);
                            }
                            detalle.Cantidad += cantidad;

                            var detalleLote = detalle.MovimientoProductoStock.FirstOrDefault(p => p.LoteSerie == lote);

                            if (detalleLote == null)
                            {
                                var almacen = AlmacenBL.Instancia.Single(idAlmacen);
                                detalleLote = new MovimientoProductoStock
                                {
                                    LoteSerie = lote,
                                    Cantidad = cantidad,
                                    IdAlmacen =  idAlmacen,
                                    NombreAlmacen = almacen.Nombre,
                                    FechaVencimiento = movimientoProductoStock.FechaVencimiento,
                                    FechaVencimientoFormato = movimientoProductoStock.FechaVencimientoFormato
                                };
                                detalle.MovimientoProductoStock.Add(detalleLote);
                            }
                            else
                            {
                                if (detalleLote.IdAlmacen == idAlmacen)
                                {
                                    detalleLote.Cantidad += cantidad;
                                }
                                else
                                {
                                    var almacen = AlmacenBL.Instancia.Single(idAlmacen);
                                    detalleLote = new MovimientoProductoStock
                                    {
                                        LoteSerie = lote,
                                        Cantidad = cantidad,
                                        IdAlmacen = idAlmacen,
                                        NombreAlmacen = almacen.Nombre,
                                        FechaVencimiento = movimientoProductoStock.FechaVencimiento,
                                        FechaVencimientoFormato = movimientoProductoStock.FechaVencimientoFormato
                                    };
                                    detalle.MovimientoProductoStock.Add(detalleLote);    
                                }
                            }
                        }
                        break;
                    }
                }

                jsonResponse.Data = DetalleTransferencia;
                jsonResponse.Success = true;
                jsonResponse.Message = "Se Proceso con exito.";
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarDetalle(int idMovimientoProducto, string lote, int idAlmacen)
        {
            var jsonResponse = new JsonResponse();

            try
            {
                var movimientoProducto = DetalleTransferencia.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                if (movimientoProducto != null)
                {
                    var detalleLote =
                        movimientoProducto.MovimientoProductoStock.FirstOrDefault(
                            p => p.LoteSerie == lote && p.IdAlmacen == idAlmacen);

                    if (detalleLote != null)
                    {
                        movimientoProducto.Cantidad -= detalleLote.Cantidad;
                        movimientoProducto.MovimientoProductoStock.Remove(detalleLote);

                        var movimientoProductoCompra =
                        CompraSeleccionada.MovimientoProducto.FirstOrDefault(
                            p => p.IdMovimientoProducto == idMovimientoProducto);

                        if (movimientoProductoCompra != null)
                        {
                            movimientoProductoCompra.Cantidad += detalleLote.Cantidad;

                            var detalleLoteCompra = movimientoProductoCompra.MovimientoProductoStock.FirstOrDefault(p => p.LoteSerie == lote);

                            if (detalleLoteCompra != null)
                            {
                                detalleLoteCompra.Cantidad += detalleLote.Cantidad;
                            }
                        }
                    }

                    if (movimientoProducto.MovimientoProductoStock.Count == 0)
                    {
                        DetalleTransferencia.Remove(movimientoProducto);   
                    }
                }

                var secuencial = 0;
                foreach (var detalle in DetalleTransferencia)
                {
                    detalle.Secuencia = ++secuencial;
                }

                jsonResponse.Data = DetalleTransferencia;
                jsonResponse.Success = true;
                jsonResponse.Message = "Se quito el registro con exito.";
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarCompras(int idProveedor, string desde, string hasta, string documento)
        {
            var compras = CompraBL.Instancia.BuscarCompras(Constantes.OperacionCompra, idProveedor, desde, hasta, documento);
            return Json(compras, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerCompra(int id)
        {
            var idTransaccion = Convert.ToInt32(id);
            var compra = CompraBL.Instancia.Single(idTransaccion);
            var detalleCompra = MovimientoProductoBL.Instancia.GetByTransaccion(idTransaccion);

            foreach (var movimientoProducto in detalleCompra)
            {
                movimientoProducto.MovimientoProductoStock = MovimientoProductoStockBL.Instancia.GetByMovimientoProducto(movimientoProducto.IdMovimientoProducto);
            }

            compra.MovimientoProducto = new List<MovimientoProducto>(detalleCompra);
            CompraSeleccionada = compra;

            return Json(compra, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerCompraSeleccionada()
        {
            return Json(CompraSeleccionada, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerDetalleTransferencia()
        {
            return Json(DetalleTransferencia, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Modificar(int id)
        {
            ViewData["Accion"] = "Modificar";

            var transaccion = DistribucionMercaderiaBL.Instancia.Single(id);

            var compra = CompraBL.Instancia.Single(Convert.ToInt32(transaccion.IdTransaccionReferencia));
            var detalleCompra = MovimientoProductoBL.Instancia.GetByTransaccion(compra.IdTransaccion);

            var detalleTransferencia = MovimientoProductoBL.Instancia.GetByTransaccion(transaccion.IdTransaccion);

            foreach (var movimientoProducto in detalleTransferencia)
            {
                movimientoProducto.MovimientoProductoStock = MovimientoProductoStockBL.Instancia.GetByMovimientoProducto(movimientoProducto.IdMovimientoProducto);
            }

            foreach (var movimientoProducto in detalleCompra)
            {
                movimientoProducto.MovimientoProductoStock = MovimientoProductoStockBL.Instancia.GetByMovimientoProducto(movimientoProducto.IdMovimientoProducto);
                foreach (var movimientoTransferencia in detalleTransferencia)
                {
                    if (movimientoProducto.IdPresentacion == movimientoTransferencia.IdPresentacion)
                    {
                        foreach (var mpsCompra in movimientoProducto.MovimientoProductoStock)
                        {
                            foreach (var mpsTransferencia in movimientoTransferencia.MovimientoProductoStock)
                            {
                                if (mpsCompra.LoteSerie == mpsTransferencia.LoteSerie)
                                {
                                    mpsCompra.Cantidad = mpsCompra.Cantidad - mpsTransferencia.Cantidad;
                                }
                            }
                        }
                    }
                }
            }

            compra.MovimientoProducto = new List<MovimientoProducto>(detalleCompra);
            CompraSeleccionada = compra;

            DetalleTransferencia = new List<MovimientoProducto>(detalleTransferencia);

            PrepararDatos(ref transaccion);
            return PartialView("DistribucionMercaderiaPanel", transaccion);
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            try
            {
                var idTransaccion = Convert.ToInt32(id);
                var distribucion = DistribucionMercaderiaBL.Instancia.Single(idTransaccion);

                if (distribucion.Estado == (int)TipoEstado.Inactivo)
                {
                    jsonResponse.Message = "La Distribución ya se encuentra eliminada.";
                }
                else
                {
                    distribucion.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    DistribucionMercaderiaBL.Instancia.Eliminar(distribucion);

                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se quito el registro con exito.";   
                }
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        private void PrepararDatos(ref DistribucionMercaderia distribucion)
        {
            var operacionDocumentos = OperacionDocumentoBL.Instancia.GetByOperacion(distribucion.IdOperacion);
            var tiposDocumentos = ItemTablaBL.Instancia.GetByTabla((int)TipoTabla.TipoComprobante);
            var documentos = new List<Comun>();

            foreach (var documento in operacionDocumentos)
            {
                var tipoDocumento = tiposDocumentos.FirstOrDefault(p => p.IdItemTabla == documento.TipoDocumento);
                documentos.Add(new Comun { Id = documento.TipoDocumento, Nombre = tipoDocumento.Valor });
            }

            distribucion.Documentos = documentos;
        }
    }
}
