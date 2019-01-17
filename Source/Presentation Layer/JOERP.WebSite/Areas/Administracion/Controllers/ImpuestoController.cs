
namespace JOERP.WebSite.Areas.Administracion.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.Enums;
    using Helpers.JqGrid;

    public class ImpuestoController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("ImpuestoListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "Descripcion", "Estado" };
                var lista = CrearJGrid(ImpuestoBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdImpuesto.ToString(),
                    cell = new[]
                                {
                                    item.IdImpuesto.ToString(),
                                    item.Nombre,
                                    item.Descripcion,
                                    item.Abreviatura,
                                    item.Signo.ToString(), 
                                    item.Monto.ToString(),
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

            var entidad = new Impuesto
            {
                Nombre = string.Empty,
                Estado = (int)TipoEstado.Activo
            };

            PrepararDatos(ref entidad);
            return PartialView("ImpuestoPanel", entidad);
        }

        [HttpPost]
        public JsonResult Crear(Impuesto entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    entidad.Signo = Convert.ToInt32(entidad.Signo);
                    entidad.EsEditable = Convert.ToInt32(entidad.Editable);

                    entidad.FechaCreacion = FechaCreacion;
                    entidad.FechaModificacion = FechaModificacion;
                    entidad.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    entidad.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    ImpuestoBL.Instancia.Add(entidad);

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

                var entidad = ImpuestoBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref entidad);

                return PartialView("ImpuestoPanel", entidad);
            }
            catch(Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Impuesto entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var entidadOriginal = ImpuestoBL.Instancia.Single(entidad.IdImpuesto);
                    entidadOriginal.Nombre = entidad.Nombre;
                    entidadOriginal.Descripcion = entidad.Descripcion;
                    entidadOriginal.Abreviatura = entidad.Abreviatura;
                    entidadOriginal.Signo = Convert.ToInt32(entidad.Signo);
                    entidadOriginal.EsEditable = Convert.ToInt32(entidad.Editable);
                    entidadOriginal.Monto = entidad.Monto;
                    entidadOriginal.Estado = entidad.Estado;
                    entidadOriginal.FechaModificacion = FechaModificacion;
                    entidadOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    ImpuestoBL.Instancia.Update(entidadOriginal);

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

        private static void PrepararDatos(ref Impuesto entidad)
        {
            entidad.Estados = Utils.EnumToList<TipoEstado>();
            entidad.Signos = new List<Comun>
                                 {
                                     new Comun { Id = -1 },new Comun{ Id = 0 },new Comun{ Id = 1 },
                                 };
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var impuestoId = Convert.ToInt32(id);
                    ImpuestoBL.Instancia.Delete(impuestoId);

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
