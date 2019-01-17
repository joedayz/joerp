
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

    public class ProveedorController : BaseController
    {
        public ActionResult Index()
        {
            return PartialView("ProveedorListado");
        }

        [HttpPost]
        public JsonResult Listar(GridTable grid)
        {
            var jqgrid = new JGrid();

            try
            {
                var nombreFiltros = new[] {"Codigo", "FullName", "Direccion", "Telefono", "Estado","IdFuncion" };
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

            var proveedor = new Persona
            {
                Codigo = (EmpleadoBL.Instancia.MaxId() + 1).ToString(),
                TipoDocumento = 4,
                PersonaDirecciones = new List<PersonaDireccion>(), 
                PersonaDireccionEnt = new PersonaDireccion{ Ubigeo = new Ubigeo() },
                PersonaContactos = new List<PersonaContacto>(), 
                PersonaFunciones= new List<PersonaFuncion>(), 
                TipoZonas = new BindingList<Comun>(),
                TipoVias = new BindingList<Comun>()
            };
            PrepararDatos(ref proveedor);
            return PartialView("ProveedorPanel", proveedor);
        }

        [HttpPost]
        public JsonResult Crear(Persona proveedor)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var persona = new Persona
                    {
                        TipoPersona = (int)TipoPersona.Juridica,
                        Documento = proveedor.Documento,
                        TipoDocumento = proveedor.TipoDocumento,
                        Telefono = proveedor.Telefono,
                        Celular = proveedor.Celular,
                        IdEmpresa = IdEmpresa,
                        RazonSocial = proveedor.RazonSocial,
                        FullName = proveedor.RazonSocial,
                        Email = proveedor.Email,
                        NumeroLicencia = proveedor.NumeroLicencia,
                        Codigo = proveedor.Codigo,
                        EstadoCivil = proveedor.EstadoCivil,
                        Sexo = proveedor.SexoC, 
                        FechaNacimiento = proveedor.FechaNacimiento,
                        FechaCreacion = FechaSistema,
                        FechaModificacion = FechaSistema,
                        UsuarioCreacion = UsuarioActual.IdEmpleado,
                        UsuarioModificacion = UsuarioActual.IdEmpleado,
                        Estado = proveedor.Estado,
                        Nombres = "-",
                        ApellidoMaterno = "-",
                        ApellidoPaterno = "-",
                        EsPercepcion = Convert.ToInt16(proveedor.EsPercepcionBool),
                        ActividadEconomica = proveedor.ActividadEconomica,
                        PersonaContactos = new List<PersonaContacto>(), 
                        PersonaDirecciones = new List<PersonaDireccion>(), 
                        PersonaFunciones = new List<PersonaFuncion>() 
                    };
                    persona.PersonaFunciones.Add(new PersonaFuncion { IdFuncion = (int)TipoFuncion.Proveedor});
                    persona.PersonaDirecciones.Add(new PersonaDireccion
                    {
                        IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(proveedor.IdDepartamento, proveedor.IdProvincia, proveedor.IdUbigeo),
                        TipoDireccion = proveedor.PersonaDireccionEnt.TipoDireccion,
                        TipoVia = proveedor.PersonaDireccionEnt.TipoVia,
                        TipoZona = proveedor.PersonaDireccionEnt.TipoZona,
                        NombreVia = proveedor.PersonaDireccionEnt.NombreVia,
                        FechaCreacion = FechaCreacion,
                        FechaModificacion = FechaModificacion,
                        UsuarioCreacion = UsuarioActual.IdEmpleado,
                        UsuarioModificacion = UsuarioActual.IdEmpleado,
                        NombreZona = proveedor.PersonaDireccionEnt.NombreZona,
                        Numero = proveedor.PersonaDireccionEnt.Numero,
                        Interior = proveedor.PersonaDireccionEnt.Interior,
                        IdPersona = proveedor.IdPersona,
                        Referencia = proveedor.PersonaDireccionEnt.Referencia
                    });
                    if (proveedor.PersonaDireccionEnt.TipoVia != null)
                        persona.Direccion =
                            ItemTablaBL.Instancia.Single((int) TipoTabla.TipoVia,
                                                         (int) proveedor.PersonaDireccionEnt.TipoVia).Nombre + " " +
                            proveedor.PersonaDireccionEnt.NombreVia + " #" + proveedor.PersonaDireccionEnt.Numero + " " +
                            proveedor.PersonaDireccionEnt.Interior;
                   
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

                var presentacion = PersonaBL.Instancia.Single(Convert.ToInt32(id));
                presentacion.EsPercepcionBool = Convert.ToBoolean(presentacion.EsPercepcion);
                PrepararDatos(ref presentacion);
                return PartialView("ProveedorPanel", presentacion);
            }
            catch (Exception ex)
            {
                return MensajeError(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Modificar(Persona proveedor)
        {
            var jsonResponse = new JsonResponse();

            if (ModelState.IsValid)
            {
                try
                {
                    var proveedorOriginal = PersonaBL.Instancia.Single(proveedor.IdPersona);
                    proveedorOriginal.TipoPersona = (int)TipoPersona.Juridica;
                    proveedorOriginal.ActividadEconomica = proveedor.ActividadEconomica;
                    proveedorOriginal.Documento = proveedor.Documento;
                    proveedorOriginal.TipoDocumento = proveedor.TipoDocumento;
                    proveedorOriginal.Telefono = proveedor.Telefono;
                    proveedorOriginal.Celular = proveedor.Celular;
                    proveedorOriginal.IdEmpresa = IdEmpresa;
                    proveedorOriginal.RazonSocial = proveedor.RazonSocial;
                    proveedorOriginal.FullName = proveedor.RazonSocial;
                    proveedorOriginal.Email = proveedor.Email;
                    proveedorOriginal.NumeroLicencia = proveedor.NumeroLicencia;
                    proveedorOriginal.Codigo = proveedor.Codigo;
                    proveedorOriginal.EstadoCivil = proveedor.EstadoCivil;
                    proveedorOriginal.Sexo = proveedor.SexoC; 
                    proveedorOriginal.FechaNacimiento = proveedor.FechaNacimiento;
                    proveedorOriginal.FechaCreacion = FechaSistema;
                    proveedorOriginal.FechaModificacion = FechaSistema;
                    proveedorOriginal.UsuarioCreacion = UsuarioActual.IdEmpleado;
                    proveedorOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;
                    proveedorOriginal.Estado = proveedor.Estado;
                    proveedorOriginal.EsPercepcion = Convert.ToInt16(proveedor.EsPercepcionBool);
                    if (proveedorOriginal.PersonaDirecciones.Count == 0)
                    {
                        proveedorOriginal.PersonaDirecciones.Add(new PersonaDireccion
                        {
                            IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(proveedor.IdDepartamento, proveedor.IdProvincia, proveedor.IdUbigeo),
                            TipoDireccion = proveedor.PersonaDireccionEnt.TipoDireccion,
                            TipoVia = proveedor.PersonaDireccionEnt.TipoVia,
                            TipoZona = proveedor.PersonaDireccionEnt.TipoZona,
                            NombreVia = proveedor.PersonaDireccionEnt.NombreVia,
                            FechaCreacion = FechaCreacion,
                            FechaModificacion = FechaModificacion,
                            UsuarioCreacion = UsuarioActual.IdEmpleado,
                            UsuarioModificacion = UsuarioActual.IdEmpleado,
                            NombreZona = proveedor.PersonaDireccionEnt.NombreZona,
                            Numero = proveedor.PersonaDireccionEnt.Numero,
                            Interior = proveedor.PersonaDireccionEnt.Interior,
                            IdPersona = proveedor.IdPersona,
                            Referencia = proveedor.PersonaDireccionEnt.Referencia
                        });
                    }
                    else
                    {
                        proveedorOriginal.PersonaDirecciones.First().IdUbigeo = UbigeoBL.Instancia.GetUbigeoId(proveedor.IdDepartamento, proveedor.IdProvincia, proveedor.IdUbigeo);
                        proveedorOriginal.PersonaDirecciones.First().TipoDireccion = proveedor.PersonaDireccionEnt.TipoDireccion;
                        proveedorOriginal.PersonaDirecciones.First().TipoVia = proveedor.PersonaDireccionEnt.TipoVia;
                        proveedorOriginal.PersonaDirecciones.First().TipoZona = proveedor.PersonaDireccionEnt.TipoZona;
                        proveedorOriginal.PersonaDirecciones.First().NombreVia = proveedor.PersonaDireccionEnt.NombreVia;
                        proveedorOriginal.PersonaDirecciones.First().FechaCreacion = FechaCreacion;
                        proveedorOriginal.PersonaDirecciones.First().FechaModificacion = FechaModificacion;
                        proveedorOriginal.PersonaDirecciones.First().UsuarioCreacion = UsuarioActual.IdEmpleado;
                        proveedorOriginal.PersonaDirecciones.First().UsuarioModificacion = UsuarioActual.IdEmpleado;
                        proveedorOriginal.PersonaDirecciones.First().NombreZona = proveedor.PersonaDireccionEnt.NombreZona;
                        proveedorOriginal.PersonaDirecciones.First().Numero = proveedor.PersonaDireccionEnt.Numero;
                        proveedorOriginal.PersonaDirecciones.First().Interior = proveedor.PersonaDireccionEnt.Interior;
                        proveedorOriginal.PersonaDirecciones.First().Referencia = proveedor.PersonaDireccionEnt.Referencia;
                    }
                    proveedorOriginal.PersonaDireccionEnt = proveedor.PersonaDireccionEnt;
                    proveedorOriginal.FechaModificacion = FechaModificacion;
                    proveedorOriginal.UsuarioModificacion = UsuarioActual.IdEmpleado;

                    proveedorOriginal.PersonaFunciones = PersonaFuncionBL.Instancia.ObtenerFunciones(proveedor.IdPersona);
                    proveedorOriginal.ProductosProveedor = new List<ProductoProveedor>();
                    if (proveedor.PersonaDireccionEnt.TipoVia != null)
                        proveedorOriginal.Direccion =
                            ItemTablaBL.Instancia.Single((int)TipoTabla.TipoVia, 
                            (int)proveedor.PersonaDireccionEnt.TipoVia).Nombre + " " +
                            proveedor.PersonaDireccionEnt.NombreVia + " #" + proveedor.PersonaDireccionEnt.Numero + " " +
                            proveedor.PersonaDireccionEnt.Interior;

                    PersonaBL.Instancia.Update(proveedorOriginal);

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

        private void PrepararDatos(ref Persona proveedor)
        {
            proveedor.Estados = Utils.EnumToList<TipoEstado>();
            proveedor.Cargos = CargoBL.Instancia.GetByEmpresa(IdEmpresa);
            proveedor.Actividades = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.ActividadEconomica);
            proveedor.EstadoCiviles = Utils.EnumToList<TipoEstadoCivil>();
            proveedor.TipoDocumentos = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoDocumento);
            proveedor.Sexos = Utils.EnumToList<TipoSexo>();
            proveedor.TipoVias = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoVia);
            proveedor.TipoZonas = ItemTablaBL.Instancia.ItemTablaToList((int)TipoTabla.TipoZona);
            proveedor.Departamentos = UbigeoBL.Instancia.GetAllDepartamentos();
            proveedor.SexoC = Convert.ToInt32(proveedor.Sexo);
            var temp = PersonaDireccionBL.Instancia.PrimeroDireccion(proveedor.IdPersona);
            
            if (temp == null)
            {
                proveedor.Departamentos.Insert(0, new Comun { Nombre = "-- Seleccionar --", Id = 0 });
                proveedor.PersonaDireccionEnt = new PersonaDireccion {TipoVia = 0};
                return;
            }
            proveedor.PersonaDireccionEnt = temp;
            proveedor.IdDepartamento = temp.Ubigeo.IdDepartamento;
            proveedor.IdProvincia = temp.Ubigeo.IdProvincia;
            proveedor.IdUbigeo = temp.Ubigeo.IdDistrito;
        }

        [HttpPost]
        public JsonResult Eliminar(string id)
        {
            var jsonResponse = new JsonResponse { Success = false };

            if (ModelState.IsValid)
            {
                try
                {
                    var personaId = Convert.ToInt32(id);
                    PersonaBL.Instancia.Delete(personaId);

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
