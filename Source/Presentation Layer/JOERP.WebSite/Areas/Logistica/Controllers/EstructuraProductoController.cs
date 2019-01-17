
namespace JOERP.WebSite.Areas.Logistica.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Administracion.Models;
    using Business.Entity;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.JqGrid;

    public class EstructuraProductoController : BaseController
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
            return PartialView("EstructuraProductoListado");
        }

        public ActionResult Crear(string idParent, string idModulo)
        {
            int nivelTemp;
            string idParentTemp = idParent;
            ViewData["Accion"] = "Crear";
            if (Convert.ToInt32(idParent) == 0 && Convert.ToInt32(idModulo) == 0)
            {
                nivelTemp = 0;
            }
            else
            {
                if (Convert.ToInt32(idParent) != 0 && Convert.ToInt32(idModulo) != 0)
                {
                    nivelTemp = EstructuraProductoBL.Instancia.Single(Convert.ToInt32(idParent)).Nivel;
                }
                else
                {
                    nivelTemp = 1;
                    idParentTemp = idModulo;
                }
            }

            var entidad = new EstructuraProducto
            {
                Nivel = nivelTemp + 1,
                Nombre = string.Empty,
                IdParent = Convert.ToInt32(idParentTemp),
                IdEmpresa = Empresa.IdEmpresa,
            };

            PrepararDatos(ref entidad);
            return PartialView("EstructuraProductoPanel", entidad);
        }

        [HttpPost]
        public JsonResult Crear(EstructuraProducto estructuraProducto)
        {
            var jsonResponse = new JsonResponse { Success = false };

            if (ModelState.IsValid)
            {
                try
                {
                    estructuraProducto.FechaCreacion = FechaCreacion;
                    estructuraProducto.FechaModificacion = FechaModificacion;
                    estructuraProducto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    estructuraProducto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    EstructuraProductoBL.Instancia.Add(estructuraProducto);

                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se Proceso con exito.";
                    AlreadyPopulated = true;
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

                var estructuraProducto = EstructuraProductoBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref estructuraProducto);
                return PartialView("EstructuraProductoPanel", estructuraProducto);
            }
            catch(Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(EstructuraProducto estructuraProducto)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var entidadOriginal = EstructuraProductoBL.Instancia.Single(estructuraProducto.IdEstructuraProducto);
                    entidadOriginal.Nombre = estructuraProducto.Nombre;
                    entidadOriginal.IdEmpresa = estructuraProducto.IdEmpresa;
                    entidadOriginal.AccionAlterna = estructuraProducto.AccionAlterna;
                    entidadOriginal.FechaModificacion = FechaModificacion;
                    entidadOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    entidadOriginal.Nivel = estructuraProducto.Nivel;

                    EstructuraProductoBL.Instancia.Update(entidadOriginal);

                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se Proceso con exito";
                    AlreadyPopulated = true;
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

        public ActionResult Mostrar(string id)
        {
            var estructuraProducto = new EstructuraProducto();
            
            if (id != "undefined")
                estructuraProducto = EstructuraProductoBL.Instancia.Single(Convert.ToInt32(id));

            return PartialView("EstructuraProductoMostrar", estructuraProducto);
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid, int id)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "Descripcion", "IdParent", "IdEstructuraProducto" };
                var lista = CrearJGrid(DatoEstructuraBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdDatoEstructura.ToString(),
                    cell = new[]
                                {
                                    item.IdDatoEstructura.ToString(),
                                    item.Nombre,
                                    item.Descripcion,
                                    item.IdParent.HasValue ? DatoEstructuraBL.Instancia.Single(item.IdParent).Nombre : string.Empty,
                                    item.IdEstructuraProducto.ToString()
                                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
            return Json(jqgrid);
        }

        [HttpPost]
        public JsonResult GetTreeData()
        {
            if (AlreadyPopulated == false)
            {
                var rootNode = new JsTreeModel();
                rootNode.data = new JsTreeNodeData {title = "Estructura", icon = Utils.AbsoluteWebRoot + "Content/images/folder.ico"};
                rootNode.attr = new JsTreeAttribute {id = "0", rel = "0"};
                AlreadyPopulated = true;
                LoadModulos(rootNode);
                return Json(rootNode);
            }
            return null;
        }

        public void Cancelar()
        {
            AlreadyPopulated = true;
        }

        public ActionResult CrearDato(int id)
        {
            ViewData["Accion"] = "CrearDato";

            var datoEstructura = new DatoEstructura
            {
                IdEstructuraProducto = id,
                Nombre = string.Empty,
            };

            PrepararDatos(ref datoEstructura);
            return PartialView("DatoEstructuraPanel", datoEstructura);
        }

        [HttpPost]
        public JsonResult CrearDato(DatoEstructura datoEstructura)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    datoEstructura.FechaCreacion = FechaCreacion;
                    datoEstructura.FechaModificacion = FechaModificacion;
                    datoEstructura.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    datoEstructura.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    DatoEstructuraBL.Instancia.Add(datoEstructura);

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

        public ActionResult ModificarDato(string id)
        {
            try
            {
                ViewData["Accion"] = "ModificarDato";

                var datoEstructura = DatoEstructuraBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref datoEstructura);
                return PartialView("DatoEstructuraPanel", datoEstructura);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ModificarDato(DatoEstructura datoEstructura)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var datoEstructuraOriginal = DatoEstructuraBL.Instancia.Single(datoEstructura.IdDatoEstructura);
                    datoEstructuraOriginal.Nombre = datoEstructura.Nombre;
                    datoEstructuraOriginal.Descripcion = datoEstructura.Descripcion;
                    datoEstructuraOriginal.FechaModificacion = FechaModificacion;
                    datoEstructuraOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    datoEstructuraOriginal.IdLinea = datoEstructura.IdLinea;
                    datoEstructuraOriginal.IdParent = datoEstructura.IdParent;

                    DatoEstructuraBL.Instancia.Update(datoEstructuraOriginal);

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
                    var datoEstructuraId = Convert.ToInt32(id);
                    DatoEstructuraBL.Instancia.Delete(datoEstructuraId);

                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se quito el registro con exito.";
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = "No es posible Eliminar, hay datos registrados relacionados con esta Estructura de Producto. ";
                }
            }
            else
            {
                jsonResponse.Message = "No se pudo eliminar.";
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TiposPadre(int idEstructura)
        {
            if (idEstructura == 1)
            {
                return Content("");
            }
            else
            {
                idEstructura--;
                var tiposPadre = DatoEstructuraBL.Instancia.GetByIdEstructura(idEstructura);
                var select = "<select>";
                select += "<option value=''></option>";

                foreach (var padre in tiposPadre)
                {
                    select += string.Format("<option value='{0}'>{1}</option>", padre.IdDatoEstructura, padre.Nombre);
                }

                select += "</select>";

                return Content(select);
            }
        }

        #endregion Metodos públicos

        #region Metodos privados

        private void LoadFormularios(int idParent, JsTreeModel moduloNode)
        {
            try
            {
                List<EstructuraProducto> formularios =
                    EstructuraProductoBL.Instancia.GetByIdParent(idParent).OrderBy(m => m.Nombre).ToList();

                LoadArbol(formularios, 1, moduloNode); //el IdModulo = 1
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void LoadArbol(IEnumerable<EstructuraProducto> estructuras, int idModulo, JsTreeModel nodo)
        {
            try
            {
                if (nodo.children == null)
                {
                    nodo.children = new List<JsTreeModel>();
                }

                foreach (var f in estructuras)
                {
                    var node = new JsTreeModel
                    {
                        attr = new JsTreeAttribute { id = f.IdEstructuraProducto.ToString(), rel = idModulo.ToString() },
                        data = new JsTreeNodeData { title = f.Nombre },
                    };
                    var forms = EstructuraProductoBL.Instancia.GetByIdParent(f.IdEstructuraProducto).OrderBy(m => m.Nombre).ToList();
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
            var modulos = EstructuraProductoBL.Instancia.GetByNivel(1).OrderBy(m => m.Nombre).ToList();
            foreach (var m in modulos)
            {
                var modulo = EstructuraProductoBL.Instancia.Single(m.IdEstructuraProducto);
                if (modulo == null) continue;
                var moduloNode = new JsTreeModel
                {
                    attr = new JsTreeAttribute { id = modulo.IdEstructuraProducto.ToString(), rel = "1" },
                    data = new JsTreeNodeData { title = modulo.Nombre, icon = Utils.AbsoluteWebRoot + "Content/images/form.ico" },
                };
                LoadFormularios(modulo.IdEstructuraProducto, moduloNode);
                node.children.Add(moduloNode);
            }
        }

        private static void PrepararDatos(ref EstructuraProducto estructuraProducto)
        {
            var parents = FormularioBL.Instancia.Get();
            estructuraProducto.Parents = Utils.ConvertToComunList(parents, "IdFormulario", "Nombre");
            var operaciones = OperacionBL.Instancia.Get();
            estructuraProducto.Operaciones = Utils.ConvertToComunList(operaciones, "IdOperacion", "Nombre");
        }

        private static void PrepararDatos(ref DatoEstructura datoEstructura)
        {
            datoEstructura.DatosEstructuras = new List<DatoEstructura>();

            if (datoEstructura.IdEstructuraProducto == 2) // Sub Linea
            {
                var lineas = DatoEstructuraBL.Instancia.GetByIdEstructura(1);
                datoEstructura.DatosEstructuras = lineas;
            }
            else if (datoEstructura.IdEstructuraProducto == 3) // Categoria
            {
                var lineas = DatoEstructuraBL.Instancia.GetByIdEstructura(1);
                datoEstructura.DatosEstructuras = lineas;

                if (datoEstructura.IdParent != null)
                {
                    var subLinea = DatoEstructuraBL.Instancia.Single(datoEstructura.IdParent.Value);
                    datoEstructura.IdLinea = subLinea.IdParent.Value;
                }
            }
        }

        public JsonResult ListarSubLineas(int idLinea)
        {
            var subLineas = DatoEstructuraBL.Instancia.GetByIdParent(idLinea);
            var listItems = Utils.ConvertToListItem(subLineas, "IdDatoEstructura", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        #endregion Metodos privados
    }
}