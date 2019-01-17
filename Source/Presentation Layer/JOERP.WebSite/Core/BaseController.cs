
namespace JOERP.WebSite.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Business.Entity;
    using JOERP.Helpers;
    using JOERP.Helpers.JqGrid;
    using Newtonsoft.Json;

    [HandleError]
    [Authorize]
    public class BaseController : Controller
    {
        #region Constructor

        public BaseController()
        {
            ViewData["FechaSistema"] = FechaSistema.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            System.Web.HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            System.Web.HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            System.Web.HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();
        }

        #endregion Constructor

        #region Propiedades

        protected Usuario UsuarioActual
        {
            get { return (Usuario) System.Web.HttpContext.Current.Session[Constantes.Usuario]; }
            set { System.Web.HttpContext.Current.Session.Add(Constantes.Usuario, value); }
        }

        protected int IdEmpresa
        {
            get { return Convert.ToInt32(System.Web.HttpContext.Current.Session[Constantes.IdEmpresa]); }
            set { System.Web.HttpContext.Current.Session.Add(Constantes.IdEmpresa, value); }
        }

        protected Empresa Empresa
        {
            get { return (Empresa) System.Web.HttpContext.Current.Session[Constantes.Empresa]; }
            set { System.Web.HttpContext.Current.Session.Add(Constantes.Empresa, value); }
        }

        protected int IdSucursal
        {
            get { return Convert.ToInt32(System.Web.HttpContext.Current.Session[Constantes.IdSucursal]); }
            set { System.Web.HttpContext.Current.Session.Add(Constantes.IdSucursal, value); }
        }

        protected Sucursal Sucursal
        {
            get { return (Sucursal) System.Web.HttpContext.Current.Session[Constantes.Sucursal]; }
            set { System.Web.HttpContext.Current.Session.Add(Constantes.Sucursal, value); }
        }

        protected DateTime FechaSistema
        {
            get { return DateTime.Now; }
        }

        protected DateTime FechaCreacion
        {
            get { return DateTime.Now; }
        }

        protected DateTime FechaModificacion
        {
            get { return DateTime.Now; }
        }

        protected Impuesto Igv
        {
            get { return (Impuesto)System.Web.HttpContext.Current.Session[Constantes.Igv]; }
            set { System.Web.HttpContext.Current.Session.Add(Constantes.Igv, value); }
        }

        protected Moneda MonedaLocal
        {
            get { return (Moneda)System.Web.HttpContext.Current.Session[Constantes.MonedaLocal]; }
            set { System.Web.HttpContext.Current.Session.Add(Constantes.MonedaLocal, value); }
        }

        protected int IdOperacion
        {
            get { return (int)Session["IdOperacion"]; }
            set { Session["IdOperacion"] = value; }
        }

        #endregion Propiedades

        #region Metodos

        protected IList<T> CrearJGrid<T>(IPaged<T> iPaged, GridTable gridTable, string [] nombreFiltros, ref JGrid jGrid) where T : class
        {
            var totalPaginas = 0;
            var filtros = new List<object>();

            if (!string.IsNullOrEmpty(gridTable.filters))
            {
                var filters = JsonConvert.DeserializeObject<JOERP.Helpers.JqGrid.Filter>(gridTable.filters);

                foreach (var nombreFiltro in nombreFiltros)
                {
                    var filtro = filters.rules.FirstOrDefault(p => p.field == nombreFiltro);
                    filtros.Add(filtro == null ? null : filtro.data);
                }
            }
            else
            {
                foreach (var nombreFiltro in nombreFiltros)
                {
                    filtros.Add(null);
                }
            }

            if (gridTable.rules != null)
            {
                var index = 0;
                foreach (var nombreFiltro in nombreFiltros)
                {
                    foreach (var rule in gridTable.rules)
                    {
                        if (rule.field != nombreFiltro) continue;
                        filtros[index] = rule.data;
                    }
                    index++;
                }
            }

            var cantidad = iPaged.Count(filtros.ToArray());

            gridTable.page = (gridTable.page == 0) ? 1 : gridTable.page;
            gridTable.rows = (gridTable.rows == 0) ? 100 : gridTable.rows;

            if (cantidad > 0 && gridTable.rows > 0)
            {
                var div = cantidad / (decimal)gridTable.rows;
                var round = Math.Ceiling(div);
                totalPaginas = Convert.ToInt32(round);
                totalPaginas = totalPaginas == 0 ? 1 : totalPaginas;
            }

            gridTable.page = gridTable.page > totalPaginas ? totalPaginas : gridTable.page;

            var start = gridTable.rows * gridTable.page - gridTable.rows;
            if (start < 0) start = 0;

            jGrid.total = totalPaginas;
            jGrid.page = gridTable.page;
            jGrid.records = cantidad;
            jGrid.start = start;

            filtros.Insert(0, gridTable.sidx);
            filtros.Insert(1, gridTable.sord);
            filtros.Insert(2, gridTable.rows);
            filtros.Insert(3, start);
            filtros.Insert(4, cantidad);

            var lista = iPaged.GetPaged(filtros.ToArray());
            
            return lista;
        }

        protected JGrid CrearJgrid(GridTable grid, int cantidad)
        {
            var totalPaginas = 0;

            grid.page = (grid.page == 0) ? 1 : grid.page;
            grid.rows = (grid.rows == 0) ? 100 : grid.rows;

            if (cantidad > 0 && grid.rows > 0)
            {
                var div = cantidad/(decimal) grid.rows;
                var round = Math.Ceiling(div);
                totalPaginas = Convert.ToInt32(round);
                totalPaginas = totalPaginas == 0 ? 1 : totalPaginas;
            }

            grid.page = grid.page > totalPaginas ? totalPaginas : grid.page;

            var start = grid.rows*grid.page - grid.rows;
            if (start < 0) start = 0;

            var jqgrid = new JGrid
                             {
                                 total = totalPaginas, 
                                 page = grid.page, 
                                 records = cantidad, 
                                 start = start, 
                             };

            return jqgrid;
        }

        protected JsonResult MensajeError(string mensaje = "Ocurrio un error.")
        {
            Response.StatusCode = 404;
            return Json(new JsonResponse {Message = mensaje}, JsonRequestBehavior.AllowGet);
        }

        public void MostrarError(string mensaje)
        {
        }

        #endregion Metodos
    }
}