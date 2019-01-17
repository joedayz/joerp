
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(PersonaValidation))]
    public class Persona
    {
        public int IdPersona { get; set; }
        public string Nombres { get; set; }
        public int TipoPersona { get; set; }
        public string Documento { get; set; }
        public int TipoDocumento { get; set; }
        public short EsPercepcion { get; set; }
        public bool EsPercepcionBool { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public int IdEmpresa { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string RazonSocial { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string PaginaWeb { get; set; }
        public string CodigoPostal { get; set; }
        public int? ActividadEconomica { get; set; }
        public string NumeroLicencia { get; set; }
        public string Codigo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public int Estado { get; set; }
        public bool EsExtranjero { get; set; }
        public int? Pais { get; set; }
        public string Direccion { get; set; }
        public int? EstadoCivil { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int Sexo { get; set; }
        public Empleado Empleado { get; set; }
        public Empresa Empresa { get; set; }
        public IList<PersonaContacto> PersonaContactos { get; set; }
        public IList<PersonaContacto> PersonaContactos1 { get; set; }
        public IList<PersonaDireccion> PersonaDirecciones { get; set; }
        public IList<PersonaEntidadFinanciera> PersonaEntidadFinancieras { get; set; }
        public IList<PersonaFuncion> PersonaFunciones { get; set; }
        public IList<ProductoProveedor> ProductosProveedor { get; set; }
        
        public int IdDepartamento { get; set; }

        public int IdProvincia { get; set; }

        public int IdUbigeo { get; set; }

        public int SexoC { get; set; }

        public IList<Comun> Departamentos { get; set; }

        public IList<Comun> Estados { get; set; }

        public IList<Comun> EstadoCiviles { get; set; }

        public IList<Comun> Sexos { get; set; }

        public IList<Cargo> Cargos { get; set; }

        public IList<Comun> TipoDocumentos { get; set; }

        public IList<Comun> TipoDirecciones { get; set; }

        public IList<Comun> TipoZonas { get; set; }

        public IList<Comun> TipoVias { get; set; }

        public IList<Comun> Paises { get; set; }

        public PersonaDireccion PersonaDireccionEnt { set; get; }

        public string NombreEstado
        {
            get { return (Estado == (int)TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }

        public override string ToString()
        {
            return FullName;
        }
        public IList<Comun> Actividades { get; set; }

        public IList<Comun> TiposPersona { get; set; }
    }
}