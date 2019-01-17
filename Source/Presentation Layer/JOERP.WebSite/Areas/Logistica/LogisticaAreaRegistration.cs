using System.Web.Mvc;

namespace JOERP.WebSite.Areas.Logistica
{
    public class LogisticaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Logistica";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Logistica_default",
                "Logistica/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
