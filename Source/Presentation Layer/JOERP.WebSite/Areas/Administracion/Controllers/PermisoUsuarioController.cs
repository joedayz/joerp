
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
    using Models;

    public class PermisoUsuarioController : BaseController
    {
        #region Variables

        public bool AlreadyPopulated
        {
            get { return Session["AlreadyPopulated"] != null && (bool)Session["AlreadyPopulated"]; }

            set { Session["AlreadyPopulated"] = value; }
        }

        #endregion Variables

        #region Metodos públicos

        public ActionResult Index()
        {
            AlreadyPopulated = false;
            return PartialView("PermisoUsuarioListado");
        }

        public virtual JsonResult ListarRoles()
        {
            var roles = RolBL.Instancia.Get();
            var listItems = Utils.ConvertToListItem(roles, "IdRol", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult MostrarF(string id)
        {
            try
            {
                var cadena = id.Split('-');
                var idFormulario = Convert.ToInt32(cadena[1]);
                var rol = RolBL.Instancia.Single(Convert.ToInt32(cadena[0]));
                var usuarios = UsuarioBL.Instancia.GetUsersInRol(rol.Nombre);
                var tiposPermiso = Utils.EnumToList<TipoPermiso>().OrderBy(p => p.Id).ToList();

                foreach (var user in usuarios)
                {
                    user.Permisos = PermisoUsuarioBL.Instancia.GetFiltered(user.IdEmpleado, idFormulario);

                    foreach (var permiso in user.Permisos)
                    {
                        permiso.Seleccionado = true;
                    }

                    foreach (var permiso in tiposPermiso)
                    {
                        if (!user.Permisos.Any(p => p.IdTipoPermiso == permiso.Id))
                        {
                            user.Permisos.Add(new PermisoUsuario
                            {
                                IdFormulario = idFormulario,
                                IdEmpleado =

                                    user.IdEmpleado,
                                IdTipoPermiso = permiso.Id
                            });
                        }
                    }

                    user.Permisos = user.Permisos.OrderBy(p => p.IdTipoPermiso).ToList();
                }

                ViewData["IdFormulario"] = idFormulario;
                ViewData["TiposPermiso"] = tiposPermiso;
                return PartialView("PermisoUsuarioMostrar", usuarios);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }
      
        public virtual ActionResult Mostrar(string id)
        {
            try
            {
                var idFormulario = Convert.ToInt32(id);
                var usuarios = UsuarioBL.Instancia.Get();
                var tiposPermiso = Utils.EnumToList<TipoPermiso>().OrderBy(p => p.Id).ToList();

                foreach (var user in usuarios)
                {
                    user.Permisos = PermisoUsuarioBL.Instancia.GetFiltered(user.IdEmpleado, idFormulario);

                    foreach (var permiso in user.Permisos)
                    {
                        permiso.Seleccionado = true;
                    }

                    foreach (var permiso in tiposPermiso)
                    {
                        if (!user.Permisos.Any(p => p.IdTipoPermiso == permiso.Id))
                        {
                            user.Permisos.Add(new PermisoUsuario
                            {
                                IdFormulario = idFormulario,
                                IdEmpleado =

                                    user.IdEmpleado,
                                IdTipoPermiso = permiso.Id
                            });
                        }
                    }

                    user.Permisos = user.Permisos.OrderBy(p => p.IdTipoPermiso).ToList();
                }

                ViewData["IdFormulario"] = idFormulario;
                ViewData["TiposPermiso"] = tiposPermiso;
                return PartialView("PermisoUsuarioMostrar", usuarios);
            }
            catch(Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }
      
        [HttpPost]
        public JsonResult GuardarPermisos(string id, IList<Usuario> usuarios)
        {
            var jsonResponse = new JsonResponse();

            try
            {
                var idFormulario = Convert.ToInt32(id);
                var permisosUsuario= new List<PermisoUsuario>();

                foreach (var user in usuarios)
                {
                    foreach (var permiso in user.Permisos)
                    {
                        if (!permiso.Seleccionado) continue;

                        var permisoRol = new PermisoUsuario
                        {
                            IdEmpleado = user.IdEmpleado,
                            IdFormulario = idFormulario,
                            IdTipoPermiso = permiso.IdTipoPermiso,
                        };

                        permisosUsuario.Add(permisoRol);
                    }
                }

                PermisoUsuarioBL.Instancia.Guardar(idFormulario, permisosUsuario);

                jsonResponse.Success = true;
                jsonResponse.Message = "Se Proceso con exito.";
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual JsonResult GetTreeData()
        {
            if (AlreadyPopulated == false)
            {
                var rootNode = new JsTreeModel();
                rootNode.data = new JsTreeNodeData { title = "Módulos", icon = Utils.AbsoluteWebRoot + "Content/images/folder.ico" };
                rootNode.attr = new JsTreeAttribute { id = "0", rel = "0" };
                AlreadyPopulated = true;
                LoadModulos(rootNode);
                return Json(rootNode);
            }
            else
            {
                return null;
            }
        }

        public virtual void Cancelar()
        {
            AlreadyPopulated = false;
        }

        #endregion Metodos públicos

        #region Metodos privados

        private void LoadFormularios(int idModulo, JsTreeModel moduloNode)
        {
            try
            {
                var formularios = FormularioBL.Instancia.FormulariosLista(idModulo, null).OrderBy(m => m.Nombre).ToList();

                LoadArbol(formularios, idModulo, moduloNode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void LoadArbol(List<Formulario> formularios, int idModulo, JsTreeModel nodo)
        {
            try
            {
                if (nodo.children == null)
                {
                    nodo.children = new List<JsTreeModel>();
                }

                foreach (Formulario f in formularios)
                {
                    var node = new JsTreeModel
                    {
                        attr = new JsTreeAttribute { id = f.IdFormulario.ToString(), rel = idModulo.ToString() },
                        data = new JsTreeNodeData { title = f.Nombre },
                    };
                    var forms = FormularioBL.Instancia.FormulariosLista(idModulo, f.IdFormulario).OrderBy(m => m.Nombre).ToList();
                    node.data.icon = (forms.Count == 0) ? Utils.AbsoluteWebRoot + "Content/images/paper.ico" : Utils.AbsoluteWebRoot + "Content/images/form.ico";
                    nodo.children.Add(node);
                    LoadArbol(forms, idModulo, node);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void LoadModulos(JsTreeModel node)
        {
            node.children = new List<JsTreeModel>();
            var modulos = ItemTablaBL.Instancia.GetByTabla((int)TipoTabla.Modulo).OrderBy(m => m.Nombre).ToList();
            foreach (ItemTabla m in modulos)
            {
                var modulo = ItemTablaBL.Instancia.Single((int)TipoTabla.Modulo, m.IdItemTabla);
                if (modulo == null) continue;
                var moduloNode = new JsTreeModel
                {
                    attr = new JsTreeAttribute { id = "0", rel = modulo.IdItemTabla.ToString() },
                    data = new JsTreeNodeData { title = modulo.Nombre, icon = Utils.AbsoluteWebRoot + "Content/images/form.ico" },
                };
                LoadFormularios(modulo.IdItemTabla, moduloNode);
                node.children.Add(moduloNode);
            }
        }
       
        #endregion Metodos privados
    }
}
