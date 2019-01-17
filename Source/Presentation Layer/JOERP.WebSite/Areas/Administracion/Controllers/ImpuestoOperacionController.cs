
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

    public class ImpuestoOperacionController : BaseController
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
            return PartialView("ImpuestoOperacionListado");
        }

        public ActionResult Mostrar(string id)
        {
            var impuestos = ImpuestoBL.Instancia.Get();
            
            var impuestosOperacion = new List<OperacionImpuesto>();

            try
            {
                var formulario = FormularioBL.Instancia.Single(Convert.ToInt32(id));

                if (formulario.IdOperacion.HasValue)
                {
                    var temp = OperacionImpuestoBL.Instancia.GetByOperacion(formulario.IdOperacion.Value);

                    foreach (var impuesto in impuestos)
                    {
                        var existe = false;
                        foreach (var item in temp)
                        {
                            if (item.IdImpuesto == impuesto.IdImpuesto) 
                                existe =true;
                        }

                        var impuestoOperacion = temp.FirstOrDefault(p => p.IdImpuesto == impuesto.IdImpuesto);
                        
                        if (impuestoOperacion == null)
                        {
                            impuestosOperacion.Add(new OperacionImpuesto
                            {
                                Impuesto = impuesto,
                                IdImpuesto = impuesto.IdImpuesto,
                                IdOperacion = formulario.IdOperacion.Value
                            });
                        }
                        else
                        {
                            impuestosOperacion.Add(new OperacionImpuesto
                            {
                                Impuesto = impuesto,
                                IdImpuesto = impuesto.IdImpuesto,
                                IdOperacion = formulario.IdOperacion.Value,
                                Seleccionado = existe,
                                Orden = impuestoOperacion.Orden
                            });
                        }
                    }

                    ViewData["IdOperacion"] = formulario.IdOperacion;
                }
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }

            return PartialView("ImpuestoOperacionMostrar", impuestosOperacion);
        }

        [HttpPost]
        public JsonResult GuardarImpuestos(string id, IList<OperacionImpuesto> operaciones)
        {
            var jsonResponse = new JsonResponse();

            try
            {
                var idOperacion = Convert.ToInt32(id);
                var impuestosOperacion = new List<OperacionImpuesto>();

                foreach (var ope in operaciones)
                {
                    if (ope.Seleccionado)
                    {

                        var operacionImpuesto = new OperacionImpuesto
                                            {
                                                IdOperacion = idOperacion,
                                                IdImpuesto = ope.IdImpuesto,
                                                Orden = ope.Orden,
                                                FechaCreacion = FechaCreacion,
                                                FechaModificacion = FechaModificacion,
                                                UsuarioCreacion = UsuarioActual.IdEmpleado,
                                                UsuarioModificacion = UsuarioActual.IdEmpleado
                                            };

                        impuestosOperacion.Add(operacionImpuesto);
                    }
                    
                }

                OperacionImpuestoBL.Instancia.Guardar(idOperacion, impuestosOperacion);

                jsonResponse.Success = true;
                jsonResponse.Message = "Se Proceso con exito.";
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
                //jsonResponse.Message = "Por favor ingrese todos los campos requeridos";
            }

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetTreeData()
        {
            var rootNode = new JsTreeModel();
            rootNode.data = new JsTreeNodeData { title = "Módulos", icon = Utils.AbsoluteWebRoot + "Content/images/folder.ico" };
            rootNode.attr = new JsTreeAttribute { id = "0", rel = "0" };
            AlreadyPopulated = true;
            LoadModulos(rootNode);
            return Json(rootNode);
        }

        public void Cancelar()
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
                    if (String.Compare(f.Nombre, "Operaciones", StringComparison.Ordinal) == 0)
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
                    if (f.IdParent != null)
                    {
                        var parent = FormularioBL.Instancia.Single((int) f.IdParent);
                        if (String.Compare(parent.Nombre, "Operaciones", StringComparison.Ordinal) == 0)
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
            foreach (var m in modulos)
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