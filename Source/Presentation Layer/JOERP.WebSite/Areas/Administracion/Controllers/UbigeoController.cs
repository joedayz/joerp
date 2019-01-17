
namespace JOERP.WebSite.Areas.Administracion.Controllers
{
    using System.Web.Mvc;
    using Business.Logic;
    using Helpers;

    public class UbigeoController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return null;
        }

        public virtual JsonResult GetProvincias(int idDepartamento)
        {
            var provincias = UbigeoBL.Instancia.GetAllProvincias(idDepartamento);
            var listItems = Utils.ConvertToListItem(provincias, "Id", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult GetDistritos(int idDepartamento, int idProvincia)
        {
            var distritos = UbigeoBL.Instancia.GetAllDistritos(idDepartamento, idProvincia);
            var listItems = Utils.ConvertToListItem(distritos, "Id", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
    }
}
