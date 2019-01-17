
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using Helpers.Enums;

    public class PermisoRol
    {
        public int IdPermisoRol { get; set; }
        public int IdRol { get; set; }
        public int IdTipoPermiso { get; set; }
        public int IdFormulario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Formulario Formulario { get; set; }
        public Rol Rol { get; set; }

        public string TipoPermiso
        {
            get { return Enum.GetName(typeof(TipoPermiso), IdTipoPermiso); }
        }

        public IList<Empleado> Empleados { get; set; }
        public IList<Rol> Roles { get; set; }
        public bool Seleccionado { get; set; }
    }
}