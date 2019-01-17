
namespace JOERP.WebSite.Areas.Logistica.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.JqGrid;
    using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

    public class ComponenteController : BaseController
    {
        public IList<ProductoComponente> Componentes
        {
            get { return (IList<ProductoComponente>)Session["ComponenteSesion"]; }
            set { Session["ComponenteSesion"] = value; }
        }

        public ActionResult Index()
        {
            return PartialView("ComponenteListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Codigo", "CodigoAlterno", "Nombre", "Descripcion","TipoProducto" };
                var lista = CrearJGrid(ProductoBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdProducto.ToString(),
                    cell = new[]
                                {
                                    item.IdProducto.ToString(),
                                    item.Codigo,
                                    item.CodigoAlterno,
                                    item.Nombre,
                                    item.Descripcion.ToString(),
                                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }

            return Json(jqgrid);
        }

        public ActionResult Modificar(string id)
        {
            try
            {
                ViewData["Accion"] = "Modificar";

                var productoComponente = ProductoBL.Instancia.Single(Convert.ToInt32(id));
                Componentes = ProductoComponenteBL.Instancia.GetByIdProducto(productoComponente.IdProducto);

                return PartialView("ComponentePanel", productoComponente);
            }
            catch(Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Producto producto)
        {
            var jsonResponse = new JsonResponse();

            if (Componentes.Count > 0)
            {
                try
                {
                    foreach (var componente in Componentes)
                    {
                        componente.IdProducto = producto.IdProducto;
                    }

                    ProductoBL.Instancia.GuardarComponentes(Componentes.ToList());

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
                jsonResponse.Message = "Por favor ingrese los componentes antes de grabar.";
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarProducto(string term)
        {
            var productos = ProductoBL.Instancia.GetByNombre(term);
            var resultado = new List<object>();

            productos.ForEach(p => resultado.Add(new
            {
                id = p.IdProducto,
                label = p.Nombre,
                value = p.Nombre
            }));

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarComponentes()
        {
            var componentes = from c in Componentes
                              select new
                                         {
                                             IdProductoComponente = c.IdProductoComponente,
                                             Codigo = c.Presentacion.Producto.Codigo,
                                             Producto = c.Presentacion.Producto.Nombre,
                                             Presentacion = c.Presentacion.Nombre,
                                             Cantidad = c.Cantidad
                                         };
            return Json(componentes);
        }

        public JsonResult AgregarComponente(int idProducto, int idPresentacion, int cantidad)
        {
            var jsonResponse = new JsonResponse();

            try
            {
                var producto = ProductoBL.Instancia.Single(idProducto);
                var presentacion = PresentacionBL.Instancia.Single(idPresentacion);

                var componente = new ProductoComponente
                                     {
                                         IdPresentacion = idPresentacion,
                                         Cantidad = cantidad,
                                         UsuarioCreacion = UsuarioActual.IdEmpleado,
                                         FechaCreacion = FechaCreacion,
                                         UsuarioModificacion = UsuarioActual.IdEmpleado,
                                         FechaModificacion = FechaModificacion,
                                         Presentacion = new Presentacion
                                                            {
                                                                IdPresentacion = idPresentacion,
                                                                Nombre = presentacion.Nombre,
                                                                Producto = new Producto
                                                                               {
                                                                                   IdProducto = idProducto,
                                                                                   Codigo = producto.Codigo,
                                                                                   Nombre = producto.Nombre
                                                                               },
                                                            }
                                     };

                if (componente.IdProductoComponente == 0)
                {
                    componente.IdProductoComponente = Componentes.Count == 0 ? 1 : Componentes.Max(p => p.IdProductoComponente) + 1;
                }

                Componentes.Add(componente);

                jsonResponse.Success = true;
                jsonResponse.Message = "Se Proceso con exito.";
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarPresentciones(int idProducto)
        {
            var presentaciones = PresentacionBL.Instancia.GetByIdProducto(idProducto);
            var listItems = Utils.ConvertToListItem(presentaciones, "IdPresentacion", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult QuitarComponente(int idComponente)
        {
            var jsonResponse = new JsonResponse();

            try
            {
                var componente = Componentes.FirstOrDefault(p => p.IdProductoComponente == idComponente);
                Componentes.Remove(componente);

                jsonResponse.Success = true;
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }
    }
}