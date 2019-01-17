
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

    public class CompraController : BaseController
    {
        public IList<MovimientoProducto> DetalleCompra
        {
            get { return (IList<MovimientoProducto>)Session["DetalleCompra"]; }
            set { Session["DetalleCompra"] = value; }
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

            return PartialView("CompraListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "IdOperacion", "Documento", "RazonSocial", "FechaDocumento", "Estado" };
                var lista = CrearJGrid(CompraBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdTransaccion.ToString(),
                    cell = new[]
                                {   
                                    item.IdTransaccion.ToString(),
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
            if (DetalleCompra == null)
            {
                return Json(new JGrid());
            }

            var jqgrid = CrearJgrid(grid, DetalleCompra.Count);

            jqgrid.rows = DetalleCompra.Select(item => new JRow
            {
                id = item.IdMovimientoProducto.ToString(),
                cell = new[]
                                {
                                    item.IdMovimientoProducto.ToString(),
                                    item.CodigoProducto,
                                    item.NombreProducto,
                                    item.NombrePresentacion,
                                    item.PrecioBase.ToString(),
                                    item.CantidadDocumento.ToString(),
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

            var compra = new Compra
            {
                IdOperacion = IdOperacion,
                FechaDocumento = FechaSistema,
                Estado = (int)TipoEstadoTransaccion.Registrado,
            };
            DetalleCompra = new List<MovimientoProducto>();

            PrepararDatos(ref compra);
            return PartialView("CompraPanel", compra);
        }

        [HttpPost]
        public JsonResult Crear(Compra compra)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    compra.IdEmpresa = IdEmpresa;
                    compra.IdSucursal = IdSucursal;
                    compra.IdOperacion = IdOperacion;
                    compra.IdEmpleado = UsuarioActual.IdEmpleado;
                    compra.Estado = (int) TipoEstadoTransaccion.Registrado;
                    compra.FechaCreacion = FechaCreacion;
                    compra.FechaModificacion = FechaModificacion;
                    compra.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    compra.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    compra.MovimientoProducto = new List<MovimientoProducto>();

                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    var serieDocumento = SerieDocumentoBL.Instancia.Single(compra.IdSerieDocumento);
                    compra.SerieDocumento = serieDocumento.Serie;
                    compra.CondicionPago = 0;

                    if (DetalleCompra.Count == 0)
                    {
                        jsonResponse.Success = false;
                        jsonResponse.Message = "No se puede registrar la compra sin ingresar los productos.";
                    }
                    else
                    {
                        var secuencia = 0;
                        foreach (var movimientoProducto in DetalleCompra)
                        {
                            movimientoProducto.Secuencia = ++secuencia;
                            movimientoProducto.FechaRegistro = compra.FechaDocumento;
                            movimientoProducto.IdAlmacen = compra.IdAlmacen;
                            movimientoProducto.SignoStock = operacion.SignoStock;
                            movimientoProducto.Estado = (int) TipoEstadoTransaccion.Registrado;
                            movimientoProducto.FechaCreacion = FechaCreacion;
                            movimientoProducto.FechaModificacion = FechaModificacion;
                            movimientoProducto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                            movimientoProducto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                            var secuenciaDetalle = 0;
                            foreach (var productoStock in movimientoProducto.MovimientoProductoStock)
                            {
                                productoStock.Secuencia = ++secuenciaDetalle;
                                productoStock.IdAlmacen = compra.IdAlmacen.Value;
                            }

                            if (movimientoProducto.MovimientoProductoStock.Count == 0)
                            {
                                movimientoProducto.MovimientoProductoStock.Add(
                                    new MovimientoProductoStock
                                        {
                                            Secuencia = 1,
                                            IdAlmacen = compra.IdAlmacen.Value,
                                            Cantidad = movimientoProducto.Cantidad,
                                            LoteSerie = string.Empty
                                        });
                            }

                            compra.MovimientoProducto.Add(movimientoProducto);
                        }

                        foreach (var impuesto in compra.TransaccionImpuesto)
                        {
                            impuesto.FechaCreacion = FechaCreacion;
                            impuesto.FechaModificacion = FechaModificacion;
                            impuesto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                            impuesto.UsuarioModificacion = UsuarioActual.IdEmpleado;
                        }

                        var impuestoTotal =
                            compra.TransaccionImpuesto.FirstOrDefault(p => p.IdImpuesto == (int) TipoImpuesto.Total);
                        if (impuestoTotal != null)
                        {
                            compra.MontoNeto = impuestoTotal.Valor;
                        }

                        CompraBL.Instancia.Insertar(compra);

                        jsonResponse.Success = true;
                        jsonResponse.Message = "La compra fue registrada correctamente.";
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
                var compra = CompraBL.Instancia.Single(idTransaccion);
                var detalleCompra = MovimientoProductoBL.Instancia.GetByTransaccion(idTransaccion);

                foreach (var movimientoProducto in detalleCompra)
                {
                    movimientoProducto.MovimientoProductoStock = MovimientoProductoStockBL.Instancia.GetByMovimientoProducto(movimientoProducto.IdMovimientoProducto);
                }

                PrepararDatos(ref compra);
                DetalleCompra = new List<MovimientoProducto>(detalleCompra);

                return PartialView("CompraPanel", compra);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Compra compra)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var compraOriginal = CompraBL.Instancia.Single(compra.IdTransaccion);
                    var operacion = OperacionBL.Instancia.Single(IdOperacion);

                    compraOriginal.IdPersona = compra.IdPersona;
                    compraOriginal.IdAlmacen = compra.IdAlmacen;
                    compraOriginal.FechaEntrega = compra.FechaEntrega;
                    compraOriginal.Glosa = compra.Glosa;
                    compraOriginal.MontoTipoCambio = compra.MontoTipoCambio;
                    compraOriginal.IdEmpleado = UsuarioActual.IdEmpleado;
                    compraOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    compraOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    compraOriginal.MovimientoProducto = new List<MovimientoProducto>();
                    compraOriginal.TransaccionImpuesto = new List<TransaccionImpuesto>();

                    if (DetalleCompra.Count == 0)
                    {
                        jsonResponse.Success = false;
                        jsonResponse.Message = "No se puede registrar la compra sin ingresar los productos.";   
                    }
                    else
                    {
                        var secuencia = 0;
                        foreach (var movimientoProducto in DetalleCompra)
                        {
                            movimientoProducto.Secuencia = ++secuencia;
                            movimientoProducto.FechaRegistro = compra.FechaDocumento;
                            movimientoProducto.IdAlmacen = compra.IdAlmacen;
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
                                productoStock.IdAlmacen = compra.IdAlmacen.Value;
                            }

                            if (movimientoProducto.MovimientoProductoStock.Count == 0)
                            {
                                movimientoProducto.MovimientoProductoStock.Add(
                                    new MovimientoProductoStock
                                    {
                                        Secuencia = 1,
                                        IdAlmacen = compra.IdAlmacen.Value,
                                        Cantidad = movimientoProducto.Cantidad,
                                        LoteSerie = string.Empty
                                    });
                            }

                            compraOriginal.MovimientoProducto.Add(movimientoProducto);
                        }

                        foreach (var impuesto in compra.TransaccionImpuesto)
                        {
                            impuesto.FechaCreacion = FechaCreacion;
                            impuesto.FechaModificacion = FechaModificacion;
                            impuesto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                            impuesto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                            compraOriginal.TransaccionImpuesto.Add(impuesto);
                        }

                        var impuestoTotal = compra.TransaccionImpuesto.FirstOrDefault(p => p.IdImpuesto == (int)TipoImpuesto.Total);
                        if (impuestoTotal != null)
                        {
                            compraOriginal.MontoNeto = impuestoTotal.Valor;
                        }

                        CompraBL.Instancia.Actualizar(compraOriginal);

                        jsonResponse.Success = true;
                        jsonResponse.Message = "La compra fue actualizada correctamente.";   
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

        public ActionResult CargarDesdeOrden (int id)
        {
            try
            {
                ViewData["Accion"] = "Crear";

                var idTransaccion = Convert.ToInt32(id);
                var compra = CompraBL.Instancia.Single(idTransaccion);
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
                DetalleCompra = new List<MovimientoProducto>(detalleCompra);

                return PartialView("CompraPanel", compra);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }
        
        public JsonResult ListarLotes(int idDetalle)
        {
            var detalle = DetalleCompra.FirstOrDefault(p => p.IdMovimientoProducto == idDetalle);
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

            if (DetalleCompra == null)
            {
                DetalleCompra = new List<MovimientoProducto>();
            }

            movimientoProducto.IdMovimientoProducto = DetalleCompra.Count == 0 ? 1 : DetalleCompra.Max(p => p.IdMovimientoProducto) + 1;
            movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();

            return PartialView("DetalleCompraPanel", movimientoProducto);
        }

        [HttpPost]
        public JsonResult CrearDetalle(MovimientoProducto movimientoProducto, string lotesJson, int tipoCompra)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var existePorductoRepetido = false;
                    foreach (var detalle in DetalleCompra)
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

                        movimientoProducto.Secuencia = DetalleCompra.Count + 1;
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
                        movimientoProducto.MovimientoProductoStock = new List<MovimientoProductoStock>();

                        foreach (var movimientoProductoStock in lotes)
                        {
                            movimientoProductoStock.FechaVencimiento = Convert.ToDateTime(movimientoProductoStock.FechaVencimientoFormato);
                            movimientoProducto.MovimientoProductoStock.Add(movimientoProductoStock);
                        }

                        if (movimientoProducto.IdMovimientoProducto == 0)
                        {
                            movimientoProducto.IdMovimientoProducto = DetalleCompra.Count == 0 ? 1 : DetalleCompra.Max(p => p.IdMovimientoProducto) + 1;
                        }

                        DetalleCompra.Add(movimientoProducto);

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
                var movimientoProducto = DetalleCompra.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                return PartialView("DetalleCompraPanel", movimientoProducto);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ModificarDetalle(MovimientoProducto movimientoProducto, string lotesJson, int tipoCompra )
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var movimientoProductoOriginal = DetalleCompra.SingleOrDefault(p => p.IdMovimientoProducto == movimientoProducto.IdMovimientoProducto);

                    if (movimientoProductoOriginal != null)
                    {
                        var existePorductoRepetido = false;
                        foreach (var detalle in DetalleCompra)
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
                            movimientoProductoOriginal.MovimientoProductoStock = new List<MovimientoProductoStock>();

                            foreach (var movimientoProductoStock in lotes)
                            {
                                movimientoProductoStock.FechaVencimiento = Convert.ToDateTime(movimientoProductoStock.FechaVencimientoFormato);
                                movimientoProductoOriginal.MovimientoProductoStock.Add(movimientoProductoStock);
                            }

                            jsonResponse.Success = true;
                            jsonResponse.Message = "El detalle del producto fue ingresado correctamente.";    
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
                    var movimientoProducto = DetalleCompra.SingleOrDefault(p => p.IdMovimientoProducto == idMovimientoProducto);

                    if (movimientoProducto != null)
                    {
                        DetalleCompra.Remove(movimientoProducto);
                    }

                    var secuencial = 0;
                    foreach (var detalle in DetalleCompra)
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

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            try
            {
                var idTransaccion = Convert.ToInt32(id);
                var compra = CompraBL.Instancia.Single(idTransaccion);

                if (compra.Estado == (int)TipoEstado.Inactivo)
                {
                    jsonResponse.Message = "La Compra ya se encuentra eliminada.";
                }
                else
                {
                    compra.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    CompraBL.Instancia.Eliminar(compra);

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

        private void PrepararDatos(ref Compra compra)
        {
            var operacionDocumentos = OperacionDocumentoBL.Instancia.GetByOperacion(compra.IdOperacion);
            var tiposDocumentos = ItemTablaBL.Instancia.GetByTabla((int)TipoTabla.TipoComprobante);
            var documentos = new List<Comun>();

            foreach (var documento in operacionDocumentos)
            {
                var tipoDocumento = tiposDocumentos.FirstOrDefault(p => p.IdItemTabla == documento.TipoDocumento);
                documentos.Add(new Comun { Id = documento.TipoDocumento, Nombre = tipoDocumento.Valor });
            }

            compra.TiposCompra = new List<Comun>();
            compra.TiposCompra.Add(new Comun { Id = 0, Nombre = "--Seleccionar--" });
            compra.TiposCompra.Add(new Comun { Id = 1, Nombre = "Local" });
            compra.TiposCompra.Add(new Comun { Id = 2, Nombre = "Extranjera" });
            
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

            if (compra.IdTransaccion == 0)
            {
                var transaccionImpuesto = new List<TransaccionImpuesto>();
                var operacionImpuestos = OperacionImpuestoBL.Instancia.GetByOperacion(compra.IdOperacion);

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

                compra.TransaccionImpuesto = transaccionImpuesto.OrderBy(p => p.Secuencia).ToList();
            }
            else
            {
                compra.TransaccionImpuesto = TransaccionImpuestoBL.Instancia.GetByTransaccion(compra.IdTransaccion);
            }

            compra.Documentos = documentos;
            compra.Direcciones = direcciones;
            compra.Monedas = MonedaBL.Instancia.GetAll();
            compra.Almacenes = AlmacenBL.Instancia.GetByIdSucursal(IdSucursal);
        }
    }
}

