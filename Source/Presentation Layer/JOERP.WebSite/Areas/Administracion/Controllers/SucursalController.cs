
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

    public class SucursalController : BaseController
    {
        public ActionResult Index(string id)
        {
            return PartialView("SucursalListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "Empresa", "EsPrincipal", "Estado" };
                var lista = CrearJGrid(SucursalBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                                {
                                    id = item.IdSucursal.ToString(),
                                    cell = new[]
                                                {
                                                    item.IdSucursal.ToString(),
                                                    item.NombreEmpresa,
                                                    item.Nombre,
                                                    item.Telefono,
                                                    item.EsPrincipal ? "SI" : "NO",
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

            var sucursal = new Sucursal
                              {
                                  Nombre = string.Empty,
                                  Estado = (int) TipoEstado.Activo
                              };

            PrepararDatos(ref sucursal);
            return PartialView("SucursalInformacion", sucursal);
        }

        [HttpPost]
        public JsonResult Crear(Sucursal sucursal)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    sucursal.IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(sucursal.IdDepartamento, sucursal.IdProvincia, sucursal.IdUbigeo);
                    sucursal.FechaCreacion = FechaCreacion;
                    sucursal.FechaModificacion = FechaModificacion;
                    sucursal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    sucursal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    SucursalBL.Instancia.Add(sucursal);

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

                var sucursal = SucursalBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref sucursal);

                return PartialView("SucursalInformacion", sucursal);
            }
            catch(Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Sucursal sucursal)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var sucursalOriginal = SucursalBL.Instancia.Single(sucursal.IdSucursal);
                    sucursalOriginal.Nombre = sucursal.Nombre;
                    sucursalOriginal.IdEmpresa = sucursal.IdEmpresa;
                    sucursalOriginal.Email = sucursal.Email;
                    sucursalOriginal.IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(sucursal.IdDepartamento,
                                                                              sucursal.IdProvincia,
                                                                              sucursal.IdUbigeo);
                    sucursalOriginal.EsPrincipal = sucursal.EsPrincipal;
                    sucursalOriginal.Estado = sucursal.Estado;
                    sucursalOriginal.Direccion = sucursal.Direccion;
                    sucursalOriginal.Telefono = sucursal.Telefono;
                    sucursalOriginal.FechaModificacion = FechaModificacion;
                    sucursalOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    SucursalBL.Instancia.Update(sucursalOriginal);

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

        private static void PrepararDatos(ref Sucursal sucursal)
        {
            var empresas = EmpresaBL.Instancia.Get();
            sucursal.Estados = Utils.EnumToList<TipoEstado>();
            sucursal.Empresas = Utils.ConvertToComunList(empresas, "IdEmpresa", "RazonSocial");
            sucursal.Departamentos = UbigeoBL.Instancia.GetAllDepartamentos();
            sucursal.Ubigeo = UbigeoBL.Instancia.Single(sucursal.IdUbigeo);
            if (sucursal.Ubigeo== null) return;
            sucursal.IdDepartamento = sucursal.Ubigeo.IdDepartamento;
            sucursal.IdProvincia = sucursal.Ubigeo.IdProvincia;
            sucursal.IdUbigeo = sucursal.Ubigeo.IdDistrito;
        }

        public JsonResult ListarSucursales(int? idEmpresa)
        {
            if (idEmpresa == null) idEmpresa = 0;
            var sucursales = SucursalBL.Instancia.GetByEmpresa(idEmpresa.Value > 0 ? idEmpresa.Value : IdEmpresa);
            var listItems = Utils.ConvertToListItem(sucursales, "IdSucursal", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
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
                    SucursalBL.Instancia.Delete(sucursalId);

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
