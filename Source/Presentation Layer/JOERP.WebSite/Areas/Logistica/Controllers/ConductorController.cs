
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

    public class ConductorController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("ConductorListado");
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
                                    item.Codigo.ToString(),
                                    item.FullName,
                                    item.Direccion ?? "",
                                    item.Telefono != null
                                        ? item.Telefono.ToString()
                                        : "",
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

            var conductor = new Persona
               {
                   Codigo = (EmpleadoBL.Instancia.MaxId() + 1).ToString(),
                   PersonaDirecciones = new List<PersonaDireccion>(),
                   PersonaDireccionEnt = new PersonaDireccion{ Ubigeo = new Ubigeo() },
                   PersonaContactos = new List<PersonaContacto>(),
                   PersonaFunciones = new List<PersonaFuncion>(),
                   TipoZonas = new BindingList<Comun>(),
                   TipoVias = new BindingList<Comun>()
               };

            PrepararDatos(ref conductor);
            return PartialView("ConductorPanel", conductor);
        }

        [HttpPost]
        public JsonResult Crear(Persona conductor)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var persona = new Persona
                                      {
                                          Nombres = conductor.Nombres,
                                          TipoPersona = (int)TipoPersona.Natural,
                                          Documento = conductor.Documento,
                                          TipoDocumento = conductor.TipoDocumento,
                                          Telefono = conductor.Telefono,
                                          Celular = conductor.Celular,
                                          IdEmpresa = IdEmpresa,
                                          ApellidoPaterno = conductor.ApellidoPaterno,
                                          ApellidoMaterno = conductor.ApellidoMaterno,
                                          FullName = string.Format("{0} {1} {2}", conductor.Nombres, conductor.ApellidoPaterno, conductor.ApellidoMaterno),
                                          Email = conductor.Email,
                                          NumeroLicencia = conductor.NumeroLicencia,
                                          Codigo = conductor.Codigo,
                                          EstadoCivil = conductor.EstadoCivil,
                                          Sexo = conductor.SexoC,
                                          FechaNacimiento = conductor.FechaNacimiento,
                                          FechaCreacion = FechaSistema,
                                          FechaModificacion = FechaSistema,
                                          UsuarioCreacion = UsuarioActual.IdEmpleado,
                                          UsuarioModificacion = UsuarioActual.IdEmpleado,
                                          Estado = conductor.Estado,
                                          PersonaContactos = new List<PersonaContacto>(),
                                          PersonaDirecciones = new List<PersonaDireccion>(),
                                          PersonaFunciones = new List<PersonaFuncion>(),
                                      };

                    persona.PersonaFunciones.Add(new PersonaFuncion { IdFuncion = (int)TipoFuncion.Conductor });
                    persona.PersonaDirecciones.Add(new PersonaDireccion
                    {
                        IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(conductor.IdDepartamento, conductor.IdProvincia,
                                     conductor.IdUbigeo),
                        TipoDireccion = conductor.PersonaDireccionEnt.TipoDireccion,
                        TipoVia = conductor.PersonaDireccionEnt.TipoVia,
                        TipoZona = conductor.PersonaDireccionEnt.TipoZona,
                        NombreVia = conductor.PersonaDireccionEnt.NombreVia,
                        FechaCreacion = FechaCreacion,
                        FechaModificacion = FechaModificacion,
                        UsuarioCreacion = UsuarioActual.IdEmpleado,
                        UsuarioModificacion = UsuarioActual.IdEmpleado,
                        NombreZona = conductor.PersonaDireccionEnt.NombreZona,
                        Numero = conductor.PersonaDireccionEnt.Numero,
                        Interior = conductor.PersonaDireccionEnt.Interior,
                        IdPersona = conductor.IdPersona,
                        Referencia = conductor.PersonaDireccionEnt.Referencia
                    });
                    if (conductor.PersonaDireccionEnt.TipoVia != null)
                        persona.Direccion =
                            ItemTablaBL.Instancia.Single((int)TipoTabla.TipoVia,
                                                         (int)conductor.PersonaDireccionEnt.TipoVia).Nombre + " " +
                            conductor.PersonaDireccionEnt.NombreVia + " #" + conductor.PersonaDireccionEnt.Numero + " " +
                            conductor.PersonaDireccionEnt.Interior;
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

                var conductor = PersonaBL.Instancia.Single(Convert.ToInt32(id));
                conductor.PersonaDireccionEnt = conductor.PersonaDirecciones.Count!=0 ? conductor.PersonaDirecciones.FirstOrDefault() : new PersonaDireccion();
                
                PrepararDatos(ref conductor);

                return PartialView("ConductorPanel", conductor);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Persona conductor)
        {
            var jsonResponse = new JsonResponse { Success = false };

            if (ModelState.IsValid)
            {
                try
                {
                    var conductorOriginal = PersonaBL.Instancia.Single(conductor.IdPersona);

                    conductorOriginal.TipoPersona = (int)TipoPersona.Natural;
                    conductorOriginal.Documento = conductor.Documento;
                    conductorOriginal.TipoDocumento = conductor.TipoDocumento;
                    conductorOriginal.Telefono = conductor.Telefono;
                    conductorOriginal.Celular = conductor.Celular;
                    conductorOriginal.IdEmpresa = IdEmpresa;
                    conductorOriginal.ApellidoPaterno = conductor.ApellidoPaterno;
                    conductorOriginal.ApellidoMaterno = conductor.ApellidoMaterno;
                    conductorOriginal.FullName = string.Format("{0} {1} {2}", conductor.Nombres,
                                                                     conductor.ApellidoPaterno,
                                                                     conductor.ApellidoMaterno);
                    conductorOriginal.Email = conductor.Email;
                    conductorOriginal.NumeroLicencia = conductor.NumeroLicencia;
                    conductorOriginal.Codigo = conductor.Codigo;
                    conductorOriginal.EstadoCivil = conductor.EstadoCivil;
                    conductorOriginal.Sexo = conductor.SexoC;
                    conductorOriginal.FechaNacimiento = conductor.FechaNacimiento;
                    conductorOriginal.FechaCreacion = FechaSistema;
                    conductorOriginal.FechaModificacion = FechaSistema;
                    conductorOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    conductorOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    conductorOriginal.Estado = conductor.Estado;
                    conductorOriginal.PersonaDirecciones.First().IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(conductor.IdDepartamento, conductor.IdProvincia, conductor.IdUbigeo);
                    conductorOriginal.PersonaDirecciones.First().TipoDireccion =
                        conductor.PersonaDireccionEnt.TipoDireccion;
                    conductorOriginal.PersonaDirecciones.First().TipoVia =
                        conductor.PersonaDireccionEnt.TipoVia;
                    conductorOriginal.PersonaDirecciones.First().TipoZona =
                        conductor.PersonaDireccionEnt.TipoZona;
                    conductorOriginal.PersonaDirecciones.First().NombreVia =
                        conductor.PersonaDireccionEnt.NombreVia;
                    conductorOriginal.PersonaDirecciones.First().FechaCreacion = FechaCreacion;
                    conductorOriginal.PersonaDirecciones.First().FechaModificacion = FechaModificacion;
                    conductorOriginal.PersonaDirecciones.First().UsuarioCreacion = UsuarioActual.IdEmpleado;
                    conductorOriginal.PersonaDirecciones.First().UsuarioModificacion = UsuarioActual.IdEmpleado;
                    conductorOriginal.PersonaDirecciones.First().NombreZona =
                        conductor.PersonaDireccionEnt.NombreZona;
                    conductorOriginal.PersonaDirecciones.First().Numero = conductor.PersonaDireccionEnt.Numero;
                    conductorOriginal.PersonaDirecciones.First().Interior =
                        conductor.PersonaDireccionEnt.Interior;
                    conductorOriginal.PersonaDirecciones.First().Referencia =
                        conductor.PersonaDireccionEnt.Referencia;
                    conductorOriginal.FechaModificacion = FechaModificacion;
                    conductorOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    conductorOriginal.PersonaFunciones = null;
                    if (conductor.PersonaDireccionEnt.TipoVia != null)
                        conductorOriginal.Direccion =
                            ItemTablaBL.Instancia.Single((int)TipoTabla.TipoVia,
                                                         (int)conductor.PersonaDireccionEnt.TipoVia).Nombre + " " +
                            conductor.PersonaDireccionEnt.NombreVia + " #" + conductor.PersonaDireccionEnt.Numero + " " +
                            conductor.PersonaDireccionEnt.Interior;
                  
                    PersonaBL.Instancia.Update(conductorOriginal);

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

        private void PrepararDatos(ref Persona conductor)
        {
            conductor.Estados = Utils.EnumToList<TipoEstado>();
            conductor.Cargos = CargoBL.Instancia.GetByEmpresa(IdEmpresa);
            conductor.EstadoCiviles = Utils.EnumToList<TipoEstadoCivil>();
            conductor.TipoDocumentos = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoDocumento);
            conductor.Sexos = Utils.EnumToList<TipoSexo>();
            conductor.TipoVias = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoVia);
            conductor.TipoZonas = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoZona);
            conductor.Departamentos = UbigeoBL.Instancia.GetAllDepartamentos();
            conductor.SexoC = Convert.ToInt32(conductor.Sexo);
            var personaDireccion = PersonaDireccionBL.Instancia.PrimeroDireccion(conductor.IdPersona);
            if (personaDireccion == null)
            {
                conductor.Departamentos.Insert(0, new Comun { Nombre = "-- Seleccionar --", Id = 0 });
                return;
            }

            conductor.PersonaDireccionEnt = personaDireccion;
            conductor.IdDepartamento = personaDireccion.Ubigeo.IdDepartamento;
            conductor.IdProvincia = personaDireccion.Ubigeo.IdProvincia;
            conductor.IdUbigeo = personaDireccion.Ubigeo.IdDistrito;
        }
    }
}