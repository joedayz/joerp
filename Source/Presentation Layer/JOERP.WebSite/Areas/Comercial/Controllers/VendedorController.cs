
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

    public class VendedorController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("VendedorListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] {"Codigo", "FullName", "Direccion", "Telefono", "Estado", "IdFuncion" };
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

            var vendedor = new Persona
            {
                Codigo = (EmpleadoBL.Instancia.MaxId() + 1).ToString(),
                PersonaDirecciones = new List<PersonaDireccion>(),
                PersonaDireccionEnt = new PersonaDireccion{ Ubigeo = new Ubigeo() },
                PersonaContactos = new List<PersonaContacto>(),
                PersonaFunciones = new List<PersonaFuncion>(),
                TipoZonas = new BindingList<Comun>(),
                TipoVias = new BindingList<Comun>()
            };

            PrepararDatos(ref vendedor);
            return PartialView("VendedorPanel", vendedor);
        }

        [HttpPost]
        public JsonResult Crear(Persona vendedor)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var persona = new Persona
                    {
                        Nombres = vendedor.Nombres,
                        TipoPersona = (int)TipoPersona.Natural,
                        Documento = vendedor.Documento,
                        TipoDocumento = vendedor.TipoDocumento,
                        Telefono = vendedor.Telefono,
                        Celular = vendedor.Celular,
                        IdEmpresa = IdEmpresa,
                        ApellidoPaterno = vendedor.ApellidoPaterno,
                        ApellidoMaterno = vendedor.ApellidoMaterno,
                        FullName = string.Format("{0} {1} {2}", vendedor.Nombres, vendedor.ApellidoPaterno, vendedor.ApellidoMaterno),
                        Email = vendedor.Email,
                        NumeroLicencia = vendedor.NumeroLicencia,
                        Codigo = vendedor.Codigo,
                        EstadoCivil = vendedor.EstadoCivil,
                        Sexo = vendedor.SexoC,
                        FechaNacimiento = vendedor.FechaNacimiento,
                        FechaCreacion = FechaSistema,
                        FechaModificacion = FechaSistema,
                        UsuarioCreacion = UsuarioActual.IdEmpleado,
                        UsuarioModificacion = UsuarioActual.IdEmpleado,
                        Estado = vendedor.Estado,
                        PersonaContactos = new List<PersonaContacto>(),
                        PersonaDirecciones = new List<PersonaDireccion>(),
                        PersonaFunciones = new List<PersonaFuncion>()
                    };

                    persona.PersonaFunciones.Add(new PersonaFuncion { IdFuncion = (int)TipoFuncion.Vendedor});
                    persona.PersonaDirecciones.Add(new PersonaDireccion
                    {
                        IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(vendedor.IdDepartamento, vendedor.IdProvincia, vendedor.IdUbigeo),
                        TipoDireccion = vendedor.PersonaDireccionEnt.TipoDireccion,
                        TipoVia = vendedor.PersonaDireccionEnt.TipoVia,
                        TipoZona = vendedor.PersonaDireccionEnt.TipoZona,
                        NombreVia = vendedor.PersonaDireccionEnt.NombreVia,
                        FechaCreacion = FechaCreacion,
                        FechaModificacion = FechaModificacion,
                        UsuarioCreacion = UsuarioActual.IdEmpleado,
                        UsuarioModificacion = UsuarioActual.IdEmpleado,
                        NombreZona = vendedor.PersonaDireccionEnt.NombreZona,
                        Numero = vendedor.PersonaDireccionEnt.Numero,
                        Interior = vendedor.PersonaDireccionEnt.Interior,
                        IdPersona = vendedor.IdPersona,
                        Referencia = vendedor.PersonaDireccionEnt.Referencia
                    });
                    if (vendedor.PersonaDireccionEnt.TipoVia != null)
                        persona.Direccion = ItemTablaBL.Instancia.Single((int)TipoTabla.TipoVia, 
                            (int)vendedor.PersonaDireccionEnt.TipoVia).Nombre + " " +
                            vendedor.PersonaDireccionEnt.NombreVia + " #" + vendedor.PersonaDireccionEnt.Numero + " " +
                            vendedor.PersonaDireccionEnt.Interior;

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

                var entidad = PersonaBL.Instancia.Single(Convert.ToInt32(id));
                PrepararDatos(ref entidad);
                return PartialView("VendedorPanel", entidad);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Persona vendedor)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var vendedorOriginal = PersonaBL.Instancia.Single(vendedor.IdPersona);

                    vendedorOriginal.TipoPersona = (int)TipoPersona.Natural;
                    vendedorOriginal.Documento = vendedor.Documento;
                    vendedorOriginal.TipoDocumento = vendedor.TipoDocumento;
                    vendedorOriginal.Telefono = vendedor.Telefono;
                    vendedorOriginal.Celular = vendedor.Celular;
                    vendedorOriginal.IdEmpresa = IdEmpresa;
                    vendedorOriginal.ApellidoPaterno = vendedor.ApellidoPaterno;
                    vendedorOriginal.ApellidoMaterno = vendedor.ApellidoMaterno;
                    vendedorOriginal.FullName = string.Format("{0} {1} {2}", vendedor.Nombres,
                                                                     vendedor.ApellidoPaterno,
                                                                     vendedor.ApellidoMaterno);
                    vendedorOriginal.Email = vendedor.Email;
                    vendedorOriginal.NumeroLicencia = vendedor.NumeroLicencia;
                    vendedorOriginal.Codigo = vendedor.Codigo;
                    vendedorOriginal.EstadoCivil = vendedor.EstadoCivil;
                    vendedorOriginal.Sexo = vendedor.SexoC;
                    vendedorOriginal.FechaNacimiento = vendedor.FechaNacimiento;
                    vendedorOriginal.FechaCreacion = FechaSistema;
                    vendedorOriginal.FechaModificacion = FechaSistema;
                    vendedorOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    vendedorOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    vendedorOriginal.Estado = vendedor.Estado;
                    vendedorOriginal.PersonaDirecciones.First().IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(vendedor.IdDepartamento, vendedor.IdProvincia, vendedor.IdUbigeo);
                    vendedorOriginal.PersonaDirecciones.First().TipoDireccion = vendedor.PersonaDireccionEnt.TipoDireccion;
                    vendedorOriginal.PersonaDirecciones.First().TipoVia = vendedor.PersonaDireccionEnt.TipoVia;
                    vendedorOriginal.PersonaDirecciones.First().TipoZona = vendedor.PersonaDireccionEnt.TipoZona;
                    vendedorOriginal.PersonaDirecciones.First().NombreVia = vendedor.PersonaDireccionEnt.NombreVia;
                    vendedorOriginal.PersonaDirecciones.First().FechaCreacion = FechaCreacion;
                    vendedorOriginal.PersonaDirecciones.First().FechaModificacion = FechaModificacion;
                    vendedorOriginal.PersonaDirecciones.First().UsuarioCreacion = UsuarioActual.IdEmpleado;
                    vendedorOriginal.PersonaDirecciones.First().UsuarioModificacion = UsuarioActual.IdEmpleado;
                    vendedorOriginal.PersonaDirecciones.First().NombreZona = vendedor.PersonaDireccionEnt.NombreZona;
                    vendedorOriginal.PersonaDirecciones.First().Numero = vendedor.PersonaDireccionEnt.Numero;
                    vendedorOriginal.PersonaDirecciones.First().Interior = vendedor.PersonaDireccionEnt.Interior;
                    vendedorOriginal.PersonaDirecciones.First().Referencia = vendedor.PersonaDireccionEnt.Referencia;
                    vendedorOriginal.FechaModificacion = FechaModificacion;
                    vendedorOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    vendedorOriginal.PersonaFunciones = null;

                    if (vendedor.PersonaDireccionEnt.TipoVia != null)
                        vendedorOriginal.Direccion = ItemTablaBL.Instancia.Single((int)TipoTabla.TipoVia, 
                            (int)vendedor.PersonaDireccionEnt.TipoVia).Nombre + " " +
                            vendedor.PersonaDireccionEnt.NombreVia + " #" + vendedor.PersonaDireccionEnt.Numero + " " +
                            vendedor.PersonaDireccionEnt.Interior;

                    PersonaBL.Instancia.Update(vendedorOriginal);

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

        private void PrepararDatos(ref Persona vendedor)
        {
            vendedor.Estados = Utils.EnumToList<TipoEstado>();
            vendedor.Cargos = CargoBL.Instancia.GetByEmpresa(IdEmpresa);
            vendedor.EstadoCiviles = Utils.EnumToList<TipoEstadoCivil>();
            vendedor.TipoDocumentos = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoDocumento);
            vendedor.Sexos = Utils.EnumToList<TipoSexo>();
            vendedor.TipoVias = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoVia);
            vendedor.TipoZonas = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoZona);
            vendedor.Departamentos = UbigeoBL.Instancia.GetAllDepartamentos();
            vendedor.SexoC = Convert.ToInt32(vendedor.Sexo);
           
            var temp = PersonaDireccionBL.Instancia.PrimeroDireccion(vendedor.IdPersona);
            if (temp == null)
            {
                vendedor.PersonaDireccionEnt = new PersonaDireccion();
                vendedor.Departamentos.Insert(0, new Comun { Nombre = "-- Seleccionar --", Id = 0 });
                return;
            }
            vendedor.PersonaDireccionEnt = temp;
            vendedor.IdDepartamento = temp.Ubigeo.IdDepartamento;
            vendedor.IdProvincia = temp.Ubigeo.IdProvincia;
            vendedor.IdUbigeo = temp.Ubigeo.IdDistrito;
        }
    }
}