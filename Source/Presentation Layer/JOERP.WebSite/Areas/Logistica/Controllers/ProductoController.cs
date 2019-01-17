
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
    using Helpers.Enums;
    using Helpers.JqGrid;

    public class ProductoController : BaseController
    {
        public IList<Presentacion> Presentaciones
        {
            get { return (IList<Presentacion>)Session["PresentacionSesion"]; }
            set { Session["PresentacionSesion"] = value; }
        }

        public ActionResult Index()
        {
            return PartialView("ProductoListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Codigo", "CodigoAlterno", "Nombre", "Descripcion","TipoProducto"};
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
                                    Enum.GetName(typeof(TipoProducto), item.TipoProducto),
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

            var producto = new Producto();
            Presentaciones = new List<Presentacion>();

            PrepararDatos(ref producto);
            return PartialView("ProductoPanel", producto);
        }

        [HttpPost]
        public JsonResult Crear(Producto producto)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    if (producto.DatoEstructuraProducto == null)
                        producto.DatoEstructuraProducto = new List<DatoEstructuraProducto>();
                    else producto.DatoEstructuraProducto.Clear();

                    producto.DatoEstructuraProducto.Add(new DatoEstructuraProducto {IdDatoEstructura = producto.IdDatoEstructura});

                    if (producto.Presentacion == null)
                        producto.Presentacion = new List<Presentacion>();
                    else producto.Presentacion.Clear();

                    foreach (var presentacion in Presentaciones)
                    {
                        producto.Presentacion.Add(
                            new Presentacion
                            {
                                IdProducto = presentacion.IdProducto,
                                Equivalencia = presentacion.Equivalencia,
                                EsBase = presentacion.EsBase,
                                IdUnidadMedida = presentacion.IdUnidadMedida,
                                Nombre = presentacion.Nombre,
                                Peso = presentacion.Peso,
                                UsuarioCreacion = presentacion.UsuarioCreacion,
                                FechaCreacion = presentacion.FechaCreacion,
                                UsuarioModificacion = presentacion.UsuarioModificacion,
                                FechaModificacion = presentacion.FechaModificacion
                            });
                    }

                    producto.IdEmpresa = IdEmpresa;
                    producto.FechaCreacion = FechaCreacion;
                    producto.FechaModificacion = FechaModificacion;
                    producto.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    producto.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    ProductoBL.Instancia.Add(producto);

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

                var producto = ProductoBL.Instancia.Single(Convert.ToInt32(id));
                Presentaciones = PresentacionBL.Instancia.GetByIdProducto(producto.IdProducto);

                PrepararDatos(ref producto);

                return PartialView("ProductoPanel", producto);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }
    
        [HttpPost]
        public JsonResult Modificar(Producto producto)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var productoOriginal = ProductoBL.Instancia.Single(producto.IdProducto);
                    productoOriginal.Nombre = producto.Nombre;
                    productoOriginal.DescripcionLarga = producto.DescripcionLarga;
                    productoOriginal.Descripcion = producto.Descripcion;
                    productoOriginal.Estado = producto.Estado;
                    productoOriginal.Codigo = producto.Codigo;
                    productoOriginal.CodigoBarra = producto.CodigoBarra;
                    productoOriginal.CodigoAlterno = producto.CodigoAlterno;
                    productoOriginal.EsAfecto = producto.EsAfecto;
                    productoOriginal.EsExonerado = producto.EsExonerado;
                    productoOriginal.StockMaximo = producto.StockMaximo;
                    productoOriginal.StockMinimo = producto.StockMinimo;
                    productoOriginal.TipoClasificacion = producto.TipoClasificacion;
                    productoOriginal.TipoProducto = producto.TipoProducto;
                    productoOriginal.FechaModificacion = FechaModificacion;
                    productoOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    if (productoOriginal.DatoEstructuraProducto == null)
                        productoOriginal.DatoEstructuraProducto = new List<DatoEstructuraProducto>();
                    else productoOriginal.DatoEstructuraProducto.Clear();

                    productoOriginal.DatoEstructuraProducto.Add(new DatoEstructuraProducto { IdDatoEstructura = producto.IdDatoEstructura });

                    if (productoOriginal.Presentacion == null)
                        productoOriginal.Presentacion = new List<Presentacion>();
                    else productoOriginal.Presentacion.Clear();

                    foreach (var presentacion in Presentaciones)
                    {
                        productoOriginal.Presentacion.Add(
                            new Presentacion
                            {
                                IdProducto = presentacion.IdProducto,
                                Equivalencia = presentacion.Equivalencia,
                                EsBase = presentacion.EsBase,
                                IdUnidadMedida = presentacion.IdUnidadMedida,
                                Nombre = presentacion.Nombre,
                                Peso = presentacion.Peso,
                                UsuarioCreacion = presentacion.UsuarioCreacion,
                                FechaCreacion = presentacion.FechaCreacion,
                                UsuarioModificacion = presentacion.UsuarioModificacion,
                                FechaModificacion = presentacion.FechaModificacion
                            });
                    }

                    ProductoBL.Instancia.Update(productoOriginal);

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
   
        private static void PrepararDatos(ref Producto producto)
        {
            producto.TiposProducto = Utils.EnumToList<TipoProducto>();
            producto.TiposProducto.Insert(0, new Comun { Id = 0, Nombre = "- Seleccionar -" });

            producto.TiposClasificacion = Utils.EnumToList<TipoClasificacion>();
            producto.TiposClasificacion.Insert(0, new Comun { Id = 0, Nombre = "- Seleccionar -" });

            producto.Estados = Utils.EnumToList<TipoEstado>();

            producto.Lineas = DatoEstructuraBL.Instancia.GetByIdEstructura(1);
            producto.Lineas.Insert(0, new DatoEstructura { IdDatoEstructura = 0, Nombre = "- Seleccionar -" });

            var datosProducto = DatoEstructuraProductoBL.Instancia.GetDatosByIdProducto(producto.IdProducto);
            if (!(datosProducto != null && datosProducto.Count > 0)) return;

            var idCategoria = datosProducto.FirstOrDefault().IdDatoEstructura;
            var categoria = DatoEstructuraBL.Instancia.Single(idCategoria);
            producto.IdDatoEstructura = categoria.IdDatoEstructura;
            if (categoria.IdParent != null) producto.IdSubLinea = (int)categoria.IdParent;
            var idLinea = DatoEstructuraBL.Instancia.Single(producto.IdSubLinea).IdParent;
            if (idLinea != null) producto.IdLinea = (int)idLinea;
        }

        public JsonResult ListarLineas()
        {
            var  Lineas = DatoEstructuraBL.Instancia.GetByIdEstructura(1);
            var listItems = Utils.ConvertToListItem(Lineas, "IdDatoEstructura", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarSubLineas(int idLinea)
        {
            var subLineas = DatoEstructuraBL.Instancia.GetByIdParent(idLinea);
            var listItems = Utils.ConvertToListItem(subLineas, "IdDatoEstructura", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarCategorias(int subLinea)
        {
            var categorias = DatoEstructuraBL.Instancia.GetByIdParent(subLinea);
            var listItems = Utils.ConvertToListItem(categorias, "IdDatoEstructura", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTiposProductoHtml()
        {
            var tiposProductos = Utils.EnumToList<TipoProducto>();
            var select = "<select>";
            select += "<option value=''></option>";
            
            foreach (var item in tiposProductos)
            {
                select += string.Format("<option value='{0}'>{1}</option>", item.Id, item.Nombre);
            }

            select += "</select>";

            return Content(select);
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse { Success = false };

            if (ModelState.IsValid)
            {
                try
                {
                    var productoId = Convert.ToInt32(id);
                    ProductoBL.Instancia.Delete(productoId);

                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se quito el registro con exito.";
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = "No es posible Eliminar, hay datos registrados relacionados con este Producto. ";
                }
            }
            else
            {
                jsonResponse.Message = "No se pudo eliminar.";
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }
        
        #region Presentación Producto
        
        [HttpPost]
        public JsonResult ListarPresentaciones(GridTable grid, int productoId)
        {
            var jqgrid = new JGrid();

            try
            {
                var totalPaginas = 0;
                var cantidad = Presentaciones.Count;

                grid.page = (grid.page == 0) ? 1 : grid.page;
                grid.rows = (grid.rows == 0) ? 100 : grid.rows;

                if (cantidad > 0 && grid.rows > 0)
                {
                    var div = cantidad / (decimal)grid.rows;
                    var round = Math.Ceiling(div);
                    totalPaginas = Convert.ToInt32(round);
                    totalPaginas
                        = totalPaginas == 0 ? 1 : totalPaginas;
                }

                grid.page = grid.page > totalPaginas ? totalPaginas : grid.page;

                var start = grid.rows * grid.page - grid.rows;
                if (start < 0) start = 0;

                jqgrid.total = totalPaginas;
                jqgrid.page = grid.page;
                jqgrid.records = cantidad;
                jqgrid.start = start;

                jqgrid.rows = Presentaciones.Select(item => new JRow
                {
                    id = item.IdProducto.ToString(),
                    cell = new[]
                                {
                                    item.IdPresentacion.ToString(),
                                    item.Nombre,
                                    item.Peso.ToString(),
                                    item.Equivalencia.ToString(),
                                    item.EsBase.ToString(),
                                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }

            return Json(jqgrid);
        }

        public ActionResult CrearPresentacion(string idProducto)
        {
            ViewData["Accion"] = "CrearPresentacion";
            var presentacion = new Presentacion { IdProducto = Convert.ToInt32(idProducto) };

            PrepararDatosPresentacion(ref presentacion);
            return PartialView("PresentacionProductoPanel", presentacion);
        }

        [HttpPost]
        public JsonResult CrearPresentacion(Presentacion presentacion)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var unidadMedida = UnidadMedidaBL.Instancia.Single(presentacion.IdUnidadMedida);
                    presentacion.Nombre = unidadMedida.Nombre;
                    presentacion.FechaCreacion = FechaCreacion;
                    presentacion.FechaModificacion = FechaModificacion;
                    presentacion.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    presentacion.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    if (presentacion.IdPresentacion == 0)
                    {
                        presentacion.IdPresentacion = Presentaciones.Count == 0 ? 1 : Presentaciones.Max(p => p.IdPresentacion) + 1;
                    }

                    Presentaciones.Add(presentacion);

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

        public ActionResult ModificarPresentacion(string id)
        {
            try
            {
                ViewData["Accion"] = "ModificarPresentacion";

                var idPresentacion = Convert.ToInt32(id);
                var presentacion = Presentaciones.SingleOrDefault(p => p.IdPresentacion == idPresentacion);

                PrepararDatosPresentacion(ref presentacion);

                return PartialView("PresentacionProductoPanel", presentacion);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ModificarPresentacion(Presentacion prese)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var presentacionOriginal = Presentaciones.SingleOrDefault(p => p.IdPresentacion == prese.IdPresentacion);
                    var unidadMedida = UnidadMedidaBL.Instancia.Single(prese.IdUnidadMedida);

                    presentacionOriginal.Nombre = unidadMedida.Nombre;
                    presentacionOriginal.Equivalencia = prese.Equivalencia;
                    presentacionOriginal.EsBase = prese.EsBase;
                    presentacionOriginal.IdUnidadMedida = prese.IdUnidadMedida;
                    presentacionOriginal.Peso = prese.Peso;
                    presentacionOriginal.FechaModificacion = FechaModificacion;
                    presentacionOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

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

        public void PrepararDatosPresentacion(ref Presentacion presentacion)
        {
            presentacion.TiposUnidad = UnidadMedidaBL.Instancia.Get();
        }

        #endregion Presentación Producto
    }
}