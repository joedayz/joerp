
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
    using Helpers.JqGrid;

    public class OperacionController : BaseController
    {
        public IList<OperacionDocumento> Documentos
        {
            get { return (IList<OperacionDocumento>)Session["DocumentosSesion"]; }
            set { Session["DocumentosSesion"] = value; }
        }

        public ActionResult Index()
        {
            return PartialView("OperacionListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Codigo", "Nombre", "SignoStock","SignoCaja","SignoContable","Estado" };
                var lista = CrearJGrid(OperacionBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdOperacion.ToString(),
                    cell = new[]
                                {
                                    item.IdOperacion.ToString(),
                                    item.Codigo,
                                    item.Nombre,
                                    item.SignoStock.ToString(),
                                    item.SignoCaja.ToString(),
                                    item.SignoContable.ToString(),
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

            var entidad = new Operacion
            {
                Codigo = (OperacionBL.Instancia.MaxId() + 1).ToString()
            };
            Documentos = new List<OperacionDocumento>();
            PrepararDatos(ref entidad);
            return PartialView("OperacionPanel", entidad);
        }

        [HttpPost]
        public JsonResult Crear(Operacion entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var doc in Documentos)
                    {
                        entidad.OperacionDocumentos.Add(new OperacionDocumento
                        {
                            IdOperacion = entidad.IdOperacion,
                            TipoDocumento = doc.TipoDocumento,
                            Orden = doc.Orden,
                            Posicion = doc.Posicion,
                            Estado = doc.Estado,
                        });
                    }

                    entidad.FechaCreacion = FechaCreacion;
                    entidad.FechaModificacion = FechaModificacion;
                    entidad.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    entidad.UsuarioModificacion = UsuarioActual.IdEmpleado;
                   
                    OperacionBL.Instancia.Add(entidad);
                    
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

                var entidad = OperacionBL.Instancia.Single(Convert.ToInt32(id));
                Documentos = OperacionDocumentoBL.Instancia.GetByOperacion(entidad.IdOperacion);
                PrepararDatos(ref entidad);

                return PartialView("OperacionPanel", entidad);
            }
            catch(Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Operacion entidad)
        {
            var jsonResponse = new JsonResponse { Success = false };

            if (ModelState.IsValid)
            {
                try
                {
                    var entidadOriginal = OperacionBL.Instancia.Single(entidad.IdOperacion);
                    entidadOriginal.Codigo= entidad.Codigo;
                    entidadOriginal.Nombre= entidad.Nombre;
                    entidadOriginal.Descripcion= entidad.Descripcion;
                    entidadOriginal.SignoCaja= entidad.SignoCaja;
                    entidadOriginal.SignoCartera= entidad.SignoCartera;
                    entidadOriginal.SignoContable= entidad.SignoContable;
                    entidadOriginal.SignoCartera= entidad.SignoCartera;
                    entidadOriginal.SignoStock = entidad.SignoStock;
                    entidadOriginal.TipoDocumentoInterno = entidad.TipoDocumentoInterno;
                    entidadOriginal.RealizaAsiento = entidad.RealizaAsiento;
                    entidadOriginal.Estado = entidad.Estado;
                    entidadOriginal.FechaModificacion = FechaModificacion;
                    entidadOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    if (entidadOriginal.OperacionDocumentos== null)
                        entidadOriginal.OperacionDocumentos= new List<OperacionDocumento>();
                    else entidadOriginal.OperacionDocumentos.Clear();

                    foreach (var doc in Documentos)
                    {
                        entidadOriginal.OperacionDocumentos.Add(new OperacionDocumento
                        {
                            IdOperacion = entidad.IdOperacion,
                            TipoDocumento = doc.TipoDocumento,
                            Orden = doc.Orden,
                            Posicion = doc.Posicion,
                            Estado = doc.Estado,
                        });
                    }

                    OperacionBL.Instancia.Update(entidadOriginal);

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

        private static void PrepararDatos(ref Operacion entidad)
        {
            entidad.Estados = Utils.EnumToList<TipoEstado>();
            entidad.Signos = new List<Comun>
                                 {
                                     new Comun {Id = -1},
                                     new Comun {Id = 0},
                                     new Comun {Id = 1},
                                 };
        }

        [HttpPost]
        public JsonResult ListarDocumentos(GridTable grid, int operacionId)
        {
            var jqgrid = new JGrid();

            try
            {
                var totalPaginas = 0;
                var cantidad = Documentos.Count;

                grid.page = (grid.page == 0) ? 1 : grid.page;
                grid.rows = (grid.rows == 0) ? 100 : grid.rows;

                if (cantidad > 0 && grid.rows > 0)
                {
                    var div = cantidad / (decimal)grid.rows;
                    var round = Math.Ceiling(div);
                    totalPaginas = Convert.ToInt32(round);
                    totalPaginas = totalPaginas == 0 ? 1 : totalPaginas;
                }

                grid.page = grid.page > totalPaginas ? totalPaginas : grid.page;

                var start = grid.rows * grid.page - grid.rows;
                if (start < 0) start = 0;

                jqgrid.total = totalPaginas;
                jqgrid.page = grid.page;
                jqgrid.records = cantidad;
                jqgrid.start = start;

                jqgrid.rows = Documentos.Select(item => new JRow
                {
                    id = item.TipoDocumento.ToString(),
                    cell = new[]
                                {
                                    item.TipoDocumento.ToString(),
                                    ItemTablaBL.Instancia.Single((int)TipoTabla.TipoComprobante,item.TipoDocumento).Nombre ,
                                    item.Orden.ToString(),
                                    item.Posicion.ToString(),
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

        public ActionResult CrearDocumento(string idOperacion)
        {
            ViewData["Accion"] = "CrearDocumento";
            var entidad = new OperacionDocumento{ IdOperacion = Convert.ToInt32(idOperacion) };

            PrepararDatosDocumento(ref entidad);
            return PartialView("OperacionDocumentoPanel", entidad);
        }

        [HttpPost]
        public JsonResult CrearDocumento(OperacionDocumento entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    if (Documentos == null)
                    {
                        Documentos = new List<OperacionDocumento>();
                    }

                    Documentos.Add(entidad);

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

        public ActionResult ModificarDocumento(string idOperacion, string tipoDocumento)
        {
            try
            {
                ViewData["Accion"] = "ModificarDocumento";

                var entidad = Documentos.SingleOrDefault(p => p.IdOperacion == Convert.ToInt32(idOperacion) && p.TipoDocumento == Convert.ToInt32(tipoDocumento));

                PrepararDatosDocumento(ref entidad);

                return PartialView("OperacionDocumentoPanel", entidad);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ModificarDocumento(OperacionDocumento entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var entidadOriginal = Documentos.SingleOrDefault(p => p.IdOperacion == entidad.IdOperacion && p.TipoDocumento == entidad.TipoDocumento);
                    if (entidadOriginal != null)
                    {
                        entidadOriginal.TipoDocumento= entidad.TipoDocumento;
                        entidadOriginal.Orden = entidad.Orden;
                        entidadOriginal.Posicion= entidad.Posicion;
                        entidadOriginal.Estado = entidad.Estado;
                    }

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

        private static void PrepararDatosDocumento (ref OperacionDocumento operacionDocumento)
        {
            operacionDocumento.TipoDocumentos = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoComprobante);
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var operacionId = Convert.ToInt32(id);
                    OperacionBL.Instancia.Delete(operacionId);

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

        [HttpPost]
        public JsonResult EliminarDocumento(string idOperacion, string tipoDocumento)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var id = Convert.ToInt32(idOperacion);
                    var tipoDoc = Convert.ToInt32(tipoDocumento);
                    var documento = Documentos.SingleOrDefault(p => p.IdOperacion == id && p.TipoDocumento == tipoDoc);

                    if (documento != null)
                    {
                        Documentos.Remove(documento);
                    }
                    
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
                jsonResponse.Message = "Por favor ingrese todos los campos requeridos";
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }
    }
}
