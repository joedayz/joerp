
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

    public class EmpresaController : BaseController
    {
        public ActionResult Index(string id)
        {
            return PartialView("EmpresaListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "RazonSocial", "RUC", "Telefono", "Direccion", "Estado" };
                var lista = CrearJGrid(EmpresaBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdEmpresa.ToString(),
                    cell = new[]
                                {
                                    item.IdEmpresa.ToString(),
                                    item.RazonSocial,
                                    item.RUC,
                                    item.Telefono,
                                    item.Direccion,
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

            var entidad = new Empresa
            {
                FechaInscripcion = FechaCreacion,
                Estado = (int)TipoEstado.Activo
            };

            PrepararDatos(ref entidad);
            return PartialView("EmpresaPanel", entidad);
        }

        [HttpPost]
        public JsonResult Crear(Empresa entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    entidad.IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(entidad.IdDepartamento, entidad.IdProvincia, entidad.IdUbigeo);
                    entidad.FechaCreacion = FechaCreacion;
                    entidad.FechaModificacion = FechaModificacion;
                    entidad.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    entidad.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    
                    EmpresaBL.Instancia.Add(entidad);

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

                var entidad = EmpresaBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref entidad);

                return PartialView("EmpresaPanel", entidad);
            }
            catch
            {
                return MensajeError();
            }
        }

        [HttpPost]
        public JsonResult Modificar(Empresa entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var entidadOriginal = EmpresaBL.Instancia.Single(entidad.IdEmpresa);
                    entidadOriginal.RazonSocial = entidad.RazonSocial;
                    entidadOriginal.RUC = entidad.RUC;
                    entidadOriginal.Direccion = entidad.Direccion;
                    entidadOriginal.Estado = entidad.Estado;
                    entidadOriginal.CodigoPostal = entidad.CodigoPostal;
                    entidadOriginal.IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(entidad.IdDepartamento, entidad.IdProvincia, entidad.IdUbigeo);
                    entidadOriginal.ActividadEconomica = entidad.ActividadEconomica;
                    entidadOriginal.Telefono = entidad.Telefono;
                    entidadOriginal.Celular = entidad.Celular;
                    entidadOriginal.Fax = entidad.Fax;
                    entidadOriginal.PaginaWeb = entidad.PaginaWeb;
                    entidadOriginal.CorreoElectronico = entidad.CorreoElectronico;
                    entidadOriginal.TipoContribuyente = entidad.TipoContribuyente;
                    entidadOriginal.FechaInscripcion = FechaCreacion;
                    entidadOriginal.FechaActividades = FechaSistema;

                    entidadOriginal.FechaModificacion = FechaModificacion;
                    entidadOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    EmpresaBL.Instancia.Update(entidadOriginal);
                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se Proceso con exito";
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                    //jsonResponse.Message = "Ocurrio un error, por favor intente de nuevo o mas tarde.";
                }
            }
            else
            {
                jsonResponse.Message = "Por favor ingrese todos los campos requeridos";
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        private static void PrepararDatos(ref Empresa entidad)
        {
            entidad.EsPercepcion = true;
            entidad.Estados = Utils.EnumToList<TipoEstado>();
            entidad.Departamentos = UbigeoBL.Instancia.GetAllDepartamentos();
            entidad.Departamentos.Add(new Comun {Id = -1, Nombre = " -- Seleccionar -- "});
            entidad.Actividades = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.ActividadEconomica);
            entidad.Actividades.Add(new Comun{Id = -1,Nombre = " -- Seleccionar -- "});
            entidad.Contribuyentes = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoContribuyente);
            entidad.Contribuyentes.Add(new Comun {Id = -1, Nombre = " -- Seleccionar -- "});
            entidad.Ubigeo = UbigeoBL.Instancia.Single(entidad.IdUbigeo);
            if (entidad.Ubigeo == null) return;
            entidad.IdDepartamento = entidad.Ubigeo.IdDepartamento;
            entidad.IdProvincia = entidad.Ubigeo.IdProvincia;
            entidad.IdUbigeo = entidad.Ubigeo.IdDistrito;
        }
        
        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var empresaId = Convert.ToInt32(id);
                    EmpresaBL.Instancia.Delete(empresaId);

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
                jsonResponse.Message = "No se puedo eliminar.";
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }
    }
}