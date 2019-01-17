
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

    public class OrdenCompraController : BaseController
    {
        public IList<MovimientoProducto> DetalleOrdenCompra
        {
            get { return (IList<MovimientoProducto>)Session["DetalleOrdenCompra"]; }
            set { Session["DetalleOrdenCompra"] = value; }
        }

        public ActionResult Index(int id)
        {
            var formulario = FormularioBL.Instancia.Single(id);
            if(formulario != null)
            {
                if (formulario.IdOperacion.HasValue)
                {
                    ViewData["Operacion"] = formulario.IdOperacion;
                    IdOperacion = formulario.IdOperacion.Value;       
                }
            }

            return PartialView("OrdenCompraListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "IdOperacion", "Documento", "RazonSocial", "FechaDocumento", "Estado" };
                var lista = CrearJGrid(OrdenCompraBL.Instancia, grid, nombreFiltros, ref jqgrid);

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

        public JsonResult BuscarOrden(int idProveedor, string desde, string hasta, string documento)
        {
            var ordenes = OrdenCompraBL.Instancia.BuscarOrden(Constantes.OperacionOrdenPedido, idProveedor, desde, hasta, documento);
            return Json(ordenes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListarDetalle(GridTable grid)
        {
            var jqgrid = CrearJgrid(grid, DetalleOrdenCompra.Count);

            jqgrid.rows = DetalleOrdenCompra.Select(item => new JRow
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
                                    item.MontoImpuesto.ToString(),
                                    item.SubTotal.ToString()
                                }
            }).ToArray();

            return Json(jqgrid);
        }

        public ActionResult Crear()
        {
            ViewData["Accion"] = "Crear";
            
            var ordenCompra = new OrdenCompra
                              {
                                  IdOperacion = IdOperacion,
                                  FechaDocumento = FechaSistema,
                                  FechaEntrega = FechaSistema.AddDays(30),
                                  Estado = (int) TipoEstadoDocumento.Pendiente,
                              };
            DetalleOrdenCompra = new List<MovimientoProducto>();
            
            PrepararDatos(ref ordenCompra);
            return PartialView("OrdenCompraPanel", ordenCompra);
        }

        [HttpPost]
        public JsonResult Crear(OrdenCompra ordenCompra)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    ordenCompra.IdEmpresa = IdEmpresa;
                    ordenCompra.IdSucursal = IdSucursal;
                    ordenCompra.IdOperacion = IdOperacion;
                    ordenCompra.IdEmpleado = UsuarioActual.IdEmpleado;
                    ordenCompra.FechaCreacion = FechaCreacion;
                    ordenCompra.FechaModificacion = FechaModificacion;
                    ordenCompra.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    ordenCompra.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    ordenCompra.MovimientoProducto = new List<MovimientoProducto>();

                    var secuencia = 0;
                    foreach (var movimientoProducto in DetalleOrdenCompra)
                    {
                        movimientoProducto.Secuencia = ++secuencia;
                        movimientoProducto.FechaRegistro = ordenCompra.FechaDocumento;
                        movimientoProducto.IdAlmacen = ordenCompra.IdAlmacen;
                        movimientoProducto.Estado = (int)TipoEstadoTransaccion.Registrado;
                        movimientoProducto.FechaCreacion = FechaCreacion;
                        movimientoProducto.FechaModificacion = FechaModificacion;
                        movimientoProducto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        movimientoProducto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                        ordenCompra.MovimientoProducto.Add(movimientoProducto);
                    }

                    foreach (var impuesto in ordenCompra.TransaccionImpuesto)
                    {
                        impuesto.FechaCreacion = FechaCreacion;
                        impuesto.FechaModificacion = FechaModificacion;
                        impuesto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        impuesto.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    }

                    var impuestoTotal = ordenCompra.TransaccionImpuesto.FirstOrDefault(p => p.IdImpuesto == (int)TipoImpuesto.Total);
                    if (impuestoTotal != null)
                    {
                        ordenCompra.MontoNeto = impuestoTotal.Valor;
                    }
                    ordenCompra.Estado = (int) TipoEstadoDocumento.Pendiente;
                    OrdenCompraBL.Instancia.Insertar(ordenCompra);
                    
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
                var ordenCompra = OrdenCompraBL.Instancia.Single(idTransaccion);
                var detalleOrdenCompra = MovimientoProductoBL.Instancia.GetByTransaccion(idTransaccion);

                PrepararDatos(ref ordenCompra);
                DetalleOrdenCompra = new List<MovimientoProducto>(detalleOrdenCompra);

                return PartialView("OrdenCompraPanel", ordenCompra);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(OrdenCompra ordenCompra)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var ordenCompraOriginal = OrdenCompraBL.Instancia.Single(ordenCompra.IdTransaccion);

                    ordenCompraOriginal.IdPersona = ordenCompra.IdPersona;
                    ordenCompraOriginal.IdAlmacen = ordenCompra.IdAlmacen;
                    ordenCompraOriginal.FechaEntrega = ordenCompra.FechaEntrega;
                    ordenCompraOriginal.Glosa = ordenCompra.Glosa;
                    ordenCompraOriginal.IdEmpleado = UsuarioActual.IdEmpleado;
                    ordenCompraOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    ordenCompraOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    ordenCompraOriginal.MovimientoProducto = new List<MovimientoProducto>();
                    ordenCompraOriginal.TransaccionImpuesto = new List<TransaccionImpuesto>();

                    var secuencia = 0;
                    foreach (var movimientoProducto in DetalleOrdenCompra)
                    {
                        movimientoProducto.Secuencia = ++secuencia;
                        movimientoProducto.FechaRegistro = ordenCompra.FechaDocumento;
                        movimientoProducto.IdAlmacen = ordenCompra.IdAlmacen;
                        movimientoProducto.Estado = (int)TipoEstadoTransaccion.Registrado;
                        movimientoProducto.FechaCreacion = FechaCreacion;
                        movimientoProducto.FechaModificacion = FechaModificacion;
                        movimientoProducto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        movimientoProducto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                        ordenCompraOriginal.MovimientoProducto.Add(movimientoProducto);
                    }

                    foreach (var impuesto in ordenCompra.TransaccionImpuesto)
                    {
                        impuesto.FechaCreacion = FechaCreacion;
                        impuesto.FechaModificacion = FechaModificacion;
                        impuesto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        impuesto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                        ordenCompraOriginal.TransaccionImpuesto.Add(impuesto);
                    }

                    var impuestoTotal = ordenCompra.TransaccionImpuesto.FirstOrDefault(p => p.IdImpuesto == (int)TipoImpuesto.Total);
                    if (impuestoTotal != null)
                    {
                        ordenCompraOriginal.MontoNeto = impuestoTotal.Valor;
                    }

                    OrdenCompraBL.Instancia.Actualizar(ordenCompraOriginal);

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

            if (DetalleOrdenCompra == null)
            {
                DetalleOrdenCompra = new List<MovimientoProducto>();
            }

            movimientoProducto.IdMovimientoProducto = DetalleOrdenCompra.Count == 0 ? 1 : DetalleOrdenCompra.Max(p => p.IdMovimientoProducto) + 1;
            movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();

            return PartialView("DetalleOrdenCompra", movimientoProducto);
        }

        [HttpPost]
        public JsonResult CrearDetalle(MovimientoProducto movimientoProducto, int tipoCompra)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var presentacion = PresentacionBL.Instancia.Single(movimientoProducto.IdPresentacion);
                    var producto = ProductoBL.Instancia.Single(movimientoProducto.IdProducto);
                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    movimientoProducto.Secuencia = DetalleOrdenCompra.Count + 1;
                    movimientoProducto.CodigoProducto = producto.Codigo;
                    movimientoProducto.NombreProducto = producto.Nombre;
                    movimientoProducto.TipoProducto = producto.TipoProducto;
                    movimientoProducto.TipoClasificacion = producto.TipoClasificacion;
                    movimientoProducto.NombrePresentacion = presentacion.Nombre;

                    if (tipoCompra == 1)
                    {
                        if (!producto.EsExonerado)
                        {
                            movimientoProducto.MontoImpuesto = movimientoProducto.PrecioNeto * (Igv.Monto / 100);
                            movimientoProducto.PorcentajeImpuesto = Igv.Monto / 100;
                        }
                    }

                    movimientoProducto.SubTotal = movimientoProducto.PrecioNeto + movimientoProducto.MontoImpuesto;
                    movimientoProducto.SignoStock = operacion.SignoStock;
                    movimientoProducto.Peso = presentacion.Peso;

                    if (movimientoProducto.IdMovimientoProducto == 0)
                    {
                        movimientoProducto.IdMovimientoProducto = DetalleOrdenCompra.Count == 0 ? 1 : DetalleOrdenCompra.Max(p => p.IdMovimientoProducto) + 1;
                    }

                    DetalleOrdenCompra.Add(movimientoProducto);

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
                var movimientoProducto = DetalleOrdenCompra.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                return PartialView("DetalleOrdenCompra", movimientoProducto);
            }
            catch(Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ModificarDetalle(MovimientoProducto movimientoProducto, int tipoCompra)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var movimientoProductoOriginal = DetalleOrdenCompra.SingleOrDefault(p => p.IdMovimientoProducto == movimientoProducto.IdMovimientoProducto);

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
                        movimientoProductoOriginal.PrecioNeto = movimientoProducto.PrecioNeto;

                        if (tipoCompra == 1)
                        {
                            if (!producto.EsExonerado)
                            {
                                movimientoProductoOriginal.MontoImpuesto = movimientoProducto.PrecioNeto * (Igv.Monto / 100);
                                movimientoProductoOriginal.PorcentajeImpuesto = Igv.Monto / 100;
                            }
                        }

                        movimientoProductoOriginal.SubTotal = movimientoProducto.PrecioNeto + movimientoProducto.MontoImpuesto;
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

            if (ModelState.IsValid)
            {
                try
                {
                    var idMovimientoProducto = Convert.ToInt32(id);
                    var movimientoProducto = DetalleOrdenCompra.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                    if (movimientoProducto != null)
                    {
                        DetalleOrdenCompra.Remove(movimientoProducto);
                    }

                    var secuencial = 0;
                    foreach (var detalle in DetalleOrdenCompra)
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

        private void PrepararDatos(ref OrdenCompra ordenCompra)
        {
            var operacionDocumentos = OperacionDocumentoBL.Instancia.GetByOperacion(ordenCompra.IdOperacion);
            var tiposDocumentos = ItemTablaBL.Instancia.GetByTabla((int)TipoTabla.TipoComprobante);
            var documentos = new List<Comun>();

            foreach (var documento in operacionDocumentos)
            {
                var tipoDocumento = tiposDocumentos.FirstOrDefault(p => p.IdItemTabla == documento.TipoDocumento);
                documentos.Add(new Comun {Id = documento.TipoDocumento, Nombre = tipoDocumento.Valor});
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

            if (ordenCompra.IdTransaccion == 0)
            {
                var transaccionImpuesto = new List<TransaccionImpuesto>();
                var operacionImpuestos = OperacionImpuestoBL.Instancia.GetByOperacion(ordenCompra.IdOperacion);

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

                ordenCompra.TransaccionImpuesto = transaccionImpuesto.OrderBy(p=>p.Secuencia).ToList();
            }
            else
            {
                ordenCompra.TransaccionImpuesto = TransaccionImpuestoBL.Instancia.GetByTransaccion(ordenCompra.IdTransaccion);
            }
            ordenCompra.TiposCompra = new List<Comun>();
            ordenCompra.TiposCompra.Add(new Comun { Id = 0, Nombre = "--Seleccionar--" });
            ordenCompra.TiposCompra.Add(new Comun { Id = 1, Nombre = "Local" });
            ordenCompra.TiposCompra.Add(new Comun { Id = 2, Nombre = "Extranjera" });

            ordenCompra.Documentos = documentos;
            ordenCompra.Direcciones = direcciones;
            ordenCompra.Monedas = MonedaBL.Instancia.GetAll();
            ordenCompra.CondicionesPago = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.CondicionPago);
            ordenCompra.Almacenes = AlmacenBL.Instancia.GetByIdSucursal(IdSucursal);
        }
    }
}
