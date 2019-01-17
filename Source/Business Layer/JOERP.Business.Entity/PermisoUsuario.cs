
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using Helpers.Enums;

    public class PermisoUsuario
    {
        public int IdPermisoUsuario { get; set; }
        public int IdTipoPermiso { get; set; }
        public int IdFormulario { get; set; }
        public int IdEmpleado { get; set; }
        public Formulario Formulario { get; set; }
        public Usuario Usuario { get; set; }

        public string TipoPermiso
        {
            get { return Enum.GetName(typeof(TipoPermiso), IdTipoPermiso); }
        }

        public bool Seleccionado { get; set; }

        public IList<Rol> Roles { get; set; }
    }
}