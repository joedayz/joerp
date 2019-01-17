
namespace JOERP.WebSite.Areas.Administracion.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Business.Entity;
    using Business.Logic;
    using Core;
    using Helpers;
    using Helpers.Enums;
    using Helpers.JqGrid;

    public class EmpleadoController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("EmpleadoListado");
        }

        public JsonResult GetByIdCargo(int idCargo)
        {
            var empleados = EmpleadoBL.Instancia.GetByIdCargo(idCargo);
            var listItems = Utils.ConvertToListItem(empleados, "IdEmpleado", "Persona");
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "NombrePersona", "NombreCargo", "FechaCreacion","Telefono", "Estado", "IdFuncion" };
                var lista = CrearJGrid(EmpleadoBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdEmpleado.ToString(),
                    cell = new[]
                                {
                                    item.IdEmpleado.ToString(),
                                    item.NombrePersona,
                                    item.NombreCargo,
                                    item.FechaCreacion.ToShortDateString(),
                                    item.NumeroTelefono,
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

            var entidad = new Empleado
            {
                Persona = new Persona
                        {
                            Codigo = (EmpleadoBL.Instancia.MaxId() + 1).ToString(),
                            Estado = (int)TipoEstado.Activo,
                            PersonaDirecciones = new List<PersonaDireccion>(),
                            PersonaDireccionEnt = new PersonaDireccion{ Ubigeo = new Ubigeo() },
                            PersonaContactos = new List<PersonaContacto>(),
                            PersonaFunciones = new List<PersonaFuncion>(),
                            TipoZonas = new BindingList<Comun>(),
                            TipoVias = new BindingList<Comun>()
                        }
            };

            PrepararDatos(ref entidad);
            return PartialView("EmpleadoPanel", entidad);
        }

        [HttpPost]
        public JsonResult Crear(Empleado entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var persona = new Persona
                    {
                        Nombres = entidad.Persona.Nombres,
                        TipoPersona = (int)TipoPersona.Natural,
                        Documento = entidad.Persona.Documento,
                        TipoDocumento = entidad.Persona.TipoDocumento,
                        Telefono = entidad.Persona.Telefono,
                        Celular = entidad.Persona.Celular,
                        IdEmpresa = IdEmpresa,
                        ApellidoPaterno = entidad.Persona.ApellidoPaterno,
                        ApellidoMaterno = entidad.Persona.ApellidoMaterno,
                        FullName = string.Format("{0} {1} {2}", entidad.Persona.Nombres, entidad.Persona.ApellidoPaterno, entidad.Persona.ApellidoMaterno),
                        Email = entidad.Persona.Email,
                        NumeroLicencia = entidad.Persona.NumeroLicencia,
                        Codigo = entidad.Persona.Codigo,
                        EstadoCivil = entidad.Persona.EstadoCivil,
                        Sexo =entidad.Persona.SexoC,
                        FechaNacimiento = entidad.Persona.FechaNacimiento,
                        FechaCreacion = FechaSistema,
                        FechaModificacion = FechaSistema,
                        UsuarioCreacion = UsuarioActual.IdEmpleado,
                        UsuarioModificacion = UsuarioActual.IdEmpleado,
                        Estado = entidad.Persona.Estado,
                    };
                    persona.PersonaContactos = new List<PersonaContacto>();
                    persona.PersonaDirecciones = new List<PersonaDireccion>();
                    persona.PersonaDirecciones.Add(new PersonaDireccion
                                                     {
                                                         IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(entidad.Persona.IdDepartamento, entidad.Persona.IdProvincia,entidad.Persona.IdUbigeo),
                                                         TipoDireccion = entidad.Persona.PersonaDireccionEnt.TipoDireccion,
                                                         TipoVia = entidad.Persona.PersonaDireccionEnt.TipoVia,
                                                         TipoZona = entidad.Persona.PersonaDireccionEnt.TipoZona,
                                                         NombreVia = entidad.Persona.PersonaDireccionEnt.NombreVia,
                                                         FechaCreacion = FechaCreacion,
                                                         FechaModificacion = FechaModificacion,
                                                         UsuarioCreacion = UsuarioActual.IdEmpleado,
                                                         UsuarioModificacion = UsuarioActual.IdEmpleado,
                                                         NombreZona = entidad.Persona.PersonaDireccionEnt.NombreZona,
                                                         Numero = entidad.Persona.PersonaDireccionEnt.Numero,
                                                         Interior = entidad.Persona.PersonaDireccionEnt.Interior,
                                                         IdPersona = entidad.Persona.IdPersona,
                                                         Referencia = entidad.Persona.PersonaDireccionEnt.Referencia
                                                     });
                    persona.PersonaFunciones = new List<PersonaFuncion>();
                    foreach (var funcion in entidad.Funciones)
                    {
                        if (funcion.Seleccionado)
                            persona.PersonaFunciones.Add(new PersonaFuncion
                                                           {
                                                               IdFuncion = funcion.IdFuncion
                                                           });
                    }

                    var empleado = new Empleado
                                       {
                                           Persona = persona,
                                           IdCargo = entidad.IdCargo,
                                           FechaCreacion = FechaCreacion,
                                           FechaModificacion = FechaModificacion,
                                           UsuarioCreacion = UsuarioActual.IdEmpleado,
                                           UsuarioModificacion = UsuarioActual.IdEmpleado
                                       };

                    var personaTemp = PersonaBL.Instancia.Add(persona);
                    empleado.IdEmpleado = personaTemp.IdPersona;
                    EmpleadoBL.Instancia.Add(empleado);

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
                var entidad = EmpleadoBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref entidad);
                return PartialView("EmpleadoPanel", entidad);
            }
            catch
            {
                return MensajeError();
            }
        }

        [HttpPost]
        public JsonResult Modificar(Empleado entidad)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var entidadOriginal = EmpleadoBL.Instancia.Single(entidad.IdEmpleado);
                    entidadOriginal.Persona.PersonaDirecciones = PersonaDireccionBL.Instancia.GetByIdPersona(entidadOriginal.Persona.IdPersona);
                    entidadOriginal.Persona.Nombres = entidad.Persona.Nombres;
                    entidadOriginal.Persona.TipoPersona = (int)TipoPersona.Natural;
                    entidadOriginal.Persona.Documento = entidad.Persona.Documento;
                    entidadOriginal.Persona.TipoDocumento = entidad.Persona.TipoDocumento;
                    entidadOriginal.Persona.Telefono = entidad.Persona.Telefono;
                    entidadOriginal.Persona.Celular = entidad.Persona.Celular;
                    entidadOriginal.Persona.IdEmpresa = IdEmpresa;
                    entidadOriginal.Persona.ApellidoPaterno = entidad.Persona.ApellidoPaterno;
                    entidadOriginal.Persona.ApellidoMaterno = entidad.Persona.ApellidoMaterno;
                    entidadOriginal.Persona.FullName = string.Format("{0} {1} {2}", entidad.Persona.Nombres,
                                                                     entidad.Persona.ApellidoPaterno,
                                                                     entidad.Persona.ApellidoMaterno);
                    entidadOriginal.Persona.Email = entidad.Persona.Email;
                    entidadOriginal.Persona.NumeroLicencia = entidad.Persona.NumeroLicencia;
                    entidadOriginal.Persona.Codigo = entidad.Persona.Codigo;
                    entidadOriginal.Persona.EstadoCivil = entidad.Persona.EstadoCivil;
                    entidadOriginal.Persona.Sexo = entidad.Persona.SexoC;
                    entidadOriginal.Persona.FechaNacimiento = entidad.Persona.FechaNacimiento;
                    entidadOriginal.Persona.FechaCreacion = FechaSistema;
                    entidadOriginal.Persona.FechaModificacion = FechaSistema;
                    entidadOriginal.Persona.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    entidadOriginal.Persona.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    entidadOriginal.Persona.Estado = entidad.Persona.Estado;

                    if (entidadOriginal.Persona.PersonaDirecciones.Count == 0)
                    {
                        var direccion = new PersonaDireccion();
                        direccion.IdUbigeo = entidad.Persona.IdUbigeo;
                        direccion.TipoDireccion = entidad.Persona.PersonaDireccionEnt.TipoDireccion;
                        direccion.TipoVia = entidad.Persona.PersonaDireccionEnt.TipoVia;
                        direccion.TipoZona = entidad.Persona.PersonaDireccionEnt.TipoZona;
                        direccion.NombreVia = entidad.Persona.PersonaDireccionEnt.NombreVia;
                        direccion.FechaCreacion = FechaCreacion;
                        direccion.FechaModificacion = FechaModificacion;
                        direccion.UsuarioCreacion = UsuarioActual.IdEmpleado;
                        direccion.UsuarioModificacion = UsuarioActual.IdEmpleado;
                        direccion.NombreZona = entidad.Persona.PersonaDireccionEnt.NombreZona;
                        direccion.Numero = entidad.Persona.PersonaDireccionEnt.Numero;
                        direccion.Interior = entidad.Persona.PersonaDireccionEnt.Interior;
                        direccion.Referencia = entidad.Persona.PersonaDireccionEnt.Referencia;

                        entidadOriginal.Persona.PersonaDirecciones.Add(direccion);
                    }
                    else
                    {
                        entidadOriginal.Persona.PersonaDirecciones.First().IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(entidad.Persona.IdDepartamento, entidad.Persona.IdProvincia, entidad.Persona.IdUbigeo);
                        entidadOriginal.Persona.PersonaDirecciones.First().TipoDireccion = entidad.Persona.PersonaDireccionEnt.TipoDireccion;
                        entidadOriginal.Persona.PersonaDirecciones.First().TipoVia = entidad.Persona.PersonaDireccionEnt.TipoVia;
                        entidadOriginal.Persona.PersonaDirecciones.First().TipoZona = entidad.Persona.PersonaDireccionEnt.TipoZona;
                        entidadOriginal.Persona.PersonaDirecciones.First().NombreVia = entidad.Persona.PersonaDireccionEnt.NombreVia;
                        entidadOriginal.Persona.PersonaDirecciones.First().FechaCreacion = FechaCreacion;
                        entidadOriginal.Persona.PersonaDirecciones.First().FechaModificacion = FechaModificacion;
                        entidadOriginal.Persona.PersonaDirecciones.First().UsuarioCreacion = UsuarioActual.IdEmpleado;
                        entidadOriginal.Persona.PersonaDirecciones.First().UsuarioModificacion = UsuarioActual.IdEmpleado;
                        entidadOriginal.Persona.PersonaDirecciones.First().NombreZona = entidad.Persona.PersonaDireccionEnt.NombreZona;
                        entidadOriginal.Persona.PersonaDirecciones.First().Numero = entidad.Persona.PersonaDireccionEnt.Numero;
                        entidadOriginal.Persona.PersonaDirecciones.First().Interior = entidad.Persona.PersonaDireccionEnt.Interior;
                        entidadOriginal.Persona.PersonaDirecciones.First().Referencia = entidad.Persona.PersonaDireccionEnt.Referencia;   
                    }

                    entidadOriginal.Persona.PersonaFunciones.Clear();
                    foreach (var funcion in entidad.Funciones)
                    {
                        if (funcion.Seleccionado)
                            entidadOriginal.Persona.PersonaFunciones.Add(new PersonaFuncion
                            {
                                IdFuncion = funcion.IdFuncion
                            });
                    }

                    entidadOriginal.Persona.PersonaDireccionEnt = entidad.Persona.PersonaDireccionEnt;
                    entidadOriginal.IdCargo = entidad.IdCargo;
                    entidadOriginal.Persona.FechaModificacion = FechaModificacion;
                    entidadOriginal.Persona.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    entidadOriginal.FechaModificacion = FechaModificacion;
                    entidadOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    PersonaBL.Instancia.Update(entidadOriginal.Persona);
                    EmpleadoBL.Instancia.Update(entidadOriginal);
                    
                    jsonResponse.Success = true;
                    jsonResponse.Message = "Se Proceso con exito";
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

        private void PrepararDatos(ref Empleado empleado)
        {
            empleado.Persona.Estados = Utils.EnumToList<TipoEstado>();
            empleado.Persona.Cargos = CargoBL.Instancia.GetByEmpresa(IdEmpresa);
            empleado.Persona.Cargos.Add(new Cargo { IdCargo = 0, Nombre = "-- Seleccionar --"});
            empleado.Persona.EstadoCiviles = Utils.EnumToList<TipoEstadoCivil>();
            empleado.Persona.TipoDocumentos = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoDocumento);
            empleado.Persona.Sexos = Utils.EnumToList<TipoSexo>();
            empleado.Persona.TipoVias = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoVia);
            empleado.Persona.TipoZonas = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoZona);
            empleado.Persona.Departamentos = UbigeoBL.Instancia.GetAllDepartamentos();
            empleado.Persona.SexoC = Convert.ToInt32(empleado.Persona.Sexo);
            empleado.Funciones = new ObservableCollection<PersonaFuncion>();

            foreach (var tipo in Utils.EnumToList<TipoFuncion>())
            {
                var funcion = empleado.Persona.PersonaFunciones.SingleOrDefault(p => p.IdFuncion == tipo.Id);
                var seleccionado = funcion != null;

                empleado.Funciones.Add(new PersonaFuncion { IdFuncion = tipo.Id, Seleccionado = seleccionado });
            }

            var personaDireccion = PersonaDireccionBL.Instancia.PrimeroDireccion(empleado.Persona.IdPersona);
            if (personaDireccion == null)
            {
                empleado.Persona.PersonaDireccionEnt = new PersonaDireccion();
                empleado.Persona.Departamentos.Insert(0, new Comun { Nombre = "-- Seleccionar --", Id = 0 });
                return;
            }
            empleado.Persona.PersonaDireccionEnt = personaDireccion;
            empleado.Persona.IdDepartamento = personaDireccion.Ubigeo.IdDepartamento;
            empleado.Persona.IdProvincia = personaDireccion.Ubigeo.IdProvincia;
            empleado.Persona.IdUbigeo = personaDireccion.Ubigeo.IdDistrito;
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var empleadoId = Convert.ToInt32(id);
                    EmpleadoBL.Instancia.Delete(empleadoId);

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
                jsonResponse.Message = "No se puedo eliminar.";
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }
    }
}