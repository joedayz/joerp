
namespace JOERP.WebSite.Areas.Logistica.Controllers
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

    public class AlmacenController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("AlmacenListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "Abreviatura", "Sucursal", "Estado" };
                var lista = CrearJGrid(AlmacenBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdAlmacen.ToString(),
                    cell = new[]
                                {
                                    item.IdAlmacen.ToString(),
                                    item.Nombre,
                                    item.Abreviatura,
                                    item.NombreSucursal,
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

            var almacen = new Almacen
            {
                Nombre = string.Empty,
                Abreviatura = string.Empty,
                Descripcion = string.Empty
            };
            
            PrepararDatos(ref almacen, IdEmpresa);
            return PartialView("AlmacenPanel", almacen);
        }

        [HttpPost]
        public JsonResult Crear(Almacen almacen)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    almacen.FechaCreacion = FechaCreacion;
                    almacen.FechaModificacion = FechaModificacion;
                    almacen.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    almacen.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    AlmacenBL.Instancia.Add(almacen);

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

                var almacen = AlmacenBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref almacen, IdEmpresa);

                return PartialView("AlmacenPanel", almacen);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Almacen almacen)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var almacenOriginal = AlmacenBL.Instancia.Single(almacen.IdAlmacen);
                    almacenOriginal.Nombre = almacen.Nombre;
                    almacenOriginal.Abreviatura = almacen.Abreviatura;
                    almacenOriginal.Descripcion = almacen.Descripcion ?? string.Empty;
                    almacenOriginal.Estado = almacen.Estado;
                    almacenOriginal.IdSucursal = almacen.IdSucursal;
                    almacenOriginal.FechaModificacion = FechaModificacion;
                    almacenOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    AlmacenBL.Instancia.Update(almacenOriginal);

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
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var almacenId = Convert.ToInt32(id);
                    AlmacenBL.Instancia.Delete(almacenId);

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

        public JsonResult ListarAlmacenes(int idSucursal)
        {
            var almacenes = AlmacenBL.Instancia.GetByIdSucursal(idSucursal);
            var listItems = Utils.ConvertToListItem(almacenes, "IdAlmacen", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        private static void PrepararDatos(ref Almacen entidad, int id)
        {
            var sucursales = SucursalBL.Instancia.GetByEmpresa(id);
            entidad.Sucursales = Utils.ConvertToComunList(sucursales, "IdSucursal", "Nombre");
            entidad.Estados = Utils.EnumToList<TipoEstado>();
        }
    }
}
