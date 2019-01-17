
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

    public class OrdenPedidoController : BaseController
    {
        public IList<MovimientoProducto> DetalleOrdenPedido
        {
            get { return (IList<MovimientoProducto>)Session["DetalleOrdenPedido"]; }
            set { Session["DetalleOrdenPedido"] = value; }
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

            return PartialView("OrdenPedidoListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "IdOperacion", "Documento", "RazonSocial", "FechaDocumento", "Estado" };
                var lista = CrearJGrid(OrdenPedidoBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdTransaccion.ToString(),
                    cell = new[]
                                {   item.IdTransaccion.ToString(),
                                    ItemTablaBL.Instancia.Single((int)TipoTabla.TipoComprobante,item.IdTipoDocumento).Valor +" - "+ item.Documento,
                                    item.RazonSocial,
                                    item.FechaDocumento.ToShortDateString(),
                                    item.MontoNeto.ToString(),
                                    item.EstadoOrden
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
            if (DetalleOrdenPedido == null)
            {
                return Json(new JGrid());
            }

            var jqgrid = CrearJgrid(grid, DetalleOrdenPedido.Count);

            jqgrid.rows = DetalleOrdenPedido.Select(item => new JRow
            {
                id = item.IdMovimientoProducto.ToString(),
                cell = new[]
                                {
                                    item.IdMovimientoProducto.ToString(),
                                    item.CodigoProducto,
                                    item.NombreProducto,
                                    item.NombrePresentacion,
                                    item.PrecioBase.ToString(),
                                    item.Cantidad.ToString(),
                                    item.MontoDescuento.ToString(),
                                    item.MontoDescuento2.ToString(),
                                    item.MontoImpuesto.ToString(),
                                    item.SubTotal.ToString()
                                }
            }).ToArray();

            return Json(jqgrid);
        }

        public ActionResult Crear()
        {
            ViewData["Accion"] = "Crear";

            var ordenPedido= new OrdenPedido
            {
                IdOperacion = IdOperacion,
                FechaDocumento = FechaSistema,
                FechaEntrega = FechaSistema.AddDays(30),
                Estado = (int)TipoEstadoDocumento.Pendiente,
            };
            DetalleOrdenPedido = new List<MovimientoProducto>();

            PrepararDatos(ref ordenPedido);
            return PartialView("OrdenPedidoPanel", ordenPedido);
        }

        [HttpPost]
        public JsonResult Crear(OrdenPedido ordenPedido)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    ordenPedido.IdEmpresa = IdEmpresa;
                    ordenPedido.IdSucursal = IdSucursal;
                    ordenPedido.IdOperacion = IdOperacion;
                    ordenPedido.IdEmpleado = UsuarioActual.IdEmpleado;
                    ordenPedido.FechaCreacion = FechaCreacion;
                    ordenPedido.FechaModificacion = FechaModificacion;
                    ordenPedido.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    ordenPedido.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    ordenPedido.MovimientoProducto = new List<MovimientoProducto>();

                    var secuencia = 0;
                    foreach (var movimientoProducto in DetalleOrdenPedido)
                    {
                        movimientoProducto.Secuencia = ++secuencia;
                        movimientoProducto.FechaRegistro = ordenPedido.FechaDocumento;
                        movimientoProducto.IdAlmacen = ordenPedido.IdAlmacen;
                        movimientoProducto.Estado = (int)TipoEstadoTransaccion.Registrado;
                        movimientoProducto.FechaCreacion = FechaCreacion;
                        movimientoProducto.FechaModificacion = FechaModificacion;
                        movimientoProducto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        movimientoProducto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                        var secuenciaDetalle = 0;
                        foreach (var productoStock in movimientoProducto.MovimientoProductoStock)
                        {
                            productoStock.Secuencia = ++secuenciaDetalle;
                            productoStock.IdAlmacen = ordenPedido.IdAlmacen.Value;
                        }

                        if (movimientoProducto.MovimientoProductoStock.Count == 0)
                        {
                            movimientoProducto.MovimientoProductoStock.Add(
                                new MovimientoProductoStock
                                {
                                    Secuencia = 1,
                                    IdAlmacen = ordenPedido.IdAlmacen.Value,
                                    Cantidad = movimientoProducto.Cantidad,
                                    LoteSerie = string.Empty
                                });
                        }

                        ordenPedido.MovimientoProducto.Add(movimientoProducto);
                    }

                    foreach (var impuesto in ordenPedido.TransaccionImpuesto)
                    {
                        impuesto.FechaCreacion = FechaCreacion;
                        impuesto.FechaModificacion = FechaModificacion;
                        impuesto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        impuesto.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    }

                    var impuestoTotal = ordenPedido.TransaccionImpuesto.FirstOrDefault(p => p.IdImpuesto == (int)TipoImpuesto.Total);
                    if (impuestoTotal != null)
                    {
                        ordenPedido.MontoNeto = impuestoTotal.Valor;
                    }
                    ordenPedido.Estado = (int)TipoEstadoDocumento.Pendiente;
                    OrdenPedidoBL.Instancia.Insertar(ordenPedido);

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
                var ordenPedido= OrdenPedidoBL.Instancia.Single(idTransaccion);
                var detalleOrdenPedido = MovimientoProductoBL.Instancia.GetByTransaccion(idTransaccion);
                
                foreach (var movimientoProducto in detalleOrdenPedido)
                {
                    movimientoProducto.MovimientoProductoStock = MovimientoProductoStockBL.Instancia.GetByMovimientoProducto(movimientoProducto.IdMovimientoProducto);
                }

                PrepararDatos(ref ordenPedido);
                DetalleOrdenPedido = new List<MovimientoProducto>(detalleOrdenPedido);

                return PartialView("OrdenPedidoPanel", ordenPedido);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(OrdenPedido ordenPedido)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var ordenPedidoOriginal = OrdenPedidoBL.Instancia.Single(ordenPedido.IdTransaccion);

                    ordenPedidoOriginal.IdPersona = ordenPedido.IdPersona;
                    ordenPedidoOriginal.IdAlmacen = ordenPedido.IdAlmacen;
                    ordenPedidoOriginal.FechaEntrega = ordenPedido.FechaEntrega;
                    ordenPedidoOriginal.Glosa = ordenPedido.Glosa;
                    ordenPedidoOriginal.IdEmpleado = UsuarioActual.IdEmpleado;
                    ordenPedidoOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    ordenPedidoOriginal.MontoTipoCambio = ordenPedido.MontoTipoCambio;
                    ordenPedidoOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    ordenPedidoOriginal.MovimientoProducto = new List<MovimientoProducto>();
                    ordenPedidoOriginal.TransaccionImpuesto = new List<TransaccionImpuesto>();

                    var secuencia = 0;
                    foreach (var movimientoProducto in DetalleOrdenPedido)
                    {
                        movimientoProducto.Secuencia = ++secuencia;
                        movimientoProducto.FechaRegistro = ordenPedido.FechaDocumento;
                        movimientoProducto.IdAlmacen = ordenPedido.IdAlmacen;
                        movimientoProducto.Estado = (int)TipoEstadoTransaccion.Registrado;
                        movimientoProducto.FechaCreacion = FechaCreacion;
                        movimientoProducto.FechaModificacion = FechaModificacion;
                        movimientoProducto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        movimientoProducto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                        var secuenciaDetalle = 0;
                        foreach (var productoStock in movimientoProducto.MovimientoProductoStock)
                        {
                            productoStock.Secuencia = ++secuenciaDetalle;
                            productoStock.IdAlmacen = ordenPedido.IdAlmacen.Value;
                        }

                        if (movimientoProducto.MovimientoProductoStock.Count == 0)
                        {
                            movimientoProducto.MovimientoProductoStock.Add(
                                new MovimientoProductoStock
                                {
                                    Secuencia = 1,
                                    IdAlmacen = ordenPedido.IdAlmacen.Value,
                                    Cantidad = movimientoProducto.Cantidad,
                                    LoteSerie = string.Empty
                                });
                        }

                        ordenPedidoOriginal.MovimientoProducto.Add(movimientoProducto);
                    }

                    foreach (var impuesto in ordenPedido.TransaccionImpuesto)
                    {
                        impuesto.FechaCreacion = FechaCreacion;
                        impuesto.FechaModificacion = FechaModificacion;
                        impuesto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        impuesto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                        ordenPedidoOriginal.TransaccionImpuesto.Add(impuesto);
                    }

                    var impuestoTotal = ordenPedido.TransaccionImpuesto.FirstOrDefault(p => p.IdImpuesto == (int)TipoImpuesto.Total);
                    if (impuestoTotal != null)
                    {
                        ordenPedido.MontoNeto = impuestoTotal.Valor;
                    }

                    OrdenPedidoBL.Instancia.Actualizar(ordenPedidoOriginal);

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

        public ActionResult CrearDetalle()
        {
            ViewData["Accion"] = "CrearDetalle";

            var movimientoProducto = new MovimientoProducto();

            if (DetalleOrdenPedido == null)
            {
                DetalleOrdenPedido = new List<MovimientoProducto>();
            }

            movimientoProducto.IdMovimientoProducto = DetalleOrdenPedido.Count == 0 ? 1 : DetalleOrdenPedido.Max(p => p.IdMovimientoProducto) + 1;
            movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();

            return PartialView("DetalleOrdenPedido", movimientoProducto);
        }

        [HttpPost]
        public JsonResult CrearDetalle(MovimientoProducto movimientoProducto, string lotesJson)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var existePorductoRepetido = false;
                    foreach (var detalle in DetalleOrdenPedido)
                    {
                        if (movimientoProducto.IdProducto == detalle.IdProducto &&
                            movimientoProducto.IdPresentacion == detalle.IdPresentacion)
                            existePorductoRepetido = true;
                    }

                    if (existePorductoRepetido)
                    {
                        jsonResponse.Success = false;
                        jsonResponse.Message = "No puede ingresar un mismo producto en mas de un Item.";
                    }
                    else
                    {
                        var lotes = new JavaScriptSerializer().Deserialize<IList<MovimientoProductoStock>>(lotesJson);
                        var presentacion = PresentacionBL.Instancia.Single(movimientoProducto.IdPresentacion);
                        var producto = ProductoBL.Instancia.Single(movimientoProducto.IdProducto);
                        var operacion = OperacionBL.Instancia.Single(IdOperacion);
                        movimientoProducto.CodigoAlternoProducto = producto.CodigoAlterno;
                        movimientoProducto.Secuencia = DetalleOrdenPedido.Count + 1;
                        movimientoProducto.CodigoProducto = producto.Codigo;
                        movimientoProducto.NombreProducto = producto.Nombre;
                        movimientoProducto.TipoProducto = producto.TipoProducto;
                        movimientoProducto.TipoClasificacion = producto.TipoClasificacion;
                        movimientoProducto.NombrePresentacion = presentacion.Nombre;

                        if (!producto.EsExonerado)
                        {
                            movimientoProducto.MontoImpuesto = (movimientoProducto.PrecioNeto.Redondear(2) * (Igv.Monto / 100)).Redondear(2);
                            movimientoProducto.PorcentajeImpuesto = Igv.Monto / 100;
                        }

                        movimientoProducto.SubTotal = movimientoProducto.PrecioNeto.Redondear(2) + movimientoProducto.MontoImpuesto.Redondear(2);
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
                            movimientoProducto.IdMovimientoProducto = DetalleOrdenPedido.Count == 0 ? 1 : DetalleOrdenPedido.Max(p => p.IdMovimientoProducto) + 1;
                        }

                        DetalleOrdenPedido.Add(movimientoProducto);

                        jsonResponse.Success = true;
                        jsonResponse.Message = "El detalle del producto fue ingresado correctamente.";   
                    }
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
                var movimientoProducto = DetalleOrdenPedido.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                return PartialView("DetalleOrdenPedido", movimientoProducto);
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
                    var movimientoProductoOriginal = DetalleOrdenPedido.SingleOrDefault(p => p.IdMovimientoProducto == movimientoProducto.IdMovimientoProducto);

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
                        movimientoProductoOriginal.MontoDescuento2 = movimientoProducto.MontoDescuento2;
                        movimientoProductoOriginal.PorcentajeDescuento2 = movimientoProducto.PorcentajeDescuento2;
                        movimientoProductoOriginal.PrecioNeto = movimientoProducto.PrecioNeto;

                        if (!producto.EsExonerado)
                        {
                            movimientoProductoOriginal.MontoImpuesto = (movimientoProducto.PrecioNeto.Redondear(2) * (Igv.Monto / 100)).Redondear(2);
                            movimientoProductoOriginal.PorcentajeImpuesto = Igv.Monto / 100;
                        }

                        movimientoProductoOriginal.SubTotal = movimientoProducto.PrecioNeto.Redondear(2) + movimientoProductoOriginal.MontoImpuesto.Redondear(2);
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
                    var movimientoProducto = DetalleOrdenPedido.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                    if (movimientoProducto != null)
                    {
                        DetalleOrdenPedido.Remove(movimientoProducto);
                    }

                    var secuencial = 0;
                    foreach (var detalle in DetalleOrdenPedido)
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
            var detalle = DetalleOrdenPedido.FirstOrDefault(p => p.IdMovimientoProducto == idDetalle);
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

        public JsonResult BuscarOrden(int idProveedor, string desde, string hasta, string documento)
        {
            var ordenes = OrdenPedidoBL.Instancia.BuscarOrden(10, idProveedor, desde, hasta, documento);
            return Json(ordenes, JsonRequestBehavior.AllowGet);
        }

        private void PrepararDatos(ref OrdenPedido ordenPedido)
        {
            var operacionDocumentos = OperacionDocumentoBL.Instancia.GetByOperacion(ordenPedido.IdOperacion);
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

            if (ordenPedido.IdTransaccion == 0)
            {
                var transaccionImpuesto = new List<TransaccionImpuesto>();
                var operacionImpuestos = OperacionImpuestoBL.Instancia.GetByOperacion(ordenPedido.IdOperacion);

                foreach (var operacionImpuesto in operacionImpuestos)
                {
                    transaccionImpuesto.Add(new TransaccionImpuesto
                    {
                        IdImpuesto = operacionImpuesto.IdImpuesto,
                        Secuencia = operacionImpuesto.Orden,
                        Tasa = operacionImpuesto.Impuesto.Monto,
                        NombreImpuesto = operacionImpuesto.Impuesto.Nombre,
                        EsEditable = Convert.ToBoolean(operacionImpuesto.Impuesto.EsEditable)
                    });
                }

                ordenPedido.TransaccionImpuesto = transaccionImpuesto.OrderBy(p => p.Secuencia).ToList();
            }
            else
            {
                ordenPedido.TransaccionImpuesto = TransaccionImpuestoBL.Instancia.GetByTransaccion(ordenPedido.IdTransaccion);
            }

            ordenPedido.Documentos = documentos;
            ordenPedido.Direcciones = direcciones;
            ordenPedido.Monedas = MonedaBL.Instancia.GetAll();
            ordenPedido.CondicionesPago = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.CondicionPago);
            ordenPedido.Almacenes = AlmacenBL.Instancia.GetByIdSucursal(IdSucursal);
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var cargoId = Convert.ToInt32(id);
                    
                    OrdenPedidoBL.Instancia.Delete(cargoId);

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
