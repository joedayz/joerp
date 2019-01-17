
namespace JOERP.WebSite.Areas.Comercial.Controllers
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

    public class DevolucionMercaderiaController : BaseController
    {
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
            return PartialView("DevolucionMercaderiaListado");
        }

        public IList<MovimientoProducto> DetalleDevolucion
        {
            get { return (IList<MovimientoProducto>)Session["DetalleDevolucion"]; }
            set { Session["DetalleDevolucion"] = value; }
        }

        [HttpPost]
        public virtual JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "IdOperacion", "Documento", "Almacen", "FechaDocumento", "Estado" };
                var lista = CrearJGrid(DevolucionMercaderiaBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdTransaccion.ToString(),
                    cell = new[]
                                {   item.IdTransaccion.ToString(),
                                    item.Documento,
                                    item.NombreAlmacen,
                                    item.FechaDocumento.ToShortDateString(),
                                    item.MontoNeto.ToString(),
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

        [HttpPost]
        public JsonResult ListarDetalle(GridTable grid)
        {
            if (DetalleDevolucion == null)
            {
                return Json(new JGrid());
            }

            var jqgrid = CrearJgrid(grid, DetalleDevolucion.Count);

            jqgrid.rows = DetalleDevolucion.Select(item => new JRow
            {
                id = item.IdMovimientoProducto.ToString(),
                cell = new[]
                                {
                                    item.IdMovimientoProducto.ToString(),
                                    item.CodigoProducto,
                                    item.NombreProducto,
                                    item.NombrePresentacion,
                                    item.Cantidad.ToString(),
                                }
            }).ToArray();

            return Json(jqgrid);
        }

        public ActionResult Crear()
        {
            ViewData["Accion"] = "Crear";

            var devolucion = new DevolucionMercaderia()
            {
                IdOperacion = IdOperacion,
                FechaDocumento = FechaSistema,
                Estado = (int)TipoEstadoTransaccion.Registrado,
            };
            DetalleDevolucion = new List<MovimientoProducto>();

            PrepararDatos(ref devolucion);
            return PartialView("DevolucionMercaderiaPanel", devolucion);
        }

        [HttpPost]
        public JsonResult Crear(DevolucionMercaderia devolucion)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    devolucion.IdEmpresa = IdEmpresa;
                    devolucion.IdSucursal = IdSucursal;
                    devolucion.IdOperacion = IdOperacion;
                    devolucion.IdEmpleado = UsuarioActual.IdEmpleado;
                    devolucion.IdPersona = UsuarioActual.IdEmpleado;
                    devolucion.Estado = (int)TipoEstadoTransaccion.Registrado;
                    devolucion.FechaCreacion = FechaCreacion;
                    devolucion.FechaModificacion = FechaModificacion;
                    devolucion.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    devolucion.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    devolucion.MovimientoProducto = new List<MovimientoProducto>();

                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    var serieDocumento = SerieDocumentoBL.Instancia.Single(devolucion.IdSerieDocumento);
                    devolucion.SerieDocumento = serieDocumento.Serie;
                    devolucion.CondicionPago = 0;

                    var secuencia = 0;
                    foreach (var movimientoProducto in DetalleDevolucion)
                    {
                        movimientoProducto.Secuencia = ++secuencia;
                        movimientoProducto.FechaRegistro = devolucion.FechaDocumento;
                        movimientoProducto.IdAlmacen = devolucion.IdAlmacen;
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
                            productoStock.IdAlmacen = devolucion.IdAlmacen.Value;
                        }

                        if (movimientoProducto.MovimientoProductoStock.Count == 0)
                        {
                            movimientoProducto.MovimientoProductoStock.Add(
                                new MovimientoProductoStock
                                {
                                    Secuencia = 1,
                                    IdAlmacen = devolucion.IdAlmacen.Value,
                                    Cantidad = movimientoProducto.Cantidad,
                                    LoteSerie = string.Empty
                                });
                        }

                        devolucion.MovimientoProducto.Add(movimientoProducto);
                    }

                    DevolucionMercaderiaBL.Instancia.Insertar(devolucion);

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
                var devolucion = DevolucionMercaderiaBL.Instancia.Single(idTransaccion);
                var detalleDevolucion = MovimientoProductoBL.Instancia.GetByTransaccion(idTransaccion);

                foreach (var movimientoProducto in detalleDevolucion)
                {
                    movimientoProducto.MovimientoProductoStock = MovimientoProductoStockBL.Instancia.GetByMovimientoProducto(movimientoProducto.IdMovimientoProducto);
                }

                PrepararDatos(ref devolucion);
                DetalleDevolucion = new List<MovimientoProducto>(detalleDevolucion);

                return PartialView("DevolucionMercaderiaPanel", devolucion);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(DevolucionMercaderia devolucion)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var devolucionOriginal = DevolucionMercaderiaBL.Instancia.Single(devolucion.IdTransaccion);
                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    devolucionOriginal.IdPersona = UsuarioActual.IdEmpleado;
                    devolucionOriginal.IdAlmacen = devolucion.IdAlmacen;
                    devolucionOriginal.FechaEntrega = devolucion.FechaEntrega;
                    devolucionOriginal.Glosa = devolucion.Glosa;
                    devolucionOriginal.MontoTipoCambio = devolucion.MontoTipoCambio;
                    devolucionOriginal.IdEmpleado = UsuarioActual.IdEmpleado;
                    devolucionOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    devolucionOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    devolucionOriginal.MovimientoProducto = new List<MovimientoProducto>();
                    devolucionOriginal.TransaccionImpuesto = new List<TransaccionImpuesto>();

                    var secuencia = 0;
                    foreach (var movimientoProducto in DetalleDevolucion)
                    {
                        movimientoProducto.Secuencia = ++secuencia;
                        movimientoProducto.FechaRegistro = devolucion.FechaDocumento;
                        movimientoProducto.IdAlmacen = devolucion.IdAlmacen;
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
                            productoStock.IdAlmacen = devolucion.IdAlmacen.Value;
                        }

                        if (movimientoProducto.MovimientoProductoStock.Count == 0)
                        {
                            movimientoProducto.MovimientoProductoStock.Add(
                                new MovimientoProductoStock
                                {
                                    Secuencia = 1,
                                    IdAlmacen = devolucion.IdAlmacen.Value,
                                    Cantidad = movimientoProducto.Cantidad,
                                    LoteSerie = string.Empty
                                });
                        }

                        devolucionOriginal.MovimientoProducto.Add(movimientoProducto);
                    }

                    DevolucionMercaderiaBL.Instancia.Actualizar(devolucionOriginal);

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

        public JsonResult ListarLotes(int idDetalle)
        {
            var detalle = DetalleDevolucion.FirstOrDefault(p => p.IdMovimientoProducto == idDetalle);
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

        public ActionResult CrearDetalle()
        {
            ViewData["Accion"] = "CrearDetalle";

            var movimientoProducto = new MovimientoProducto();

            if (DetalleDevolucion == null)
            {
                DetalleDevolucion = new List<MovimientoProducto>();
            }

            movimientoProducto.IdMovimientoProducto = DetalleDevolucion.Count == 0 ? 1 : DetalleDevolucion.Max(p => p.IdMovimientoProducto) + 1;
            movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();

            return PartialView("DetalleDevolucionMercaderia", movimientoProducto);
        }

        [HttpPost]
        public JsonResult CrearDetalle(MovimientoProducto movimientoProducto, string lotesJson)
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

                    movimientoProducto.Secuencia = DetalleDevolucion.Count + 1;
                    movimientoProducto.CodigoProducto = producto.Codigo;
                    movimientoProducto.NombreProducto = producto.Nombre;
                    movimientoProducto.TipoProducto = producto.TipoProducto;
                    movimientoProducto.TipoClasificacion = producto.TipoClasificacion;
                    movimientoProducto.NombrePresentacion = presentacion.Nombre;


                    if (!producto.EsExonerado)
                    {
                        movimientoProducto.MontoImpuesto = movimientoProducto.PrecioNeto * (Igv.Monto / 100);
                        movimientoProducto.PorcentajeImpuesto = Igv.Monto / 100;
                    }

                    movimientoProducto.SubTotal = movimientoProducto.PrecioNeto + movimientoProducto.MontoImpuesto;
                    movimientoProducto.SignoStock = operacion.SignoStock;
                    movimientoProducto.Peso = presentacion.Peso;
                    movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();

                    foreach (var movimientoProductoStock in lotes)
                    {
                        movimientoProductoStock.FechaVencimiento = Convert.ToDateTime(movimientoProductoStock.FechaVencimientoFormato);
                        movimientoProducto.MovimientoProductoStock.Add(movimientoProductoStock);
                    }

                    if (movimientoProducto.IdMovimientoProducto == 0)
                    {
                        movimientoProducto.IdMovimientoProducto = DetalleDevolucion.Count == 0 ? 1 : DetalleDevolucion.Max(p => p.IdMovimientoProducto) + 1;
                    }

                    DetalleDevolucion.Add(movimientoProducto);

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

        public ActionResult ModificarDetalle(string id)
        {
            try
            {
                ViewData["Accion"] = "ModificarDetalle";

                var idMovimientoProducto = Convert.ToInt32(id);
                var movimientoProducto = DetalleDevolucion.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                return PartialView("DetalleDevolucionMercaderia", movimientoProducto);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ModificarDetalle(MovimientoProducto movimientoProducto, string lotesJson)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var movimientoProductoOriginal = DetalleDevolucion.SingleOrDefault(p => p.IdMovimientoProducto == movimientoProducto.IdMovimientoProducto);

                    if (movimientoProductoOriginal != null)
                    {
                        var lotes = new JavaScriptSerializer().Deserialize<IList<MovimientoProductoStock>>(lotesJson);
                        var presentacion = PresentacionBL.Instancia.Single(movimientoProducto.IdPresentacion);
                        var producto = ProductoBL.Instancia.Single(movimientoProducto.IdProducto);
                        var operacion = OperacionBL.Instancia.Single(IdOperacion);

                        movimientoProductoOriginal.IdProducto = movimientoProducto.IdProducto;
                        movimientoProductoOriginal.IdPresentacion = movimientoProducto.IdPresentacion;
                        movimientoProductoOriginal.CodigoProducto = producto.Codigo;
                        movimientoProductoOriginal.NombreProducto = producto.Nombre;
                        movimientoProductoOriginal.TipoProducto = producto.TipoProducto;
                        movimientoProductoOriginal.TipoClasificacion = producto.TipoClasificacion;
                        movimientoProductoOriginal.NombrePresentacion = presentacion.Nombre;
                        movimientoProductoOriginal.PrecioBase = movimientoProducto.PrecioBase;
                        movimientoProductoOriginal.Cantidad = movimientoProducto.Cantidad;
                        movimientoProductoOriginal.MontoDescuento = movimientoProducto.MontoDescuento;
                        movimientoProductoOriginal.PorcentajeDescuento = movimientoProducto.PorcentajeDescuento;
                        movimientoProductoOriginal.PrecioNeto = movimientoProducto.PrecioNeto;
                        movimientoProductoOriginal.CantidadDocumento = movimientoProducto.CantidadDocumento;

                        if (!producto.EsExonerado)
                        {
                            movimientoProductoOriginal.MontoImpuesto = movimientoProducto.PrecioNeto * (Igv.Monto / 100);
                            movimientoProductoOriginal.PorcentajeImpuesto = Igv.Monto / 100;
                        }

                        movimientoProductoOriginal.SubTotal = movimientoProducto.PrecioNeto + movimientoProducto.MontoImpuesto;
                        movimientoProductoOriginal.SignoStock = operacion.SignoStock;
                        movimientoProductoOriginal.Peso = presentacion.Peso;
                        movimientoProductoOriginal.MovimientoProductoStock = new List<MovimientoProductoStock>();

                        foreach (var movimientoProductoStock in lotes)
                        {
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
                    var movimientoProducto = DetalleDevolucion.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                    if (movimientoProducto != null)
                    {
                        DetalleDevolucion.Remove(movimientoProducto);
                    }

                    var secuencial = 0;
                    foreach (var detalle in DetalleDevolucion)
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

        private void PrepararDatos(ref DevolucionMercaderia devolucion)
        {
            var operacionDocumentos = OperacionDocumentoBL.Instancia.GetByOperacion(devolucion.IdOperacion);
            var tiposDocumentos = ItemTablaBL.Instancia.GetByTabla((int)TipoTabla.TipoComprobante);
            var documentos = new List<Comun>();

            foreach (var documento in operacionDocumentos)
            {
                var tipoDocumento = tiposDocumentos.FirstOrDefault(p => p.IdItemTabla == documento.TipoDocumento);
                documentos.Add(new Comun { Id = documento.TipoDocumento, Nombre = tipoDocumento.Valor });
            }


            var sucursales = SucursalBL.Instancia.GetAutorizadas(IdEmpresa, UsuarioActual.IdEmpleado);
            var direcciones = new List<Comun>();
            foreach (var sucursal in sucursales)
            {
                var ubigeo = UbigeoBL.Instancia.Single(sucursal.IdUbigeo);
                direcciones.Add(new Comun
                {
                    Id = sucursal.IdSucursal,
                    Nombre = string.Format("{0} - {1}, {2}", sucursal.Nombre, sucursal.Direccion, ubigeo.Direccion)
                });
            }

            if (devolucion.IdTransaccion == 0)
            {
                var transaccionImpuesto = new List<TransaccionImpuesto>();
                var operacionImpuestos = OperacionImpuestoBL.Instancia.GetByOperacion(devolucion.IdOperacion);

                foreach (var operacionImpuesto in operacionImpuestos)
                {
                    transaccionImpuesto.Add(new TransaccionImpuesto
                    {
                        IdImpuesto = operacionImpuesto.IdImpuesto,
                        Secuencia = operacionImpuesto.Orden,
                        Tasa = operacionImpuesto.Impuesto.Monto,
                        NombreImpuesto = operacionImpuesto.Impuesto.Nombre,
                        Signo = operacionImpuesto.Impuesto.Signo,
                        EsEditable = Convert.ToBoolean(operacionImpuesto.Impuesto.EsEditable)
                    });
                }

                devolucion.TransaccionImpuesto = transaccionImpuesto.OrderBy(p => p.Secuencia).ToList();
            }
            else
            {
                devolucion.TransaccionImpuesto = TransaccionImpuestoBL.Instancia.GetByTransaccion(devolucion.IdTransaccion);
            }

            devolucion.Documentos = documentos;
            devolucion.Direcciones = direcciones;
            devolucion.Monedas = MonedaBL.Instancia.GetAll();
            devolucion.Almacenes = AlmacenBL.Instancia.GetByIdSucursal(IdSucursal);
        }
    }
}
