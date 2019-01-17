
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(UsuarioValidation))]
    public class Usuario
    {
        public Usuario()
        {
            PermisosUsuario = new List<PermisoUsuario>();
        }

        public int IdEmpleado { get; set; }
        public int IdRol { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModiifacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Empleado Empleado { get; set; }
        public IList<PermisoUsuario> PermisosUsuario { get; set; }
        public Rol Rol { get; set; }

        public IList<Rol> Roles { get; set; }

        public IList<Cargo> Cargos { get; set; }

        public int IdCargo { get; set; }

        public IList<Empleado> Empleados { get; set; }

        public string NombreRol { set; get; }

        public string NombreEmpleado { set; get; }

        public IList<UsuarioSucursal> Sucursales { set; get; }

        public string NombreEstado
        {
            get { return (Estado == (int)TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }

        public List<Comun> Estados { get; set; }

        public IList<PermisoUsuario> Permisos { get; set; }

        public IList<UsuarioSucursal> UsuarioSucursal { set; get; }
    }
}