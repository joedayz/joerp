
namespace JOERP.WebSite.Areas.Administracion.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.Enums;
    using Helpers.JqGrid;

    public class UsuarioController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("UsuarioListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "Username", "NombreRol", "Estado" };
                var lista = CrearJGrid(UsuarioBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdEmpleado.ToString(),
                    cell = new[]
                                {
                                    item.IdEmpleado.ToString(),
                                    item.NombreEmpleado,
                                    item.Username,
                                    item.NombreRol,
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

            var usuario = new Usuario
            {
                Estado = (int)TipoEstado.Activo, 
                UsuarioSucursal  = new List<UsuarioSucursal>(),
            };

            PrepararDatos(ref usuario);
            return PartialView("UsuarioPanel", usuario);
        }

        [HttpPost]
        public JsonResult Crear(Usuario usuario)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    usuario.UsuarioSucursal= new List<UsuarioSucursal>();
                    foreach (var sucursal in usuario.Sucursales)
                    {
                        if (sucursal.Seleccionado)
                            usuario.UsuarioSucursal.Add(new UsuarioSucursal
                            {
                                IdEmpleado = usuario.IdEmpleado,
                                IdSucursal = sucursal.IdSucursal
                            });
                    }
                    usuario.Password = Encriptador.Encriptar(usuario.Password);
                    usuario.FechaCreacion = FechaCreacion;
                    usuario.FechaModiifacion= FechaModificacion;
                    usuario.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    usuario.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    UsuarioBL.Instancia.Add(usuario);

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

                var usuario = UsuarioBL.Instancia.Single(Convert.ToInt32(id));
                usuario.Password = Encriptador.Desencriptar(usuario.Password);
                usuario.IdCargo = usuario.Empleado.IdCargo;
                usuario.IdEmpleado = usuario.Empleado.IdEmpleado;
                usuario.UsuarioSucursal = UsuarioSucursalBL.Instancia.GetByUsuario(usuario.IdEmpleado);
                PrepararDatos(ref usuario);

                return PartialView("UsuarioPanel", usuario);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Usuario usuario)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioOriginal = UsuarioBL.Instancia.Single(usuario.IdEmpleado);
                    usuarioOriginal.Username = usuario.Username;
                    usuarioOriginal.Password = Encriptador.Encriptar(usuario.Password);
                    usuarioOriginal.IdRol = usuario.IdRol;
                    usuarioOriginal.UsuarioSucursal = new List<UsuarioSucursal>();
                    usuarioOriginal.Estado = usuario.Estado;

                    foreach (var sucursal in usuario.Sucursales)
                    {
                        if (sucursal.Seleccionado)
                            usuarioOriginal.UsuarioSucursal.Add(new UsuarioSucursal
                            {
                                IdSucursal = sucursal.IdSucursal,
                                IdEmpleado = usuario.IdEmpleado
                            });
                    }
                    usuarioOriginal.FechaModiifacion= FechaModificacion;
                    usuarioOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    UsuarioBL.Instancia.Update(usuarioOriginal);
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

        private  void PrepararDatos(ref Usuario usuario)
        {
            usuario.Estados = Utils.EnumToList<TipoEstado>();
            usuario.Cargos = CargoBL.Instancia.GetByEmpresa(IdEmpresa);
            usuario.Cargos.Add(new Cargo { IdCargo = 0, Nombre = "-- Seleccionar --" });
            usuario.Empleados = EmpleadoBL.Instancia.GetAll();
            usuario.Roles= RolBL.Instancia.Get();
            usuario.Sucursales = new ObservableCollection<UsuarioSucursal>();
            foreach (var tipo in SucursalBL.Instancia.Get())
            {
                var funcion = usuario.UsuarioSucursal.SingleOrDefault(p => p.IdSucursal== tipo.IdSucursal);
                var seleccionado = funcion != null;
                usuario.Sucursales.Add(new UsuarioSucursal
                                           {
                                               IdSucursal = tipo.IdSucursal,
                                               Seleccionado = seleccionado,
                                               NombreSucursal = SucursalBL.Instancia.Single(tipo.IdSucursal).Nombre
                                           });
            }
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioId = Convert.ToInt32(id);
                    UsuarioBL.Instancia.Delete(usuarioId);

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
