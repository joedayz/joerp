
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

    public class TablaController : BaseController
    {
        #region Tabla

        public ActionResult Index(string id)
        {
            return PartialView("TablaListado");
        }

        [HttpPost]
        public JsonResult ListarGeneral(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "Descripcion", "Estado" };
                var lista = CrearJGrid(TablaBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdTabla.ToString(),
                    cell = new[]
                                {
                                    item.IdTabla.ToString(),
                                    item.Nombre,
                                    item.Descripcion,
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

        #endregion Tabla

        #region ItemTabla

        public ActionResult CrearItemTabla(string id)
        {
            ViewData["Accion"] = "CrearItemTabla";

            var entidad = new ItemTabla
            {
                IdTabla = Convert.ToInt32(id),
                Estado = (int)TipoEstado.Activo
            };
            entidad.IdItemTabla = ItemTablaBL.Instancia.GetMaximoId()+1;
            PrepararDatos(ref entidad);
            return PartialView("ItemTablePanel", entidad);
        }

        [HttpPost]
        public JsonResult CrearItemTabla(ItemTabla itemTabla)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    ItemTablaBL.Instancia.Add(itemTabla);
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

        public ActionResult ModificarItemTabla(string id)
        {
            var cadena = id.Split('-');

            try
            {
                ViewData["Accion"] = "ModificarItemTabla";

                var itemTabla = ItemTablaBL.Instancia.Single(Convert.ToInt32(cadena[0]), Convert.ToInt32(cadena[1]));
                if (itemTabla.Codigo == null)
                    itemTabla.Codigo = " ";
                PrepararDatos(ref itemTabla);

                return PartialView("ItemTablePanel", itemTabla);
            }
            catch(Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ModificarItemTabla(ItemTabla itemTabla)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var itemTablaOriginal = ItemTablaBL.Instancia.Single(itemTabla.IdTabla, itemTabla.IdItemTabla);
                    itemTablaOriginal.Nombre = itemTabla.Nombre;
                    itemTablaOriginal.Descripcion = itemTabla.Descripcion;
                    itemTablaOriginal.Estado = itemTabla.Estado;
                    itemTablaOriginal.Valor = itemTabla.Valor ?? string.Empty;
                    itemTablaOriginal.Codigo = itemTabla.Codigo;
                    ItemTablaBL.Instancia.Update(itemTablaOriginal);
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
        public JsonResult ListarDetalle(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "Valor", "Descripcion","Estado","IdTabla"};
                var lista = CrearJGrid(ItemTablaBL.Instancia, grid, nombreFiltros, ref jqgrid);
           
                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdItemTabla.ToString(),
                    cell = new[]
                                {
                                    item.IdItemTabla.ToString(),
                                    item.Nombre,
                                    item.Valor,
                                    item.Descripcion,
                                    item.NombreEstado,
                                    item.IdTabla.ToString()
                                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
            return Json(jqgrid);
        }

        private static void PrepararDatos(ref ItemTabla itemTabla)
        {
            itemTabla.Estados = Utils.EnumToList<TipoEstado>();
        }

        [HttpPost]
        public JsonResult EliminarItemTabla(string idItemTabla, string idTabla)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var itemTabla = Convert.ToInt32(idItemTabla);
                    var tabla = Convert.ToInt32(idTabla);
                    ItemTablaBL.Instancia.Delete(itemTabla,tabla);

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

        #endregion ItemTabla
    }
}