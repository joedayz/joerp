
namespace JOERP.WebSite.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Business.Entity;
    using Business.Logic;
    using Helpers;
    using Helpers.Enums;
    using Models;

    [HandleError]
    public class SesionController : Controller
    {
        public ActionResult Login()
        {
            var empresas = EmpresaBL.Instancia.Get();

            var modelo = new LoginModel
            {
                UserName = string.Empty,
                Password = string.Empty,
                Empresas = Utils.ConvertToListItem(empresas, "IdEmpresa", "RazonSocial"),
                Sucursales = new List<SelectListItem>()
            };

            return View(modelo);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = UsuarioBL.Instancia.Single(model.UserName, model.Password, model.IdEmpresa);
                    if (usuario != null)
                    {
                        if (UsuarioBL.Instancia.VerificarAccesoSucusal(model.UserName, model.IdSucursal))
                        {
                            var minutosDuracionCookie = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MinutosDuracionCookie"));
                            var userData = string.Format("{0} {1} {2}", usuario.IdEmpleado, usuario.NombreEmpleado, usuario.IdCargo);
                            var ticket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(minutosDuracionCookie), model.RememberMe, userData);
                            var encTicket = FormsAuthentication.Encrypt(ticket);
                            var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);

                            var idModulos = FormularioBL.Instancia.Modulos(usuario.IdEmpleado);
                            var modulos = ItemTablaBL.Instancia.GetByTabla((int)TipoTabla.Modulo);
                            var formularios = new List<Formulario>();

                            foreach (var idModulo in idModulos)
                            {
                                var formulariosModulo = FormularioBL.Instancia.Formularios(usuario.IdEmpleado, idModulo).OrderBy(p => p.Nombre);
                                formularios.AddRange(formulariosModulo);
                            }

                            System.Web.HttpContext.Current.Session.Add(Constantes.Usuario, usuario);
                            System.Web.HttpContext.Current.Session.Add(Constantes.IdModulos, idModulos);
                            System.Web.HttpContext.Current.Session.Add(Constantes.Modulos, modulos);
                            System.Web.HttpContext.Current.Session.Add(Constantes.Formularios, formularios);
                            System.Web.HttpContext.Current.Session.Add(Constantes.IdEmpresa, model.IdEmpresa);
                            System.Web.HttpContext.Current.Session.Add(Constantes.IdSucursal,model.IdSucursal);
                            System.Web.HttpContext.Current.Session.Add(Constantes.Igv, ImpuestoBL.Instancia.Single((int)TipoImpuesto.IGV));
                            System.Web.HttpContext.Current.Session.Add(Constantes.MonedaLocal, MonedaBL.Instancia.MonedaLocal(model.IdEmpresa));
                            System.Web.HttpContext.Current.Response.Cookies.Add(faCookie);

                            jsonResponse.Success = true;
                            jsonResponse.Data = Utils.AbsoluteWebRoot + "ERP";   
                        }
                        else
                        {
                            jsonResponse.Message = "Ud. no tiene acceso a la Sucursal seleccionada.";    
                        }
                    }
                    else
                    {
                        jsonResponse.Message = "Las credenciales especificadas son incorrectas.";
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = "Las credenciales especificadas son incorrectas.";
                }
            }
            else
            {
                jsonResponse.Message = "Ud. no selecciono la información correcta.";
            }

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOff()
        {
            System.Web.HttpContext.Current.Session.Abandon();
            System.Web.HttpContext.Current.Session.Clear();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Sesion");
        }

        public JsonResult ListarPorPais(int idEmpresa)
        {
            var sucursales = SucursalBL.Instancia.GetByEmpresa(idEmpresa);
            var listItems = Utils.ConvertToListItem(sucursales, "IdSucursal", "Nombre");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
    }
}
