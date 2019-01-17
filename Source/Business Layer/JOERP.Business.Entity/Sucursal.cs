
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(SucursalValidation))]
    public class Sucursal
    {
        private string _nombreEmpresa;

        public int IdSucursal { get; set; }
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; }
        public bool EsPrincipal { get; set; }
        public int IdUbigeo { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Empresa Empresa { get; set; }
        public Ubigeo Ubigeo { get; set; }

        public string NombreEstado
        {
            get { return (Estado == (int)TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }

        public string NombreEmpresa
        {
            get
            {
                return string.IsNullOrEmpty(_nombreEmpresa)
                           ? Empresa == null ? string.Empty : Empresa.RazonSocial
                           : _nombreEmpresa;
            }
            set { _nombreEmpresa = value; }
        }

        public List<Comun> Estados { get; set; }

        public List<Comun> Empresas { get; set; }

        public int IdDepartamento { get; set; }

        public int IdProvincia { get; set; }

        public IList<Comun> Departamentos { get; set; }

        public IList<Almacen> Almacen { get; set; }
    }
}