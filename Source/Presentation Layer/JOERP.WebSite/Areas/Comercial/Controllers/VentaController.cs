
namespace JOERP.WebSite.Areas.Comercial.Controllers
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

    public class VentaController : BaseController
    {
        public IList<MovimientoProducto> DetalleVenta
        {
            get { return (IList<MovimientoProducto>)Session["DetalleVenta"]; }
            set { Session["DetalleVenta"] = value; }
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

            return PartialView("VentaListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "IdOperacion", "Documento", "RazonSocial", "FechaDocumento", "Estado" };
                var lista = CrearJGrid(VentaBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdTransaccion.ToString(),
                    cell = new[]
                                {   item.IdTransaccion.ToString(),
                                    ItemTablaBL.Instancia.Single((int)TipoTabla.TipoComprobante,item.IdTipoDocumento).Valor +" - "+ item.Documento,
                                    item.RazonSocial,
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
            if (DetalleVenta == null) DetalleVenta = new List<MovimientoProducto>();

            var jqgrid = CrearJgrid(grid, DetalleVenta.Count);

            jqgrid.rows = DetalleVenta.Select(item => new JRow
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

            var venta = new Venta
            {
                IdOperacion = IdOperacion,
                FechaDocumento = FechaSistema,
                Estado = (int)TipoEstadoTransaccion.Registrado,
            };
            DetalleVenta = new List<MovimientoProducto>();

            PrepararDatos(ref venta);
            return PartialView("VentaPanel", venta);
        }

        [HttpPost]
        public JsonResult Crear(Venta venta)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    venta.IdEmpresa = IdEmpresa;
                    venta.IdSucursal = IdSucursal;
                    venta.IdOperacion = IdOperacion;
                    venta.IdEmpleado = UsuarioActual.IdEmpleado;
                    venta.Estado = (int)TipoEstadoTransaccion.Registrado;
                    venta.FechaCreacion = FechaCreacion;
                    venta.FechaModificacion = FechaModificacion;
                    venta.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    venta.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    venta.MovimientoProducto = new List<MovimientoProducto>();

                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    var serieDocumento = SerieDocumentoBL.Instancia.Single(venta.IdSerieDocumento);
                    venta.SerieDocumento = serieDocumento.Serie;
                    venta.CondicionPago = 0;

                    var secuencia = 0;
                    foreach (var movimientoProducto in DetalleVenta)
                    {
                        movimientoProducto.Secuencia = ++secuencia;
                        movimientoProducto.FechaRegistro = venta.FechaDocumento;
                        movimientoProducto.IdAlmacen = venta.IdAlmacen;
                        movimientoProducto.Estado = (int)TipoEstadoTransaccion.Registrado;
                        movimientoProducto.SignoStock = operacion.SignoStock;
                        movimientoProducto.FechaCreacion = FechaCreacion;
                        movimientoProducto.FechaModificacion = FechaModificacion;
                        movimientoProducto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        movimientoProducto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                        var secuenciaDetalle = 0;
                        foreach (var productoStock in movimientoProducto.MovimientoProductoStock)
                        {
                            productoStock.Secuencia = ++secuenciaDetalle;
                            productoStock.IdAlmacen = venta.IdAlmacen.Value;
                        }

                        venta.MovimientoProducto.Add(movimientoProducto);
                    }

                    foreach (var impuesto in venta.TransaccionImpuesto)
                    {
                        impuesto.FechaCreacion = FechaCreacion;
                        impuesto.FechaModificacion = FechaModificacion;
                        impuesto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        impuesto.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    }

                    var impuestoTotal = venta.TransaccionImpuesto.FirstOrDefault(p => p.IdImpuesto == (int)TipoImpuesto.Total);
                    if (impuestoTotal != null)
                    {
                        venta.MontoNeto = impuestoTotal.Valor;
                    }

                    VentaBL.Instancia.Insertar(venta);

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
                var venta = VentaBL.Instancia.Single(idTransaccion);
                var detalleVenta = MovimientoProductoBL.Instancia.GetByTransaccion(idTransaccion);

                foreach (var movimientoProducto in detalleVenta)
                {
                    movimientoProducto.MovimientoProductoStock = MovimientoProductoStockBL.Instancia.GetByMovimientoProducto(movimientoProducto.IdMovimientoProducto);
                }

                PrepararDatos(ref venta);
                DetalleVenta = new List<MovimientoProducto>(detalleVenta);

                return PartialView("VentaPanel", venta);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Venta venta)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var ventaOriginal = VentaBL.Instancia.Single(venta.IdTransaccion);
                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    ventaOriginal.IdPersona = venta.IdPersona;
                    ventaOriginal.IdAlmacen = venta.IdAlmacen;
                    ventaOriginal.FechaEntrega = venta.FechaEntrega;
                    ventaOriginal.Glosa = venta.Glosa;
                    ventaOriginal.MontoTipoCambio = venta.MontoTipoCambio;
                    ventaOriginal.IdEmpleado = UsuarioActual.IdEmpleado;
                    ventaOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    ventaOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    ventaOriginal.MovimientoProducto = new List<MovimientoProducto>();
                    ventaOriginal.TransaccionImpuesto = new List<TransaccionImpuesto>();
                    ventaOriginal.TipoVenta = venta.TipoVenta;
                    var secuencia = 0;
                    foreach (var movimientoProducto in DetalleVenta)
                    {
                        movimientoProducto.Secuencia = ++secuencia;
                        movimientoProducto.FechaRegistro = venta.FechaDocumento;
                        movimientoProducto.IdAlmacen = venta.IdAlmacen;
                        movimientoProducto.SignoStock = operacion.SignoStock;
                        movimientoProducto.Estado = (int)TipoEstadoTransaccion.Registrado;
                        movimientoProducto.FechaCreacion = FechaCreacion;
                        movimientoProducto.FechaModificacion = FechaModificacion;
                        movimientoProducto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        movimientoProducto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                        if (ventaOriginal.IdTransaccionReferencia.HasValue)
                        {
                            var secuenciaDetalle = 0;
                            foreach (var productoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                productoStock.Secuencia = ++secuenciaDetalle;
                                productoStock.IdAlmacen = venta.IdAlmacen.Value;
                            }
                        }
                        else
                        {
                            movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();
                        }

                        ventaOriginal.MovimientoProducto.Add(movimientoProducto);
                    }

                    foreach (var impuesto in venta.TransaccionImpuesto)
                    {
                        impuesto.FechaCreacion = FechaCreacion;
                        impuesto.FechaModificacion = FechaModificacion;
                        impuesto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        impuesto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                        ventaOriginal.TransaccionImpuesto.Add(impuesto);
                    }

                    var impuestoTotal = venta.TransaccionImpuesto.FirstOrDefault(p => p.IdImpuesto == (int)TipoImpuesto.Total);
                    if (impuestoTotal != null)
                    {
                        ventaOriginal.MontoNeto = impuestoTotal.Valor;
                    }

                    VentaBL.Instancia.Actualizar(ventaOriginal);

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

            if (DetalleVenta == null) DetalleVenta = new List<MovimientoProducto>();

            movimientoProducto.IdMovimientoProducto = DetalleVenta.Count == 0 ? 1 : DetalleVenta.Max(p => p.IdMovimientoProducto) + 1;
            movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();

            return PartialView("DetalleVentaPanel", movimientoProducto);
        }

        [HttpPost]
        public JsonResult CrearDetalle(MovimientoProducto movimientoProducto)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var existePorductoRepetido = false;
                    foreach (var detalle in DetalleVenta)
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
                        //var productoStock  = ProductoStockBL.Instancia.Single(movimientoProducto.IdPresentacion, movimientoProducto.IdAlmacen, )

                        var presentacion = PresentacionBL.Instancia.Single(movimientoProducto.IdPresentacion);
                        var producto = ProductoBL.Instancia.Single(movimientoProducto.IdProducto);
                        var operacion = OperacionBL.Instancia.Single(IdOperacion);

                        movimientoProducto.Secuencia = DetalleVenta.Count + 1;
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

                        if (movimientoProducto.IdMovimientoProducto == 0)
                        {
                            movimientoProducto.IdMovimientoProducto = DetalleVenta.Count == 0 ? 1 : DetalleVenta.Max(p => p.IdMovimientoProducto) + 1;
                        }

                        DetalleVenta.Add(movimientoProducto);

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
                var movimientoProducto = DetalleVenta.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                return PartialView("DetalleVentaPanel", movimientoProducto);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ModificarDetalle(MovimientoProducto movimientoProducto)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var movimientoProductoOriginal = DetalleVenta.SingleOrDefault(p => p.IdMovimientoProducto == movimientoProducto.IdMovimientoProducto);

                    if (movimientoProductoOriginal != null)
                    {
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

            try
            {
                var idMovimientoProducto = Convert.ToInt32(id);
                var movimientoProducto = DetalleVenta.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                if (movimientoProducto != null)
                {
                    DetalleVenta.Remove(movimientoProducto);
                }

                var secuencial = 0;
                foreach (var detalle in DetalleVenta
                    )
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
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarLotes(int idDetalle)
        {
            var detalle = DetalleVenta.FirstOrDefault(p => p.IdMovimientoProducto == idDetalle);
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

        public ActionResult CargarDesdeOrden(int id)
        {
            try
            {
                ViewData["Accion"] = "Crear";

                var idTransaccion = Convert.ToInt32(id);
                var compra = VentaBL.Instancia.Single(idTransaccion);
                compra.DocumentoRelacionado = compra.SerieDocumento + compra.NumeroDocumento;
                compra.IdTransaccionReferencia = idTransaccion;
                compra.IdOperacion = IdOperacion;
                compra.IdSerieDocumento = 0;
                compra.NumeroDocumento = "";
                compra.IdTipoDocumento = 0;
                compra.IdTransaccion = 0;

                var detalleCompra = MovimientoProductoBL.Instancia.GetByTransaccion(idTransaccion);

                foreach (var movimientoProducto in detalleCompra)
                {
                    movimientoProducto.MovimientoProductoStock = MovimientoProductoStockBL.Instancia.GetByMovimientoProducto(movimientoProducto.IdMovimientoProducto);
                }

                PrepararDatos(ref compra);
                DetalleVenta= new List<MovimientoProducto>(detalleCompra);

                return PartialView("VentaPanel", compra);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            try
            {
                var idTransaccion = Convert.ToInt32(id);
                var venta = VentaBL.Instancia.Single(idTransaccion);

                if (venta.Estado == (int)TipoEstado.Inactivo)
                {
                    jsonResponse.Message = "La Compra ya se encuentra eliminada.";
                }
                else
                {
                    venta.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    VentaBL.Instancia.Eliminar(venta);

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

        private void PrepararDatos(ref Venta venta)
        {
            var operacionDocumentos = OperacionDocumentoBL.Instancia.GetByOperacion(venta.IdOperacion);
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

            if (venta.IdTransaccion == 0)
            {
                var transaccionImpuesto = new List<TransaccionImpuesto>();
                var operacionImpuestos = OperacionImpuestoBL.Instancia.GetByOperacion(venta.IdOperacion);

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

                venta.TransaccionImpuesto = transaccionImpuesto.OrderBy(p => p.Secuencia).ToList();
            }
            else
            {
                venta.TransaccionImpuesto = TransaccionImpuestoBL.Instancia.GetByTransaccion(venta.IdTransaccion);
            }

            venta.Documentos = documentos;
            venta.Direcciones = direcciones;
            venta.Monedas = MonedaBL.Instancia.GetAll();
            venta.Almacenes = AlmacenBL.Instancia.GetByIdSucursal(IdSucursal);

            var tiposVenta = ItemTablaBL.Instancia.GetByTabla((int)TipoTabla.TipoVenta);
            venta.TiposVenta = new List<Comun>();
            venta.TiposVenta.Add(new Comun { Id = 0 , Nombre = "- Seleccionar -"});

            foreach (var item in tiposVenta)
            {
                venta.TiposVenta.Add(new Comun{ Id = item.IdItemTabla, Nombre = item.Nombre } );
            }
        }

    }
}
