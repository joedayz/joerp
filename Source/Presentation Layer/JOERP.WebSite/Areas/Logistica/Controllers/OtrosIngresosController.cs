
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

    public class OtrosIngresosController : BaseController
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
            return PartialView("OtrosIngresosListado");
        }

        public IList<MovimientoProducto> DetalleOtrosIngresos
        {
            get { return (IList<MovimientoProducto>)Session["DetalleOtrosIngresos"]; }
            set { Session["DetalleOtrosIngresos"] = value; }
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "IdOperacion", "Documento", "Almacen", "FechaDocumento", "Estado" };
                var lista = CrearJGrid(IngresoOtroConceptoBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdTransaccion.ToString(),
                    cell = new[]
                                {   item.IdTransaccion.ToString(),
                                    ItemTablaBL.Instancia.Single((int)TipoTabla.TipoComprobante,item.IdTipoDocumento).Valor +" - "+ item.Documento,
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
            if (DetalleOtrosIngresos == null)
            {
                return Json(new JGrid());
            }

            var jqgrid = CrearJgrid(grid, DetalleOtrosIngresos.Count);

            jqgrid.rows = DetalleOtrosIngresos.Select(item => new JRow
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

            var ingreso = new IngresoOtroConceptoDTO
            {
                IdOperacion = IdOperacion,
                FechaDocumento = FechaSistema,
                Estado = (int)TipoEstadoTransaccion.Registrado,
            };
            DetalleOtrosIngresos = new List<MovimientoProducto>();

            PrepararDatos(ref ingreso);
            return PartialView("OtrosIngresosPanel", ingreso);
        }

        [HttpPost]
        public JsonResult Crear(IngresoOtroConceptoDTO ingreso)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    ingreso.IdEmpresa = IdEmpresa;
                    ingreso.IdSucursal = IdSucursal;
                    ingreso.IdOperacion = IdOperacion;
                    ingreso.IdEmpleado = UsuarioActual.IdEmpleado;
                    ingreso.IdPersona = UsuarioActual.IdEmpleado;
                    ingreso.Estado = (int)TipoEstadoTransaccion.Registrado;
                    ingreso.FechaCreacion = FechaCreacion;
                    ingreso.FechaModificacion = FechaModificacion;
                    ingreso.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    ingreso.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    ingreso.MovimientoProducto = new List<MovimientoProducto>();

                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    var serieDocumento = SerieDocumentoBL.Instancia.Single(ingreso.IdSerieDocumento);
                    ingreso.SerieDocumento = serieDocumento.Serie;
                    ingreso.CondicionPago = 0;

                    if (DetalleOtrosIngresos.Count == 0)
                    {
                        jsonResponse.Success = false;
                        jsonResponse.Message = "No se puede registrar sin ingresar los productos.";
                    }
                    else
                    {
                        var secuencia = 0;
                        foreach (var movimientoProducto in DetalleOtrosIngresos)
                        {
                            movimientoProducto.Secuencia = ++secuencia;
                            movimientoProducto.FechaRegistro = ingreso.FechaDocumento;
                            movimientoProducto.IdAlmacen = ingreso.IdAlmacen;
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
                                productoStock.IdAlmacen = ingreso.IdAlmacen.Value;
                            }

                            if (movimientoProducto.MovimientoProductoStock.Count == 0)
                            {
                                movimientoProducto.MovimientoProductoStock.Add(
                                    new MovimientoProductoStock
                                    {
                                        Secuencia = 1,
                                        IdAlmacen = ingreso.IdAlmacen.Value,
                                        Cantidad = movimientoProducto.Cantidad,
                                        LoteSerie = string.Empty
                                    });
                            }

                            ingreso.MovimientoProducto.Add(movimientoProducto);
                        }

                        IngresoOtroConceptoBL.Instancia.Insertar(ingreso);

                        jsonResponse.Success = true;
                        jsonResponse.Message = "El ingreso se registro correctamente.";   
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

        public ActionResult Modificar(string id)
        {
            try
            {
                ViewData["Accion"] = "Modificar";

                var idTransaccion = Convert.ToInt32(id);
                var ingreso = IngresoOtroConceptoBL.Instancia.Single(idTransaccion);
                var detalleOtrosIngresos = MovimientoProductoBL.Instancia.GetByTransaccion(idTransaccion);

                foreach (var movimientoProducto in detalleOtrosIngresos)
                {
                    movimientoProducto.MovimientoProductoStock = MovimientoProductoStockBL.Instancia.GetByMovimientoProducto(movimientoProducto.IdMovimientoProducto);
                }

                PrepararDatos(ref ingreso);
                DetalleOtrosIngresos = new List<MovimientoProducto>(detalleOtrosIngresos);

                return PartialView("OtrosIngresosPanel", ingreso);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(IngresoOtroConceptoDTO ingreso)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var ingresoOriginal = IngresoOtroConceptoBL.Instancia.Single(ingreso.IdTransaccion);
                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    ingresoOriginal.IdPersona = UsuarioActual.IdEmpleado ;
                    ingresoOriginal.IdAlmacen = ingreso.IdAlmacen;
                    ingresoOriginal.FechaEntrega = ingreso.FechaEntrega;
                    ingresoOriginal.Glosa = ingreso.Glosa;
                    ingresoOriginal.MontoTipoCambio = ingreso.MontoTipoCambio;
                    ingresoOriginal.IdEmpleado = UsuarioActual.IdEmpleado;
                    ingresoOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    ingresoOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    ingresoOriginal.MovimientoProducto = new List<MovimientoProducto>();
                    ingresoOriginal.TransaccionImpuesto = new List<TransaccionImpuesto>();

                    if (DetalleOtrosIngresos.Count == 0)
                    {
                        jsonResponse.Success = false;
                        jsonResponse.Message = "No se puede registrar sin ingresar los productos.";
                    }
                    else
                    {
                        var secuencia = 0;
                        foreach (var movimientoProducto in DetalleOtrosIngresos)
                        {
                            movimientoProducto.Secuencia = ++secuencia;
                            movimientoProducto.FechaRegistro = ingreso.FechaDocumento;
                            movimientoProducto.IdAlmacen = ingreso.IdAlmacen;
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
                                productoStock.IdAlmacen = ingreso.IdAlmacen.Value;
                            }

                            if (movimientoProducto.MovimientoProductoStock.Count == 0)
                            {
                                movimientoProducto.MovimientoProductoStock.Add(
                                    new MovimientoProductoStock
                                    {
                                        Secuencia = 1,
                                        IdAlmacen = ingreso.IdAlmacen.Value,
                                        Cantidad = movimientoProducto.Cantidad,
                                        LoteSerie = string.Empty
                                    });
                            }

                            ingresoOriginal.MovimientoProducto.Add(movimientoProducto);
                        }

                        IngresoOtroConceptoBL.Instancia.Actualizar(ingresoOriginal);

                        jsonResponse.Success = true;
                        jsonResponse.Message = "El ingreso se modifico correctamente.";       
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

        public JsonResult ListarLotes(int idDetalle)
        {
            var detalle = DetalleOtrosIngresos.FirstOrDefault(p => p.IdMovimientoProducto == idDetalle);
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

            if (DetalleOtrosIngresos == null)
            {
                DetalleOtrosIngresos = new List<MovimientoProducto>();
            }

            movimientoProducto.IdMovimientoProducto = DetalleOtrosIngresos.Count == 0 ? 1 : DetalleOtrosIngresos.Max(p => p.IdMovimientoProducto) + 1;
            movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();

            return PartialView("DetalleOtrosIngresos", movimientoProducto);
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
                    foreach (var detalle in DetalleOtrosIngresos)
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

                        movimientoProducto.Secuencia = DetalleOtrosIngresos.Count + 1;
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
                            movimientoProducto.IdMovimientoProducto = DetalleOtrosIngresos.Count == 0 ? 1 : DetalleOtrosIngresos.Max(p => p.IdMovimientoProducto) + 1;
                        }

                        DetalleOtrosIngresos.Add(movimientoProducto);

                        jsonResponse.Success = true;
                        jsonResponse.Message = "El ingreso se registro correctamente.";   
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
                var movimientoProducto = DetalleOtrosIngresos.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                return PartialView("DetalleOtrosIngresos", movimientoProducto);
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
                    var movimientoProductoOriginal = DetalleOtrosIngresos.SingleOrDefault(p => p.IdMovimientoProducto == movimientoProducto.IdMovimientoProducto);

                    if (movimientoProductoOriginal != null)
                    {
                        var existePorductoRepetido = false;
                        foreach (var detalle in DetalleOtrosIngresos)
                        {
                            if (movimientoProducto.IdProducto == detalle.IdProducto &&
                                movimientoProducto.IdPresentacion == detalle.IdPresentacion &&
                                movimientoProducto.IdMovimientoProducto != detalle.IdMovimientoProducto)
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

                            jsonResponse.Success = true;
                            jsonResponse.Message = "EL ingreso de modifico correctamente.";
                        }
                    }
                    else
                    {
                        jsonResponse.Success = false;
                        jsonResponse.Message = "No existe el producto dentro de la lista de los ya ingresados.";   
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

        [HttpPost]
        public JsonResult EliminarDetalle(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var idMovimientoProducto = Convert.ToInt32(id);
                    var movimientoProducto = DetalleOtrosIngresos.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                    if (movimientoProducto != null)
                    {
                        DetalleOtrosIngresos.Remove(movimientoProducto);
                    }

                    var secuencial = 0;
                    foreach (var detalle in DetalleOtrosIngresos)
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

        private void PrepararDatos(ref IngresoOtroConceptoDTO ingreso)
        {
            var operacionDocumentos = OperacionDocumentoBL.Instancia.GetByOperacion(ingreso.IdOperacion);
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

            if (ingreso.IdTransaccion == 0)
            {
                var transaccionImpuesto = new List<TransaccionImpuesto>();
                var operacionImpuestos = OperacionImpuestoBL.Instancia.GetByOperacion(ingreso.IdOperacion);

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

                ingreso.TransaccionImpuesto = transaccionImpuesto.OrderBy(p => p.Secuencia).ToList();
            }
            else
            {
                ingreso.TransaccionImpuesto = TransaccionImpuestoBL.Instancia.GetByTransaccion(ingreso.IdTransaccion);
            }

            ingreso.Documentos = documentos;
            ingreso.Direcciones = direcciones;
            ingreso.Monedas = MonedaBL.Instancia.GetAll();
            ingreso.Almacenes = AlmacenBL.Instancia.GetByIdSucursal(IdSucursal);
        }
    }
}
