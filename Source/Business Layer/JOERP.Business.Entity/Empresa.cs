
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof (EmpresaValidation))]
    public class Empresa
    {
        public int IdEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public string RUC { get; set; }
        public string Direccion { get; set; }
        public int Estado { get; set; }
        public string CodigoPostal { get; set; }
        public int IdUbigeo { get; set; }
        public int? ActividadEconomica { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string Fax { get; set; }
        public string PaginaWeb { get; set; }
        public string CorreoElectronico { get; set; }
        public int? TipoContribuyente { get; set; }
        public DateTime? FechaInscripcion { get; set; }
        public DateTime? FechaActividades { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public bool EsPercepcion { get; set; }
        public Ubigeo Ubigeo { get; set; }

        public string NombreEstado
        {
            get { return (Estado == (int) TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }

        public List<Comun> Estados { get; set; }

        public int IdDepartamento { get; set; }

        public int IdProvincia { get; set; }

        public IList<Comun> Departamentos { get; set; }

        public IList<Comun> Actividades { get; set; }

        public IList<Comun> Contribuyentes { get; set; }
    }
}