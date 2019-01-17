
namespace JOERP.WebSite.Areas.Logistica.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Logic;
    using Core;
    using Helpers.JqGrid;

    public class RecepcionTransferenciaController : BaseController
    {
        public ActionResult Index(int id)
        {
            var formulario = FormularioBL.Instancia.Single(id);
            if (formulario != null)
            {
                if (formulario.IdOperacion.HasValue)
                {
                    ViewData["Operacion"] = formulario.IdOperacion;
                    IdOperacion = formulario.IdOperacion.Value;
                }
            }
            return PartialView("RecepcionTransferenciaListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "IdOperacion", "Documento", "RazonSocial", "FechaDocumento", "Estado" };
                var lista = CrearJGrid(IngresoTransferenciaAlmacenBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdTransaccion.ToString(),
                    cell = new[]
                                {   item.IdTransaccion.ToString(),
                                    item.Documento,
                                    AlmacenBL.Instancia.Single(item.IdAlmacen).Nombre,
                                    item.FechaDocumento.ToShortDateString(),
                                    item.MontoNeto.ToString(),
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
    }
}
