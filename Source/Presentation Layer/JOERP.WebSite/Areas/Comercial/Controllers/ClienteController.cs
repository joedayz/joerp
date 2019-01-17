
namespace JOERP.WebSite.Areas.Comercial.Controllers
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

    public class ClienteController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("ClienteListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] { "Codigo", "FullName", "Direccion", "Telefono", "Estado", "IdFuncion" };
                var lista = CrearJGrid(PersonaBL.Instancia, grid, nombreFiltros, ref jqgrid);

                jqgrid.rows = lista.Select(item => new JRow
                {
                    id = item.IdPersona.ToString(),
                    cell = new[]
                                {
                                    item.IdPersona.ToString(),
                                    item.Codigo,
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

            var cliente = new Persona
            {
                Codigo = (EmpleadoBL.Instancia.MaxId() + 1).ToString(),
                PersonaDirecciones = new List<PersonaDireccion>(),
                PersonaDireccionEnt = new PersonaDireccion{ Ubigeo = new Ubigeo() },
                PersonaContactos = new List<PersonaContacto>(),
                PersonaFunciones = new List<PersonaFuncion>(),
                TipoZonas = new BindingList<Comun>(),
                TipoVias = new BindingList<Comun>()
            };

            PrepararDatos(ref cliente);
            return PartialView("ClientePanel", cliente);
        }

        [HttpPost]
        public JsonResult Crear(Persona cliente)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var persona = new Persona
                    {
                        Nombres = cliente.Nombres,
                        TipoPersona = cliente.TipoPersona,
                        Documento = cliente.Documento,
                        TipoDocumento = cliente.TipoDocumento,
                        Telefono = cliente.Telefono,
                        Celular = cliente.Celular,
                        IdEmpresa = IdEmpresa,
                        ApellidoPaterno = cliente.ApellidoPaterno,
                        ApellidoMaterno = cliente.ApellidoMaterno,
                        RazonSocial = cliente.RazonSocial,
                        FullName = cliente.TipoPersona == (int)TipoPersona.Natural ? string.Format("{0} {1} {2}", cliente.Nombres, cliente.ApellidoPaterno,
                                          cliente.ApellidoMaterno) : cliente.RazonSocial,
                        Email = cliente.Email,
                        NumeroLicencia = cliente.NumeroLicencia,
                        Codigo = cliente.Codigo,
                        EstadoCivil = cliente.EstadoCivil,
                        Sexo = cliente.SexoC,
                        FechaNacimiento = cliente.FechaNacimiento,
                        FechaCreacion = FechaSistema,
                        FechaModificacion = FechaSistema,
                        UsuarioCreacion = UsuarioActual.IdEmpleado,
                        UsuarioModificacion = UsuarioActual.IdEmpleado,
                        Estado = cliente.Estado,
                        PersonaContactos = new List<PersonaContacto>(),
                        PersonaDirecciones = new List<PersonaDireccion>(),
                        PersonaFunciones = new List<PersonaFuncion>()
                    };

                    persona.PersonaFunciones.Add(new PersonaFuncion { IdFuncion = (int)TipoFuncion.Cliente });
                    persona.PersonaDirecciones.Add(new PersonaDireccion
                    {
                        IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(cliente.IdDepartamento, cliente.IdProvincia, cliente.IdUbigeo),
                        TipoDireccion = cliente.PersonaDireccionEnt.TipoDireccion,
                        TipoVia = cliente.PersonaDireccionEnt.TipoVia,
                        TipoZona = cliente.PersonaDireccionEnt.TipoZona,
                        NombreVia = cliente.PersonaDireccionEnt.NombreVia,
                        FechaCreacion = FechaCreacion,
                        FechaModificacion = FechaModificacion,
                        UsuarioCreacion = UsuarioActual.IdEmpleado,
                        UsuarioModificacion = UsuarioActual.IdEmpleado,
                        NombreZona = cliente.PersonaDireccionEnt.NombreZona,
                        Numero = cliente.PersonaDireccionEnt.Numero,
                        Interior = cliente.PersonaDireccionEnt.Interior,
                        IdPersona = cliente.IdPersona,
                        Referencia = cliente.PersonaDireccionEnt.Referencia
                    });
                    if (cliente.PersonaDireccionEnt.TipoVia != null)
                        persona.Direccion = ItemTablaBL.Instancia.Single((int)TipoTabla.TipoVia, 
                            (int)cliente.PersonaDireccionEnt.TipoVia).Nombre + " " +
                            cliente.PersonaDireccionEnt.NombreVia + " #" + cliente.PersonaDireccionEnt.Numero + " " +
                            cliente.PersonaDireccionEnt.Interior;

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

                var cliente = PersonaBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref cliente);
                return PartialView("ClientePanel", cliente);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Persona cliente)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var clienteOriginal = PersonaBL.Instancia.Single(cliente.IdPersona);

                    clienteOriginal.TipoPersona = cliente.TipoPersona;
                    clienteOriginal.Documento = cliente.Documento;
                    clienteOriginal.TipoDocumento = cliente.TipoDocumento;
                    clienteOriginal.Telefono = cliente.Telefono;
                    clienteOriginal.Celular = cliente.Celular;
                    clienteOriginal.IdEmpresa = IdEmpresa;
                    clienteOriginal.ApellidoPaterno = cliente.ApellidoPaterno;
                    clienteOriginal.ApellidoMaterno = cliente.ApellidoMaterno;
                    clienteOriginal.FullName = cliente.TipoPersona == (int)TipoPersona.Natural
                                   ? string.Format("{0} {1} {2}", cliente.Nombres, cliente.ApellidoPaterno, cliente.ApellidoMaterno)
                                   : cliente.RazonSocial;
                    clienteOriginal.Email = cliente.Email;
                    clienteOriginal.NumeroLicencia = cliente.NumeroLicencia;
                    clienteOriginal.Codigo = cliente.Codigo;
                    clienteOriginal.EstadoCivil = cliente.EstadoCivil;
                    clienteOriginal.Sexo = cliente.SexoC;
                    clienteOriginal.FechaNacimiento = cliente.FechaNacimiento;
                    clienteOriginal.FechaCreacion = FechaSistema;
                    clienteOriginal.FechaModificacion = FechaSistema;
                    clienteOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    clienteOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    clienteOriginal.Estado = cliente.Estado;

                    if (clienteOriginal.PersonaDirecciones.Count == 0)
                    {
                        clienteOriginal.PersonaDirecciones.Add(new PersonaDireccion
                        {
                            IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(cliente.IdDepartamento, cliente.IdProvincia, cliente.IdUbigeo),
                            TipoDireccion = cliente.PersonaDireccionEnt.TipoDireccion,
                            TipoVia = cliente.PersonaDireccionEnt.TipoVia,
                            TipoZona = cliente.PersonaDireccionEnt.TipoZona,
                            NombreVia = cliente.PersonaDireccionEnt.NombreVia,
                            FechaCreacion = FechaCreacion,
                            FechaModificacion = FechaModificacion,
                            UsuarioCreacion = UsuarioActual.IdEmpleado,
                            UsuarioModificacion = UsuarioActual.IdEmpleado,
                            NombreZona = cliente.PersonaDireccionEnt.NombreZona,
                            Numero = cliente.PersonaDireccionEnt.Numero,
                            Interior = cliente.PersonaDireccionEnt.Interior,
                            IdPersona = cliente.IdPersona,
                            Referencia = cliente.PersonaDireccionEnt.Referencia
                        });
                    }
                    else
                    {
                        clienteOriginal.PersonaDirecciones.First().IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(cliente.IdDepartamento, cliente.IdProvincia, cliente.IdUbigeo);
                        clienteOriginal.PersonaDirecciones.First().TipoDireccion = cliente.PersonaDireccionEnt.TipoDireccion;
                        clienteOriginal.PersonaDirecciones.First().TipoVia = cliente.PersonaDireccionEnt.TipoVia;
                        clienteOriginal.PersonaDirecciones.First().TipoZona = cliente.PersonaDireccionEnt.TipoZona;
                        clienteOriginal.PersonaDirecciones.First().NombreVia = cliente.PersonaDireccionEnt.NombreVia;
                        clienteOriginal.PersonaDirecciones.First().FechaCreacion = FechaCreacion;
                        clienteOriginal.PersonaDirecciones.First().FechaModificacion = FechaModificacion;
                        clienteOriginal.PersonaDirecciones.First().UsuarioCreacion = UsuarioActual.IdEmpleado;
                        clienteOriginal.PersonaDirecciones.First().UsuarioModificacion = UsuarioActual.IdEmpleado;
                        clienteOriginal.PersonaDirecciones.First().NombreZona = cliente.PersonaDireccionEnt.NombreZona;
                        clienteOriginal.PersonaDirecciones.First().Numero = cliente.PersonaDireccionEnt.Numero;
                        clienteOriginal.PersonaDirecciones.First().Interior = cliente.PersonaDireccionEnt.Interior;
                        clienteOriginal.PersonaDirecciones.First().Referencia = cliente.PersonaDireccionEnt.Referencia;
                    }
                    clienteOriginal.FechaModificacion = FechaModificacion;
                    clienteOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    clienteOriginal.PersonaFunciones = null;
                    if (cliente.PersonaDireccionEnt.TipoVia != null)
                        clienteOriginal.Direccion = ItemTablaBL.Instancia.Single((int)TipoTabla.TipoVia, 
                            (int)cliente.PersonaDireccionEnt.TipoVia).Nombre + " " +
                            cliente.PersonaDireccionEnt.NombreVia + " #" + cliente.PersonaDireccionEnt.Numero + " " +
                            cliente.PersonaDireccionEnt.Interior;

                    PersonaBL.Instancia.Update(clienteOriginal);

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

        private void PrepararDatos(ref Persona cliente)
        {
            cliente.Estados = Utils.EnumToList<TipoEstado>();
            cliente.TiposPersona = Utils.EnumToList<TipoPersona>();
            cliente.Cargos = CargoBL.Instancia.GetByEmpresa(IdEmpresa);
            cliente.EstadoCiviles = Utils.EnumToList<TipoEstadoCivil>();
            cliente.TipoDocumentos = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoDocumento);
            cliente.Sexos = Utils.EnumToList<TipoSexo>();
            cliente.TipoVias = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoVia);
            cliente.TipoZonas = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoZona);
            cliente.Departamentos = UbigeoBL.Instancia.GetAllDepartamentos();
            cliente.SexoC = Convert.ToInt32(cliente.Sexo);
            var temp = PersonaDireccionBL.Instancia.PrimeroDireccion(cliente.IdPersona);
            if (temp == null)
            {
                cliente.PersonaDireccionEnt = new PersonaDireccion();
                cliente.Departamentos.Insert(0, new Comun { Nombre = "-- Seleccionar --", Id = 0 });
                return;
            }
            cliente.PersonaDireccionEnt = temp;
            cliente.IdDepartamento = temp.Ubigeo.IdDepartamento;
            cliente.IdProvincia = temp.Ubigeo.IdProvincia;
            cliente.IdUbigeo = temp.Ubigeo.IdDistrito;
        }
    }
}