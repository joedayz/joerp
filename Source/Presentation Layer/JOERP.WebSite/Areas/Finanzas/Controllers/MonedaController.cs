
namespace JOERP.WebSite.Areas.Finanzas.Controllers
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

    public class MonedaController : BaseController
    {
        public ActionResult Index(string id)
        {
            return PartialView("MonedaListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
          var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "Abreviatura", "Simbolo", "Estado","IdEmpresa" };
                var lista = CrearJGrid(MonedaBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdMoneda.ToString(),
                    cell = new[]
                                {
                                    item.IdMoneda.ToString(),
                                    item.Nombre,
                                    item.Abreviatura,
                                    item.Simbolo,
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

        public ActionResult Crear()
        {
            ViewData["Accion"] = "Crear";

            var moneda = new Moneda
            {
                Nombre = string.Empty,
                Simbolo = string.Empty,
                Abreviatura=string.Empty,
                Estado = (int)TipoEstado.Activo
            };

            PrepararDatos(ref moneda);
            return PartialView("MonedaPanel", moneda);
        }

        [HttpPost]
        public JsonResult Crear(Moneda moneda)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    moneda.FechaCreacion = FechaCreacion;
                    moneda.FechaModificacion = FechaModificacion;
                    moneda.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    moneda.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    moneda.IdEmpresa = IdEmpresa;

                    MonedaBL.Instancia.Add(moneda);

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

                var moneda = MonedaBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref moneda);

                return PartialView("MonedaPanel", moneda);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Moneda entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var monedaOriginal = MonedaBL.Instancia.Single(entidad.IdMoneda);
                    monedaOriginal.Nombre = entidad.Nombre;
                    monedaOriginal.Abreviatura = entidad.Abreviatura;
                    monedaOriginal.Simbolo = entidad.Simbolo;
                    monedaOriginal.Estado = entidad.Estado;
                    monedaOriginal.FechaModificacion = FechaModificacion;
                    monedaOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    MonedaBL.Instancia.Update(monedaOriginal);
                    
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

        private static void PrepararDatos(ref Moneda moneda)
        {
            moneda.Estados = Utils.EnumToList<TipoEstado>();
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var monedaId = Convert.ToInt32(id);
                    MonedaBL.Instancia.Delete(monedaId);

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

