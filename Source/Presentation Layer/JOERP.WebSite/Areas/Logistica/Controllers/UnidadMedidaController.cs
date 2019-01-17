
namespace JOERP.WebSite.Areas.Logistica.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.JqGrid;

    public class UnidadMedidaController : BaseController
    {
        public ActionResult Index(string id)
        {
            return PartialView("UnidadMedidaListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "Abreviatura", "Descripcion", "IdEmpresa" };
                var lista = CrearJGrid(UnidadMedidaBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdUnidadMedida.ToString(),
                    cell = new[]
                                {
                                    item.IdUnidadMedida.ToString(),
                                    item.Nombre,
                                    item.Abreviatura,
                                    item.Descripcion,
                                    item.IdEmpresa.ToString()                                    
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

            var unidadMedida = new UnidadMedida
            {
                Nombre = string.Empty,
                Abreviatura = string.Empty,
                Descripcion = string.Empty
            };

            PrepararDatos(ref unidadMedida,IdEmpresa);
            return PartialView("UnidadMedidaPanel", unidadMedida);
        }

        [HttpPost]
        public JsonResult Crear(UnidadMedida unidadMedida)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    unidadMedida.FechaCreacion = FechaCreacion;
                    unidadMedida.FechaModificacion = FechaModificacion;
                    unidadMedida.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    unidadMedida.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    UnidadMedidaBL.Instancia.Add(unidadMedida);

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

                var unidadMedida = UnidadMedidaBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref unidadMedida, IdEmpresa);

                return PartialView("UnidadMedidaPanel", unidadMedida);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(UnidadMedida unidadMedida)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var unidadMedidaOriginal = UnidadMedidaBL.Instancia.Single(unidadMedida.IdUnidadMedida);
                    unidadMedidaOriginal.Nombre = unidadMedida.Nombre;
                    unidadMedidaOriginal.IdEmpresa = unidadMedida.IdEmpresa;
                    unidadMedidaOriginal.Abreviatura = unidadMedida.Abreviatura;
                    unidadMedidaOriginal.Descripcion = unidadMedida.Descripcion;
                    unidadMedidaOriginal.FechaModificacion = FechaModificacion;
                    unidadMedidaOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    UnidadMedidaBL.Instancia.Update(unidadMedidaOriginal);

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

        private static void PrepararDatos(ref UnidadMedida unidadMedida, int idEmp)
        {
            var empresas = EmpresaBL.Instancia.Get();
            unidadMedida.Empresas = Utils.ConvertToComunList(empresas, "IdEmpresa", "RazonSocial");
            unidadMedida.IdEmpresa = idEmp;
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var unidadMedidaId = Convert.ToInt32(id);
                    UnidadMedidaBL.Instancia.Delete(unidadMedidaId);

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