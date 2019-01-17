
namespace JOERP.WebSite.Areas.Administracion.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Logic;
    using Helpers;
    using Helpers.Enums;
    using Helpers.JqGrid;
    using Core;

    public class RolController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("RolListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "RazonSocial", "Estado" };
                var lista = CrearJGrid(RolBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdRol.ToString(),
                    cell = new[]
                                {
                                    item.IdRol.ToString(),
                                    item.Nombre,
                                    item.Empresa.RazonSocial,
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

            var entidad = new Rol
            {
                Nombre = string.Empty,
            };

            PrepararDatos(ref entidad);
            return PartialView("RolPanel", entidad);
        }

        [HttpPost]
        public JsonResult Crear(Rol rol)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    rol.FechaCreacion = FechaCreacion;
                    rol.FechaModificacion = FechaModificacion;
                    rol.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    rol.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    rol.IdEmpresa = IdEmpresa;

                    RolBL.Instancia.Add(rol);

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

                var entidad = RolBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref entidad);

                return PartialView("RolPanel", entidad);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Rol rol)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var rolOriginal = RolBL.Instancia.Single(rol.IdRol);
                    rolOriginal.Nombre = rol.Nombre;
                    rolOriginal.IdEmpresa = IdEmpresa;
                    rolOriginal.Estado = rol.Estado;
                    rolOriginal.FechaModificacion = FechaModificacion;
                    rolOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    RolBL.Instancia.Update(rolOriginal);

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

        private static void PrepararDatos(ref Rol rol)
        {
            rol.Estados = Utils.EnumToList<TipoEstado>();
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var rolId= Convert.ToInt32(id);
                    RolBL.Instancia.Delete(rolId);

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
