
namespace JOERP.WebSite.Areas.Administracion.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.Enums;
    using Helpers.JqGrid;

    public class SerieDocumentoController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("SerieDocumentoListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "TipoDocumento", "Serie", "NumeroActual","NumeroInicio","NumeroFinal","FechaCreacion" };
                var lista = CrearJGrid(SerieDocumentoBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdSerieDocumento.ToString(),
                    cell = new[]
                                {
                                    item.IdSerieDocumento.ToString(),
                                    ItemTablaBL.Instancia.Single((int)TipoTabla.TipoComprobante,item.TipoDocumento).Nombre ,
                                    item.Serie,
                                    item.NumeroActual,
                                    item.NumeroInicio,
                                    item.NumeroFinal,
                                    item.FechaCreacion.ToShortDateString(),
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

            var entidad = new SerieDocumento();

            PrepararDatos(ref entidad);
            return PartialView("SerieDocumentoPanel", entidad);
        }

        [HttpPost]
        public JsonResult Crear(SerieDocumento serieDocumento)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    serieDocumento.FechaCreacion = FechaCreacion;
                    serieDocumento.FechaModificacion = FechaModificacion;
                    serieDocumento.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    serieDocumento.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    serieDocumento.IdSucursal = IdSucursal;

                    SerieDocumentoBL.Instancia.Add(serieDocumento);

                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se Proceso con exito.";
                }
                catch(Exception ex)
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

                var serieDocumento = SerieDocumentoBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref serieDocumento);

                return PartialView("SerieDocumentoPanel", serieDocumento);
            }
            catch(Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(SerieDocumento serieDocumento)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var serieDocumentoOriginal = SerieDocumentoBL.Instancia.Single(serieDocumento.IdSerieDocumento);
                    serieDocumentoOriginal.Serie = serieDocumento.Serie;
                    serieDocumentoOriginal.NumeroActual = serieDocumento.NumeroActual;
                    serieDocumentoOriginal.NumeroInicio = serieDocumento.NumeroInicio;
                    serieDocumentoOriginal.NumeroFinal = serieDocumento.NumeroFinal;
                    serieDocumentoOriginal.TipoDocumento = serieDocumento.TipoDocumento;
                    serieDocumentoOriginal.FechaModificacion = FechaModificacion;
                    serieDocumentoOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    SerieDocumentoBL.Instancia.Update(serieDocumentoOriginal);

                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se Proceso con exito";
                }
                catch(Exception ex)
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

        private static void PrepararDatos(ref SerieDocumento serieDocumento)
        {
            serieDocumento.TipoDocumentos = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoComprobante);
        }

        public ActionResult TiposDocumentoHtml()
        {
            var tiposDocumento = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoComprobante);
            var select = "<select>";
            select += "<option value=''></option>";

            foreach (var tipoDocumento in tiposDocumento)
            {
                select += string.Format("<option value='{0}'>{1}</option>", tipoDocumento.Id, tipoDocumento.Nombre);
            }

            select += "</select>";

            return Content(select);
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var sucursalId = Convert.ToInt32(id);
                    SerieDocumentoBL.Instancia.Delete(sucursalId);

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
