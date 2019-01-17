
namespace JOERP.WebSite.Areas.Logistica.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.Enums;
    using Helpers.JqGrid;

    public class TransporteController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("TransporteListado");
        }
        
        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Codigo","FullName", "Direccion", "Telefono", "Estado", "IdFuncion" };
                var lista = CrearJGrid(PersonaBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdPersona.ToString(),
                    cell = new[]
                                {
                                   item.IdPersona.ToString(),
                                    item.FullName,
                                    item.Direccion ?? "",
                                    item.Telefono!=null? item.Telefono.ToString():"",
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

             var transporte = new Persona
                {
                    Codigo = (EmpleadoBL.Instancia.MaxId() + 1).ToString(),
                    TipoDocumento = 4,
                    PersonaDirecciones = new List<PersonaDireccion>(), 
                    PersonaDireccionEnt = new PersonaDireccion{ Ubigeo = new Ubigeo() },
                    PersonaContactos = new List<PersonaContacto>(),
                    PersonaFunciones = new List<PersonaFuncion>(),
                    TipoZonas = new BindingList<Comun>(),
                    TipoVias = new BindingList<Comun>()
                };
            PrepararDatos(ref transporte);
          
            return PartialView("TransportePanel", transporte);
        }

        [HttpPost]
        public JsonResult Crear(Persona transporte)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var persona = new Persona
                    {
                        TipoPersona = (int)TipoPersona.Natural,
                        Documento = transporte.Documento,
                        TipoDocumento = transporte.TipoDocumento,
                        Telefono = transporte.Telefono,
                        Celular = transporte.Celular,
                        IdEmpresa = IdEmpresa,
                        Nombres = "-",
                        ApellidoMaterno = "-",
                        ApellidoPaterno = "-",
                        FullName = transporte.RazonSocial,
                        RazonSocial = transporte.RazonSocial,
                        Email = transporte.Email,
                        NumeroLicencia = transporte.NumeroLicencia,
                        Codigo = transporte.Codigo,
                        EstadoCivil = transporte.EstadoCivil,
                        Sexo = transporte.SexoC,
                        FechaNacimiento = transporte.FechaNacimiento,
                        FechaCreacion = FechaSistema,
                        FechaModificacion = FechaSistema,
                        UsuarioCreacion = UsuarioActual.IdEmpleado,
                        UsuarioModificacion = UsuarioActual.IdEmpleado,
                        Estado = transporte.Estado,
                        PersonaContactos = new List<PersonaContacto>(),
                        PersonaDirecciones = new List<PersonaDireccion>(),
                        PersonaFunciones = new List<PersonaFuncion>()
                    };

                    persona.PersonaFunciones.Add(new PersonaFuncion { IdFuncion = (int)TipoFuncion.Transporte});
                    persona.PersonaDirecciones.Add(new PersonaDireccion
                    {
                        IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(transporte.IdDepartamento, transporte.IdProvincia,
                                     transporte.IdUbigeo),
                        TipoDireccion = transporte.PersonaDireccionEnt.TipoDireccion,
                        TipoVia = transporte.PersonaDireccionEnt.TipoVia,
                        TipoZona = transporte.PersonaDireccionEnt.TipoZona,
                        NombreVia = transporte.PersonaDireccionEnt.NombreVia,
                        FechaCreacion = FechaCreacion,
                        FechaModificacion = FechaModificacion,
                        UsuarioCreacion = UsuarioActual.IdEmpleado,
                        UsuarioModificacion = UsuarioActual.IdEmpleado,
                        NombreZona = transporte.PersonaDireccionEnt.NombreZona,
                        Numero = transporte.PersonaDireccionEnt.Numero,
                        Interior = transporte.PersonaDireccionEnt.Interior,
                        IdPersona = transporte.IdPersona,
                        Referencia = transporte.PersonaDireccionEnt.Referencia
                    });
                    if (transporte.PersonaDireccionEnt.TipoVia != null)
                        persona.Direccion = ItemTablaBL.Instancia.Single((int)TipoTabla.TipoVia, 
                            (int)transporte.PersonaDireccionEnt.TipoVia).Nombre + " " +
                            transporte.PersonaDireccionEnt.NombreVia + " #" + transporte.PersonaDireccionEnt.Numero + " " +
                            transporte.PersonaDireccionEnt.Interior;

                    PersonaBL.Instancia.Add(persona);

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

                var transporte = PersonaBL.Instancia.Single(Convert.ToInt32(id));
                transporte.PersonaDireccionEnt = transporte.PersonaDirecciones.Count != 0 ? transporte.PersonaDirecciones.FirstOrDefault() : new PersonaDireccion();
                
                PrepararDatos(ref transporte);
                return PartialView("TransportePanel", transporte);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Persona transporte)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var transporteOriginal = PersonaBL.Instancia.Single(transporte.IdPersona);
                    transporteOriginal.RazonSocial = transporte.RazonSocial;
                    transporteOriginal.FullName = transporte.RazonSocial;
                    transporteOriginal.TipoPersona = (int) TipoPersona.Natural;
                    transporteOriginal.Documento = transporte.Documento;
                    transporteOriginal.TipoDocumento = transporte.TipoDocumento;
                    transporteOriginal.Telefono = transporte.Telefono;
                    transporteOriginal.Celular = transporte.Celular;
                    transporteOriginal.IdEmpresa = IdEmpresa;
                    transporteOriginal.Email = transporte.Email;
                    transporteOriginal.NumeroLicencia = transporte.NumeroLicencia;
                    transporteOriginal.Codigo = transporte.Codigo;
                    transporteOriginal.EstadoCivil = transporte.EstadoCivil;
                    transporteOriginal.Sexo = transporte.SexoC;
                    transporteOriginal.FechaNacimiento = transporte.FechaNacimiento;
                    transporteOriginal.FechaCreacion = FechaSistema;
                    transporteOriginal.FechaModificacion = FechaSistema;
                    transporteOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    transporteOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    transporteOriginal.Estado = transporte.Estado;
                    transporteOriginal.PersonaDirecciones.First().IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(transporte.IdDepartamento, transporte.IdProvincia, transporte.IdUbigeo);
                    transporteOriginal.PersonaDirecciones.First().TipoDireccion = transporte.PersonaDireccionEnt.TipoDireccion;
                    transporteOriginal.PersonaDirecciones.First().TipoVia = transporte.PersonaDireccionEnt.TipoVia;
                    transporteOriginal.PersonaDirecciones.First().TipoZona = transporte.PersonaDireccionEnt.TipoZona;
                    transporteOriginal.PersonaDirecciones.First().NombreVia = transporte.PersonaDireccionEnt.NombreVia;
                    transporteOriginal.PersonaDirecciones.First().FechaCreacion = FechaCreacion;
                    transporteOriginal.PersonaDirecciones.First().FechaModificacion = FechaModificacion;
                    transporteOriginal.PersonaDirecciones.First().UsuarioCreacion = UsuarioActual.IdEmpleado;
                    transporteOriginal.PersonaDirecciones.First().UsuarioModificacion = UsuarioActual.IdEmpleado;
                    transporteOriginal.PersonaDirecciones.First().NombreZona = transporte.PersonaDireccionEnt.NombreZona;
                    transporteOriginal.PersonaDirecciones.First().Numero = transporte.PersonaDireccionEnt.Numero;
                    transporteOriginal.PersonaDirecciones.First().Interior = transporte.PersonaDireccionEnt.Interior;
                    transporteOriginal.PersonaDirecciones.First().Referencia = transporte.PersonaDireccionEnt.Referencia;
                    transporteOriginal.FechaModificacion = FechaModificacion;
                    transporteOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    transporteOriginal.PersonaFunciones = null;
                    if (transporte.PersonaDireccionEnt.TipoVia != null)
                        transporteOriginal.Direccion = ItemTablaBL.Instancia.Single((int)TipoTabla.TipoVia, 
                            (int)transporte.PersonaDireccionEnt.TipoVia).Nombre + " " +
                            transporte.PersonaDireccionEnt.NombreVia + " #" + transporte.PersonaDireccionEnt.Numero + " " +
                            transporte.PersonaDireccionEnt.Interior;

                    PersonaBL.Instancia.Update(transporteOriginal);

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

        private void PrepararDatos(ref Persona transporte)
        {
            transporte.Estados = Utils.EnumToList<TipoEstado>();
            transporte.Cargos = CargoBL.Instancia.GetByEmpresa(IdEmpresa);
            transporte.EstadoCiviles = Utils.EnumToList<TipoEstadoCivil>();
            transporte.TipoDocumentos = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoDocumento);
            transporte.Sexos = Utils.EnumToList<TipoSexo>();
            transporte.TipoVias = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoVia);
            transporte.TipoZonas = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoZona);
            transporte.Departamentos = UbigeoBL.Instancia.GetAllDepartamentos();
            transporte.SexoC = Convert.ToInt32(transporte.Sexo);
            var temp = PersonaDireccionBL.Instancia.PrimeroDireccion(transporte.IdPersona);
            if (temp == null)
            {
                transporte.Departamentos.Insert(0, new Comun { Nombre = "-- Seleccionar --", Id = 0 });
                return;
            }
            transporte.PersonaDireccionEnt = temp;
            transporte.IdDepartamento = temp.Ubigeo.IdDepartamento;
            transporte.IdProvincia = temp.Ubigeo.IdProvincia;
            transporte.IdUbigeo = temp.Ubigeo.IdDistrito;
        }
    }
}
