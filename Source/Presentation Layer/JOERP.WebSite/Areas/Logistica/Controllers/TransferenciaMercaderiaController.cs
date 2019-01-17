
namespace JOERP.WebSite.Areas.Logistica.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using Business.Entity;
    using Business.Entity.DTO;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.Enums;
    using Helpers.JqGrid;

    public class TransferenciaMercaderiaController : BaseController
    {
        public IList<MovimientoProducto> DetalleTransferencia
        {
            get { return (IList<MovimientoProducto>)Session["DetalleTransferencia"]; }
            set { Session["DetalleTransferencia"] = value; }
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
            return PartialView("TransferenciaMercaderiaListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "IdOperacion", "Documento", "FechaDocumento","AlmacenOrigen","AlmacenDestino", "Estado" };
                var lista = CrearJGrid(SalidaTransferenciaAlmacenBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdTransaccion.ToString(),
                    cell = new[]
                                {   item.IdTransaccion.ToString(),
                                    item.Documento,
                                    item.FechaDocumento.ToShortDateString(),
                                    AlmacenBL.Instancia.Single(item.IdAlmacen).Nombre,
                                    AlmacenBL.Instancia.Single(item.IdAlmacenAlterno).Nombre,
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

            var transaccion = new Transferencia
            {
                IdOperacion = IdOperacion,
                FechaDocumento = FechaSistema,
                FechaEntrega = FechaSistema.AddDays(30),
                Estado = (int)TipoEstadoDocumento.Pendiente,
                IdAlmacen = 0,IdAlmacenAlterno = 0
            };
            DetalleTransferencia = new List<MovimientoProducto>();

            PrepararDatos(ref transaccion);
            return PartialView("TransferenciaMercaderiaPanel", transaccion);
        }
        
        [HttpPost]
        public JsonResult Crear(Transferencia transferencia)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    transferencia.IdEmpresa = IdEmpresa;
                    transferencia.IdSucursal = IdSucursal;
                    transferencia.IdOperacion = IdOperacion;
                    transferencia.IdEmpleado = UsuarioActual.IdEmpleado;
                    transferencia.Estado = (int)TipoEstadoTransaccion.Registrado;
                    transferencia.FechaCreacion = FechaCreacion;
                    transferencia.FechaModificacion = FechaModificacion;
                    transferencia.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    transferencia.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    transferencia.MovimientoProducto = new List<MovimientoProducto>();
                    transferencia.TransaccionImpuesto = new List<TransaccionImpuesto>();

                    var serieDocumento = SerieDocumentoBL.Instancia.Single(transferencia.IdSerieDocumento);
                    transferencia.SerieDocumento = serieDocumento.Serie;
                    transferencia.CondicionPago = 0;

                    var secuencia = 0;
                    foreach (var movimientoProducto in DetalleTransferencia)
                    {
                        movimientoProducto.Secuencia = ++secuencia;
                        movimientoProducto.FechaRegistro = transferencia.FechaDocumento;
                        movimientoProducto.IdAlmacen = transferencia.IdAlmacen;
                        movimientoProducto.Estado = (int)TipoEstadoTransaccion.Registrado;
                        movimientoProducto.FechaCreacion = FechaCreacion;
                        movimientoProducto.FechaModificacion = FechaModificacion;
                        movimientoProducto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        movimientoProducto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                        var secuenciaDetalle = 0;
                        foreach (var productoStock in movimientoProducto.MovimientoProductoStock)
                        {
                            productoStock.Secuencia = ++secuenciaDetalle;
                            productoStock.IdAlmacen = transferencia.IdAlmacen.Value;
                        }

                        if (movimientoProducto.MovimientoProductoStock.Count == 0)
                        {
                            movimientoProducto.MovimientoProductoStock.Add(
                                new MovimientoProductoStock
                                {
                                    Secuencia = 1,
                                    IdAlmacen = transferencia.IdAlmacen.Value,
                                    Cantidad = movimientoProducto.Cantidad,
                                    LoteSerie = string.Empty
                                });
                        }

                        transferencia.MovimientoProducto.Add(movimientoProducto);
                    }

                    SalidaTransferenciaAlmacenBL.Instancia.Insertar(transferencia);
                   
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
        public JsonResult ListarDetalle(GridTable grid)
        {
            if (DetalleTransferencia == null)
            {
                return Json(new JGrid());
            }

            var jqgrid = CrearJgrid(grid, DetalleTransferencia.Count);

            jqgrid.rows = DetalleTransferencia.Select(item => new JRow
            {
                id = item.IdMovimientoProducto.ToString(),
                cell = new[]
                                {
                                    item.IdMovimientoProducto.ToString(),
                                    item.CodigoProducto,
                                    item.NombreProducto,
                                    item.NombrePresentacion,
                                    item.Cantidad.ToString()
                                }
            }).ToArray();

            return Json(jqgrid);
        }

        public ActionResult CrearDetalle()
        {
            ViewData["Accion"] = "CrearDetalle";

            var movimientoProducto = new MovimientoProducto();

            movimientoProducto.IdMovimientoProducto = DetalleTransferencia.Count == 0 ? 1 : DetalleTransferencia.Max(p => p.IdMovimientoProducto) + 1;
            movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();

            return PartialView("DetalleTransferenciaPanel", movimientoProducto);
        }

        [HttpPost]
        public JsonResult CrearDetalle(MovimientoProducto movimientoProducto, string lotesJson, string idAlmacen)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var lotes = new JavaScriptSerializer().Deserialize<IList<MovimientoProductoStock>>(lotesJson);
                    var presentacion = PresentacionBL.Instancia.Single(movimientoProducto.IdPresentacion);
                    var producto = ProductoBL.Instancia.Single(movimientoProducto.IdProducto);
                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    movimientoProducto.Secuencia = DetalleTransferencia.Count + 1;
                    movimientoProducto.CodigoProducto = producto.Codigo;
                    movimientoProducto.NombreProducto = producto.Nombre;
                    movimientoProducto.TipoProducto = producto.TipoProducto;
                    movimientoProducto.TipoClasificacion = producto.TipoClasificacion;
                    movimientoProducto.NombrePresentacion = presentacion.Nombre;
                    movimientoProducto.SignoStock = operacion.SignoStock;
                    movimientoProducto.Peso = presentacion.Peso;

                    foreach (var movimientoProductoStock in lotes)
                    {
                        foreach (var detalle in DetalleTransferencia)
                        {
                            if (movimientoProducto.IdPresentacion == detalle.IdPresentacion)
                            {
                                foreach (var lote in detalle.MovimientoProductoStock)
                                {
                                    if (movimientoProductoStock.LoteSerie == lote.LoteSerie)
                                    {
                                        throw new Exception(string.Format("El lote {0}, ya ha sido ingresado junto con el detalle Nro. {1}", lote.LoteSerie, detalle.Secuencia));
                                    }
                                }   
                            }
                        }
                    }

                    foreach (var movimientoProductoStock in lotes)
                    {
                        var productoStock = ProductoStockBL.Instancia.Single(movimientoProducto.IdPresentacion,
                                                                             Convert.ToInt32(idAlmacen),
                                                                             movimientoProductoStock.LoteSerie);
                        if (productoStock == null)
                        {
                            throw new Exception(string.Format("El lote {0} no tiene Stock para el Lote de origen.", movimientoProductoStock.LoteSerie));
                        }
                        else
                        {
                            if (movimientoProductoStock.Cantidad > productoStock.StockFisico)
                            {
                                throw new Exception(string.Format("La cantidad ingresada para el lote {0} es mayor al stock actual, el cual es {1}", movimientoProductoStock.LoteSerie, productoStock.StockFisico));
                            }   
                        }
                    }

                    var secuencia = 0;
                    movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();
                    foreach (var movimientoProductoStock in lotes)
                    {
                        movimientoProductoStock.Secuencia = ++secuencia;
                        movimientoProductoStock.FechaVencimiento = Convert.ToDateTime(movimientoProductoStock.FechaVencimientoFormato);
                        movimientoProducto.MovimientoProductoStock.Add(movimientoProductoStock);
                    }

                    if (movimientoProducto.IdMovimientoProducto == 0)
                    {
                        movimientoProducto.IdMovimientoProducto = DetalleTransferencia.Count == 0 ? 1 : DetalleTransferencia.Max(p => p.IdMovimientoProducto) + 1;
                    }

                    DetalleTransferencia.Add(movimientoProducto);

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

        public ActionResult Modificar(string id)
        {
            try
            {
                ViewData["Accion"] = "Modificar";

                var idTransaccion = Convert.ToInt32(id);
                var transferencia =  SalidaTransferenciaAlmacenBL.Instancia.Single(idTransaccion);
                var detalleTransferencia = MovimientoProductoBL.Instancia.GetByTransaccion(idTransaccion);

                foreach (var movimientoProducto in detalleTransferencia)
                {
                    movimientoProducto.MovimientoProductoStock = MovimientoProductoStockBL.Instancia.GetByMovimientoProducto(movimientoProducto.IdMovimientoProducto);
                }

                PrepararDatos(ref transferencia);
                DetalleTransferencia = new List<MovimientoProducto>(detalleTransferencia);

                return PartialView("TransferenciaMercaderiaPanel", transferencia);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        public ActionResult ModificarDetalle(string id)
        {
            try
            {
                ViewData["Accion"] = "ModificarDetalle";

                var idMovimientoProducto = Convert.ToInt32(id);
                var movimientoProducto = DetalleTransferencia.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                return PartialView("DetalleTransferenciaPanel", movimientoProducto);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ModificarDetalle(MovimientoProducto movimientoProducto, string lotesJson, string idAlmacen)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var movimientoProductoOriginal = DetalleTransferencia.SingleOrDefault(p => p.IdMovimientoProducto == movimientoProducto.IdMovimientoProducto);

                    if (movimientoProductoOriginal != null)
                    {
                        var lotes = new JavaScriptSerializer().Deserialize<IList<MovimientoProductoStock>>(lotesJson);
                        var presentacion = PresentacionBL.Instancia.Single(movimientoProducto.IdPresentacion);
                        var producto = ProductoBL.Instancia.Single(movimientoProducto.IdProducto);
                        var operacion = OperacionBL.Instancia.Single(IdOperacion);

                        movimientoProductoOriginal.CodigoProducto = producto.Codigo;
                        movimientoProductoOriginal.NombreProducto = producto.Nombre;
                        movimientoProductoOriginal.TipoProducto = producto.TipoProducto;
                        movimientoProductoOriginal.TipoClasificacion = producto.TipoClasificacion;
                        movimientoProductoOriginal.NombrePresentacion = presentacion.Nombre;
                        movimientoProductoOriginal.SignoStock = operacion.SignoStock;
                        movimientoProductoOriginal.Peso = presentacion.Peso;
                        movimientoProductoOriginal.Cantidad = movimientoProducto.Cantidad;
                        movimientoProductoOriginal.CantidadDocumento = movimientoProducto.CantidadDocumento;

                        var secuencia = 0;
                        movimientoProductoOriginal.MovimientoProductoStock = new List<MovimientoProductoStock>();
                        foreach (var movimientoProductoStock in lotes)
                        {
                            movimientoProductoStock.Secuencia = ++secuencia;
                            movimientoProductoStock.FechaVencimiento = Convert.ToDateTime(movimientoProductoStock.FechaVencimientoFormato);
                            movimientoProductoOriginal.MovimientoProductoStock.Add(movimientoProductoStock);
                        }
                    }

                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se Proceso con exito";
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
        public JsonResult EliminarDetalle(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var idMovimientoProducto = Convert.ToInt32(id);
                    var movimientoProducto = DetalleTransferencia.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                    if (movimientoProducto != null)
                    {
                        DetalleTransferencia.Remove(movimientoProducto);
                    }

                    var secuencial = 0;
                    foreach (var detalle in DetalleTransferencia)
                    {
                        detalle.Secuencia = ++secuencial;
                    }

                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se quito el registro con exito.";
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

        public JsonResult ListarLotes(int idDetalle)
        {
            var detalle = DetalleTransferencia.FirstOrDefault(p => p.IdMovimientoProducto == idDetalle);
            if (detalle != null)
            {
                foreach (var productoStock in detalle.MovimientoProductoStock)
                {
                    if (productoStock.FechaVencimiento.HasValue)
                    {
                        productoStock.FechaVencimientoFormato = productoStock.FechaVencimiento.Value.ToString("dd/MM/yyyy");
                    }
                }
                return Json(detalle.MovimientoProductoStock, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public void PrepararDatos(ref Transferencia transaccion)
        {
            var operacionDocumentos = OperacionDocumentoBL.Instancia.GetByOperacion(transaccion.IdOperacion);
            var tiposDocumentos = ItemTablaBL.Instancia.GetByTabla((int)TipoTabla.TipoComprobante);
            var documentos = new List<Comun>();

            foreach (var documento in operacionDocumentos)
            {
                var tipoDocumento = tiposDocumentos.FirstOrDefault(p => p.IdItemTabla == documento.TipoDocumento);
                documentos.Add(new Comun { Id = documento.TipoDocumento, Nombre = tipoDocumento.Valor });
            }

            transaccion.Documentos = documentos;
            transaccion.SucursalesAlt =  SucursalBL.Instancia.GetAutorizadas(IdEmpresa, UsuarioActual.IdEmpleado);
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var idTransaccion = Convert.ToInt32(id);
                    SalidaTransferenciaAlmacenBL.Instancia.Eliminar(idTransaccion, UsuarioActual.IdEmpleado,
                                                                    (int) TipoEstadoTransaccion.Anulado,
                                                                    DateTime.Now);
                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se quito el registro con exito.";
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
            }
            else
            {
                jsonResponse.Message = "No se pudo eliminar.";
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }
    }
}
