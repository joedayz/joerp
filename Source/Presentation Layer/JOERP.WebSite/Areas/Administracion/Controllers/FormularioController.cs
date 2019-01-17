
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

    public class FormularioController : BaseController
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
            return PartialView("FormularioListado");
        }

        public ActionResult Crear(string idParent, string idModulo)
        {
            ViewData["Accion"] = "Crear";

            var entidad = new Formulario
            {
                Estado = (int)TipoEstado.Activo,
                Nombre=string.Empty,
                IdParent = Convert.ToInt32(idParent),
                IdModulo = Convert.ToInt32(idModulo),
            };

            PrepararDatos(ref entidad);
            return PartialView("FormularioPanel", entidad);
        }

        [HttpPost]
        public JsonResult Crear(Formulario entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    entidad.IdOperacion = (entidad.IdOperacion == 0) ? null : entidad.IdOperacion;
                    FormularioBL.Instancia.Add(entidad);
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

                var entidad = FormularioBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref entidad);
                return PartialView("FormularioPanel", entidad);
            }
            catch(Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Formulario entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var entidadOriginal = FormularioBL.Instancia.Single(entidad.IdFormulario);
                    entidadOriginal.Nombre = entidad.Nombre;
                    entidadOriginal.Descripcion = entidad.Descripcion;
                    entidadOriginal.Assembly = entidad.Assembly;
                    entidadOriginal.Direccion = entidad.Direccion;
                    entidadOriginal.IdParent = entidad.IdParent;
                    entidadOriginal.Nivel = entidad.Nivel;
                    entidadOriginal.IdModulo = entidad.IdModulo;
                    entidadOriginal.Estado = entidad.Estado;
                    if(entidad.IdOperacion!=0)
                        entidadOriginal.IdOperacion = entidad.IdOperacion;
                    entidadOriginal.Orden = entidad.Orden;

                    FormularioBL.Instancia.Update(entidadOriginal);

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

        public ActionResult Mostrar(string id)
        {
            try
            {
                var idform = Convert.ToInt32(id);
                var formulario = FormularioBL.Instancia.Single(idform);
                PrepararDatos(ref formulario);
                return PartialView("FormularioMostrar", formulario);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }
        
        [HttpPost]
        public JsonResult GetTreeData()
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
            return null;
        }

        public void Cancelar()
        {
            AlreadyPopulated = false;
        }

        #endregion

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

        private void LoadArbol(IEnumerable<Formulario> formularios, int idModulo, JsTreeModel nodo)
        {
            try
            {
                if (nodo.children == null)
                {
                    nodo.children = new List<JsTreeModel>();
                }

                foreach (var f in formularios)
                {
                    var node = new JsTreeModel
                                   {
                                       attr =
                                           new JsTreeAttribute
                                               {id = f.IdFormulario.ToString(), rel = idModulo.ToString()},
                                       data = new JsTreeNodeData {title = f.Nombre},
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
            var modulos = ItemTablaBL.Instancia.GetByTabla((int) TipoTabla.Modulo).OrderBy(m => m.Nombre).ToList();
            foreach (var m in modulos)
            {
                var modulo = ItemTablaBL.Instancia.Single((int) TipoTabla.Modulo, m.IdItemTabla);
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

        private static void PrepararDatos(ref Formulario entidad)
        {
            var parents = FormularioBL.Instancia.Get();
            entidad.Parents = Utils.ConvertToComunList(parents, "IdFormulario", "Nombre");
            entidad.Estados = Utils.EnumToList<TipoEstado>();
            entidad.Modulos = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.Modulo);
            var operaciones = OperacionBL.Instancia.Get();
            entidad.Operaciones = Utils.ConvertToComunList(operaciones,"IdOperacion","Nombre");
        }

        #endregion
    }
}
