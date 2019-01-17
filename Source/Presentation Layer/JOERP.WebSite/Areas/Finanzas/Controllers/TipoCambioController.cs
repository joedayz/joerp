
namespace JOERP.WebSite.Areas.Finanzas.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.JqGrid;
    using JOERP.Helpers.Enums;

    public class TipoCambioController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("TipoCambioListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Nombre", "Fecha", "ValorCompra", "ValorVenta", "IdEmpresa" };
                var lista = CrearJGrid(TipoCambioBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdTipoCambio.ToString(),
                    cell = new[]
                                {
                                    item.IdTipoCambio.ToString(),
                                    item.Nombre,
                                    item.Fecha.Value.ToShortDateString(),
                                    item.ValorCompra.ToString(),
                                    item.ValorVenta.ToString(),
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

            var entidad = new TipoCambio
            {
               ValorCompra = 0,ValorVenta = 0,Fecha = DateTime.Now,
            };

            PrepararDatos(ref entidad);
            return PartialView("TipoCambioPanel", entidad);
        }

        [HttpPost]
        public JsonResult Crear(TipoCambio tipoCambio)
        {
            var jsonResponse = new JsonResponse();
           
            if (ModelState.IsValid)
            {
                try
                {
                    tipoCambio.IdEmpresa = IdEmpresa;
                    tipoCambio.FechaCreacion = FechaCreacion;
                    tipoCambio.FechaModificacion = FechaModificacion;
                    tipoCambio.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    tipoCambio.UsuarioModifcacion= UsuarioActual.IdEmpleado;
                    tipoCambio.IdEmpresa = IdEmpresa;

                    TipoCambioBL.Instancia.Add(tipoCambio);

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

                var tipoCambio = TipoCambioBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref tipoCambio);

                return PartialView("TipoCambioPanel", tipoCambio);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(TipoCambio tipoCambio)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var tipoCambioOriginal =TipoCambioBL.Instancia.Single(tipoCambio.IdTipoCambio);

                    tipoCambioOriginal.IdEmpresa = IdEmpresa;
                    tipoCambioOriginal.Fecha = tipoCambio.Fecha;
                    tipoCambioOriginal.IdMoneda = tipoCambio.IdMoneda;
                    tipoCambioOriginal.ValorVenta= tipoCambio.ValorVenta;
                    tipoCambioOriginal.ValorCompra = tipoCambio.ValorCompra;
                    tipoCambioOriginal.TipoCalculo = tipoCambio.TipoCalculo;
                    tipoCambioOriginal.FechaModificacion = FechaModificacion;
                    tipoCambioOriginal.UsuarioModifcacion= UsuarioActual.IdEmpleado;

                    TipoCambioBL.Instancia.Update(tipoCambioOriginal);

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

        private static void PrepararDatos(ref TipoCambio tipoCambio)
        {
            tipoCambio.Monedas = MonedaBL.Instancia.GetAll();
            tipoCambio.TiposCalculo = Utils.EnumToList<TipoCalculoMoneda>();
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var tipoCambioId = Convert.ToInt32(id);
                    TipoCambioBL.Instancia.Delete(tipoCambioId);

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
