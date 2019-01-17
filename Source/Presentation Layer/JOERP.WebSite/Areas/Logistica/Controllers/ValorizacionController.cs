
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

    public class ValorizacionController : BaseController
    {
        public IList<TransaccionDocumento> DetalleValorizacion
        {
            get { return (IList<TransaccionDocumento>)Session["DetalleValorizacion"]; }
            set { Session["DetalleValorizacion"] = value; }
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
            return PartialView("ListadoValorizacion");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "IdOperacion", "Documento", "RazonSocial", "FechaDocumento", "Estado" };
                var lista = CrearJGrid(ValorizacionBL.Instancia, grid, nombreFiltros, ref jqgrid);

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
            if (DetalleValorizacion == null) return Json(new JGrid());

            var jqgrid = CrearJgrid(grid, DetalleValorizacion.Count);

            jqgrid.rows = DetalleValorizacion.Select(item => new JRow
            {
                id = item.IdTransaccionDocumento.ToString(),
                cell = new[]
                                {
                                    item.IdTransaccionDocumento.ToString(),
                                    string.Format("{0}-{1}-{2}", item.Documento, item.SerieDocumento, item.NumeroDocumento),
                                    item.FechaDocumento.ToString("dd/MM/yyyy"),
                                    item.ImpuestoNombre,
                                    item.Comentarios,
                                    item.MonedaNombre,
                                    item.TipoCambio.ToString(),
                                    item.Monto.ToString()
                                }
            }).ToArray();

            return Json(jqgrid);
        }

        public JsonResult CargarNumerosDocumento(int idSerieDocumento)
        {
            var numerosDocumentos = CompraBL.Instancia.GetNumerosDocumentosByIdSerieDocumento(IdOperacion, idSerieDocumento);
            var listItems = Utils.ConvertToListItem(numerosDocumentos, "Id", "Valor", false);
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CargarDatosDocumento(int idTransaccion)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                var transaccion = CompraBL.Instancia.GetDatosDocumento(idTransaccion);
                
                jsonResponse.Data = transaccion;
                jsonResponse.Success = true;
            }
            catch (Exception ex)
            {
                jsonResponse.Data = ex.Message;
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Crear()
        {
            ViewData["Accion"] = "Crear";

            var valorizacion = new Valorizacion { FechaRegistro = DateTime.Now };
            DetalleValorizacion = new List<TransaccionDocumento>();

            PrepararDatos(ref valorizacion);
            return PartialView("ValorizacionPanel", valorizacion);
        }

        [HttpPost]
        public JsonResult Crear(Valorizacion valorizacion)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    if (valorizacion.IdTransaccionReferencia.HasValue)
                    {
                        var compra = CompraBL.Instancia.Single(valorizacion.IdTransaccionReferencia.Value);
                        
                        valorizacion.IdEmpresa = IdEmpresa;
                        valorizacion.IdSucursal = IdSucursal;
                        valorizacion.IdOperacion = IdOperacion;
                        valorizacion.IdEmpleado = UsuarioActual.IdEmpleado;
                        valorizacion.IdTipoDocumento = compra.IdTipoDocumento;
                        valorizacion.SerieDocumento = compra.SerieDocumento;
                        valorizacion.NumeroDocumento = compra.NumeroDocumento;
                        valorizacion.FechaDocumento = compra.FechaDocumento;
                        valorizacion.IdPersona = compra.IdPersona;
                        valorizacion.Estado = (int)TipoEstadoTransaccion.Registrado;
                        valorizacion.FechaCreacion = FechaCreacion;
                        valorizacion.FechaModificacion = FechaModificacion;
                        valorizacion.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        valorizacion.UsuarioModificacion = UsuarioActual.IdEmpleado;
                        valorizacion.DocumentosRelacionados = new List<TransaccionDocumento>(DetalleValorizacion);

                        var resultado = ValorizacionBL.Instancia.Add(valorizacion);

                        jsonResponse.Success = true;
                        jsonResponse.Data = resultado.IdTransaccion;
                        jsonResponse.Message = "Se Proceso con exito.";
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
                var valorizacion = ValorizacionBL.Instancia.Single(idTransaccion);
                var detalleValorizacion = TransaccionDocumentoBL.Instancia.GetByTrasaccion(idTransaccion);

                PrepararDatos(ref valorizacion);
                DetalleValorizacion = new List<TransaccionDocumento>(detalleValorizacion);

                return PartialView("ValorizacionPanel", valorizacion);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Valorizacion valorizacion) 
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    if (valorizacion.IdTransaccionReferencia.HasValue)
                    {
                        var compra = CompraBL.Instancia.Single(valorizacion.IdTransaccionReferencia.Value);
                        var valorizacionOriginal = ValorizacionBL.Instancia.Single(valorizacion.IdTransaccion);

                        valorizacionOriginal.IdPersona = compra.IdPersona;
                        valorizacionOriginal.IdTipoDocumento = compra.IdTipoDocumento;
                        valorizacionOriginal.SerieDocumento = compra.SerieDocumento;
                        valorizacionOriginal.NumeroDocumento = compra.NumeroDocumento;
                        valorizacionOriginal.FechaDocumento = compra.FechaDocumento;
                        valorizacionOriginal.Glosa = valorizacion.Glosa;
                        valorizacionOriginal.IdEmpleado = UsuarioActual.IdEmpleado;
                        valorizacionOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        valorizacionOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                        valorizacionOriginal.DocumentosRelacionados = new List<TransaccionDocumento>(DetalleValorizacion);

                        ValorizacionBL.Instancia.Update(valorizacionOriginal);

                        jsonResponse.Success = true;
                        jsonResponse.Message = "Se Proceso con exito.";
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

        public ActionResult CrearDetalle()
        {
            ViewData["Accion"] = "CrearDetalle";

            var transaccionDocumento = new TransaccionDocumento { FechaDocumento = DateTime.Now };

            if (DetalleValorizacion == null)
            {
                DetalleValorizacion = new List<TransaccionDocumento>();
            }

            transaccionDocumento.IdTransaccionDocumento = DetalleValorizacion.Count == 0 ? 1 : DetalleValorizacion.Max(p => p.IdTransaccionDocumento) + 1;

            PrepararDatosDetalle(ref transaccionDocumento);
            return PartialView("ValorizacionDetalle", transaccionDocumento);
        }

        [HttpPost]
        public JsonResult CrearDetalle(TransaccionDocumento transaccionDocumento)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    transaccionDocumento.Documento = ItemTablaBL.Instancia.Single((int)TipoTabla.TipoComprobante,transaccionDocumento.IdTipoDocumento).Nombre;
                    transaccionDocumento.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    transaccionDocumento.FechaCreacion = FechaCreacion;

                    if (transaccionDocumento.IdTransaccionDocumento == 0)
                    {
                        transaccionDocumento.IdTransaccionDocumento = DetalleValorizacion.Count == 0 ? 1 : DetalleValorizacion.Max(p => p.IdTransaccionDocumento) + 1;
                    }

                    DetalleValorizacion.Add(transaccionDocumento);

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

                var idTransaccionDocumento = Convert.ToInt32(id);
                var transaccionDocumento = DetalleValorizacion.SingleOrDefault(p => p.IdTransaccionDocumento == idTransaccionDocumento);

                PrepararDatosDetalle(ref transaccionDocumento);
                return PartialView("ValorizacionDetalle", transaccionDocumento);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ModificarDetalle(TransaccionDocumento transaccionDocumento)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var transaccionDocumentoOriginal = DetalleValorizacion.SingleOrDefault(p => p.IdTransaccionDocumento == transaccionDocumento.IdTransaccionDocumento);

                    if (transaccionDocumentoOriginal != null)
                    {
                        transaccionDocumentoOriginal.IdImpuesto = transaccionDocumento.IdImpuesto;
                        transaccionDocumentoOriginal.IdTipoDocumento = transaccionDocumento.IdTipoDocumento;
                        transaccionDocumentoOriginal.IdMoneda = transaccionDocumento.IdMoneda;
                        transaccionDocumentoOriginal.SerieDocumento = transaccionDocumento.SerieDocumento;
                        transaccionDocumentoOriginal.NumeroDocumento = transaccionDocumento.NumeroDocumento;
                        transaccionDocumentoOriginal.FechaDocumento = transaccionDocumento.FechaDocumento;
                        transaccionDocumentoOriginal.TipoCambio = transaccionDocumento.TipoCambio;
                        transaccionDocumentoOriginal.Monto = transaccionDocumento.Monto;
                        transaccionDocumentoOriginal.Comentarios = transaccionDocumento.Comentarios;
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
                    var idTransaccionDocumento = Convert.ToInt32(id);
                    var movimientoProducto = DetalleValorizacion.SingleOrDefault(p => p.IdTransaccionDocumento == idTransaccionDocumento);

                    if (movimientoProducto != null)
                    {
                        DetalleValorizacion.Remove(movimientoProducto);
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

        public JsonResult Valorizar(int idTransaccion)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                ValorizacionBL.Instancia.Valorizar(idTransaccion);
                var idCompra = ValorizacionBL.Instancia.Single(idTransaccion).IdTransaccionReferencia;
                var movimientos = MovimientoProductoBL.Instancia.GetByTransaccion((int)idCompra);
                jsonResponse.Data = movimientos;
                jsonResponse.Success = true;
            }
            catch (Exception ex)
            {
                jsonResponse.Data = ex.Message;
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        private void PrepararDatos(ref Valorizacion valorizacion)
        {
            var operacionDocumentos = OperacionDocumentoBL.Instancia.GetByOperacion(IdOperacion);
            var tiposDocumentos = ItemTablaBL.Instancia.GetByTabla((int)TipoTabla.TipoComprobante);

            valorizacion.Documentos = new List<Comun>();
            foreach (var documento in operacionDocumentos)
            {
                var tipoDocumento = tiposDocumentos.FirstOrDefault(p => p.IdItemTabla == documento.TipoDocumento);
                valorizacion.Documentos.Add(new Comun { Id = documento.TipoDocumento, Nombre = tipoDocumento.Valor });
            }
        }

        private void PrepararDatosDetalle(ref TransaccionDocumento transaccionDocumento)
        {
            transaccionDocumento.Impuestos = ImpuestoBL.Instancia.Get();
            transaccionDocumento.Monedas = MonedaBL.Instancia.GetAll();
            transaccionDocumento.TiposDocumentos = ItemTablaBL.Instancia.GetByTabla((int)TipoTabla.TipoComprobante);
        }
    }
}
