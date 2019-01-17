using System.Web.Mvc;

namespace JOERP.WebSite.Areas.Finanzas
{
    public class FinanzasAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Finanzas";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Finanzas_default",
                "Finanzas/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
