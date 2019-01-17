
namespace JOERP.WebSite.Areas.Administracion.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.JqGrid;

    public class CargoController : BaseController
    {
        public ActionResult Index(string id)
        {
            return PartialView("CargoListado");
        }
        
        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "Descripcion", "Empresa"};
                var lista = CrearJGrid(CargoBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdCargo.ToString(),
                    cell = new[]
                                {
                                    item.IdCargo.ToString(),
                                    item.Nombre,
                                    item.Descripcion,
                                    item.NombreEmpresa,
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

            var entidad = new Cargo
            {
                Nombre = string.Empty,
                Descripcion =  string.Empty
            };

            PrepararDatos(ref entidad);
            return PartialView("CargoPanel", entidad);
        }

        [HttpPost]
        public JsonResult Crear(Cargo entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    entidad.FechaCreacion = FechaCreacion;
                    entidad.FechaModificacion = FechaModificacion;
                    entidad.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    entidad.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    entidad.IdEmpresa = IdEmpresa;

                    CargoBL.Instancia.Add(entidad);

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

                var entidad = CargoBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref entidad);

                return PartialView("CargoPanel", entidad);
            }
            catch
            {
                return MensajeError();
            }
        }

        [HttpPost]
        public JsonResult Modificar(Cargo entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var entidadOriginal = CargoBL.Instancia.Single(entidad.IdCargo);
                    entidadOriginal.Nombre = entidad.Nombre;
                    entidadOriginal.IdEmpresa = IdEmpresa;
                    entidadOriginal.Descripcion = entidad.Descripcion;
                    entidadOriginal.FechaModificacion = FechaModificacion;
                    entidadOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    CargoBL.Instancia.Update(entidadOriginal);

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

        private static void PrepararDatos(ref Cargo entidad)
        {

        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var cargoId = Convert.ToInt32(id);
                    CargoBL.Instancia.Delete(cargoId);

                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se quito el registro con exito.";
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = "No es posible Eliminar, hay empleados registrados con este cargo. ";
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
