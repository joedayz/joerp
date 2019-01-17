
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

    public class OtrasSalidasController : BaseController
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
            return PartialView("OtrasSalidasListado");
        }

        public IList<MovimientoProducto> DetalleOtrasSalidas
        {
            get { return (IList<MovimientoProducto>)Session["DetalleOtrasSalidas"]; }
            set { Session["DetalleOtrasSalidas"] = value; }
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "IdOperacion", "Documento", "Almacen", "FechaDocumento", "Estado" };
                var lista = CrearJGrid(SalidaOtroConceptoBL.Instancia, grid, nombreFiltros, ref jqgrid);

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
            if (DetalleOtrasSalidas == null)
            {
                return Json(new JGrid());
            }

            var jqgrid = CrearJgrid(grid, DetalleOtrasSalidas.Count);

            jqgrid.rows = DetalleOtrasSalidas.Select(item => new JRow
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

            var salida = new SalidaOtroConceptoDTO
            {
                IdOperacion = IdOperacion,
                FechaDocumento = FechaSistema,
                Estado = (int)TipoEstadoTransaccion.Registrado,
            };
            DetalleOtrasSalidas = new List<MovimientoProducto>();

            PrepararDatos(ref salida);
            return PartialView("OtrasSalidasPanel", salida);
        }

        [HttpPost]
        public JsonResult Crear(SalidaOtroConceptoDTO salida)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    salida.IdEmpresa = IdEmpresa;
                    salida.IdSucursal = IdSucursal;
                    salida.IdOperacion = IdOperacion;
                    salida.IdEmpleado = UsuarioActual.IdEmpleado;
                    salida.IdPersona = UsuarioActual.IdEmpleado;
                    salida.Estado = (int)TipoEstadoTransaccion.Registrado;
                    salida.FechaCreacion = FechaCreacion;
                    salida.FechaModificacion = FechaModificacion;
                    salida.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    salida.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    salida.MovimientoProducto = new List<MovimientoProducto>();

                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    var serieDocumento = SerieDocumentoBL.Instancia.Single(salida.IdSerieDocumento);
                    salida.SerieDocumento = serieDocumento.Serie;
                    salida.CondicionPago = 0;

                    if (DetalleOtrasSalidas.Count == 0)
                    {
                        jsonResponse.Success = false;
                        jsonResponse.Message = "No se puede registrar sin ingresar los productos.";
                    }
                    else
                    {
                        var secuencia = 0;
                        foreach (var movimientoProducto in DetalleOtrasSalidas)
                        {
                            movimientoProducto.Secuencia = ++secuencia;
                            movimientoProducto.FechaRegistro = salida.FechaDocumento;
                            movimientoProducto.IdAlmacen = salida.IdAlmacen;
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
                                productoStock.IdAlmacen = salida.IdAlmacen.Value;
                            }

                            if (movimientoProducto.MovimientoProductoStock.Count == 0)
                            {
                                movimientoProducto.MovimientoProductoStock.Add(
                                    new MovimientoProductoStock
                                    {
                                        Secuencia = 1,
                                        IdAlmacen = salida.IdAlmacen.Value,
                                        Cantidad = movimientoProducto.Cantidad,
                                        LoteSerie = string.Empty
                                    });
                            }

                            salida.MovimientoProducto.Add(movimientoProducto);
                        }

                        SalidaOtroConceptoBL.Instancia.Insertar(salida);

                        jsonResponse.Success = true;
                        jsonResponse.Message = "El registro se realizo correctamente.";   
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
                var salida = SalidaOtroConceptoBL.Instancia.Single(idTransaccion);
                var detalleOtrasSalidas = MovimientoProductoBL.Instancia.GetByTransaccion(idTransaccion);

                foreach (var movimientoProducto in detalleOtrasSalidas)
                {
                    movimientoProducto.MovimientoProductoStock = MovimientoProductoStockBL.Instancia.GetByMovimientoProducto(movimientoProducto.IdMovimientoProducto);
                }

                PrepararDatos(ref salida);
                DetalleOtrasSalidas = new List<MovimientoProducto>(detalleOtrasSalidas);

                return PartialView("OtrasSalidasPanel", salida);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(SalidaOtroConceptoDTO salida)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var salidaOriginal = SalidaOtroConceptoBL.Instancia.Single(salida.IdTransaccion);
                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    salidaOriginal.IdPersona = UsuarioActual.IdEmpleado;
                    salidaOriginal.IdAlmacen = salida.IdAlmacen;
                    salidaOriginal.FechaEntrega = salida.FechaEntrega;
                    salidaOriginal.Glosa = salida.Glosa;
                    salidaOriginal.MontoTipoCambio = salida.MontoTipoCambio;
                    salidaOriginal.IdEmpleado = UsuarioActual.IdEmpleado;
                    salidaOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    salidaOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    salidaOriginal.MovimientoProducto = new List<MovimientoProducto>();
                    salidaOriginal.TransaccionImpuesto = new List<TransaccionImpuesto>();

                    if (DetalleOtrasSalidas.Count == 0)
                    {
                        jsonResponse.Success = false;
                        jsonResponse.Message = "No se puede registrar sin ingresar los productos.";
                    }
                    else
                    {
                        var secuencia = 0;
                        foreach (var movimientoProducto in DetalleOtrasSalidas)
                        {
                            movimientoProducto.Secuencia = ++secuencia;
                            movimientoProducto.FechaRegistro = salida.FechaDocumento;
                            movimientoProducto.IdAlmacen = salida.IdAlmacen;
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
                                productoStock.IdAlmacen = salida.IdAlmacen.Value;
                            }

                            if (movimientoProducto.MovimientoProductoStock.Count == 0)
                            {
                                movimientoProducto.MovimientoProductoStock.Add(
                                    new MovimientoProductoStock
                                    {
                                        Secuencia = 1,
                                        IdAlmacen = salida.IdAlmacen.Value,
                                        Cantidad = movimientoProducto.Cantidad,
                                        LoteSerie = string.Empty
                                    });
                            }

                            salidaOriginal.MovimientoProducto.Add(movimientoProducto);
                        }

                        SalidaOtroConceptoBL.Instancia.Actualizar(salidaOriginal);

                        jsonResponse.Success = true;
                        jsonResponse.Message = "El registro se realizo correctamente.";   
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
            var detalle = DetalleOtrasSalidas.FirstOrDefault(p => p.IdMovimientoProducto == idDetalle);
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

            if (DetalleOtrasSalidas == null)
            {
                DetalleOtrasSalidas = new List<MovimientoProducto>();
            }

            movimientoProducto.IdMovimientoProducto = DetalleOtrasSalidas.Count == 0 ? 1 : DetalleOtrasSalidas.Max(p => p.IdMovimientoProducto) + 1;
            movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();

            return PartialView("DetalleOtrasSalidas", movimientoProducto);
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
                    foreach (var detalle in DetalleOtrasSalidas)
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

                        movimientoProducto.Secuencia = DetalleOtrasSalidas.Count + 1;
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
                            movimientoProducto.IdMovimientoProducto = DetalleOtrasSalidas.Count == 0 ? 1 : DetalleOtrasSalidas.Max(p => p.IdMovimientoProducto) + 1;
                        }

                        DetalleOtrasSalidas.Add(movimientoProducto);

                        jsonResponse.Success = true;
                        jsonResponse.Message = "El producto se registro correctamente.";   
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
                var movimientoProducto = DetalleOtrasSalidas.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                return PartialView("DetalleOtrasSalidas", movimientoProducto);
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
                    var movimientoProductoOriginal = DetalleOtrasSalidas.SingleOrDefault(p => p.IdMovimientoProducto == movimientoProducto.IdMovimientoProducto);

                    if (movimientoProductoOriginal != null)
                    {
                        var existePorductoRepetido = false;
                        foreach (var detalle in DetalleOtrasSalidas)
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
                            jsonResponse.Message = "El producto se modifico correctamente.";
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
                    var movimientoProducto = DetalleOtrasSalidas.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                    if (movimientoProducto != null)
                    {
                        DetalleOtrasSalidas.Remove(movimientoProducto);
                    }

                    var secuencial = 0;
                    foreach (var detalle in DetalleOtrasSalidas)
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

        private void PrepararDatos(ref SalidaOtroConceptoDTO salida)
        {
            var operacionDocumentos = OperacionDocumentoBL.Instancia.GetByOperacion(salida.IdOperacion);
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

            if (salida.IdTransaccion == 0)
            {
                var transaccionImpuesto = new List<TransaccionImpuesto>();
                var operacionImpuestos = OperacionImpuestoBL.Instancia.GetByOperacion(salida.IdOperacion);

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

                salida.TransaccionImpuesto = transaccionImpuesto.OrderBy(p => p.Secuencia).ToList();
            }
            else
            {
                salida.TransaccionImpuesto = TransaccionImpuestoBL.Instancia.GetByTransaccion(salida.IdTransaccion);
            }

            salida.Documentos = documentos;
            salida.Direcciones = direcciones;
            salida.Monedas = MonedaBL.Instancia.GetAll();
            salida.Almacenes = AlmacenBL.Instancia.GetByIdSucursal(IdSucursal);
        }
    }
}
