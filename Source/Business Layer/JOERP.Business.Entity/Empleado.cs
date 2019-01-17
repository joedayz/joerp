
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(EmpleadoValidation))]
    public class Empleado
    {
        public int IdEmpleado { get; set; }
        public int IdCargo { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public Cargo Cargo { get; set; }
        public Persona Persona { get; set; }
        public Usuario Usuario { get; set; }
        public int Estado { set; get; }
        public string NombreCargo { set; get; }

        public override string ToString()
        {
            return Persona.FullName;
        }

        public string NombreEstado
        {
            get { return (Estado == (int)TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }

        public IList<PersonaFuncion> Funciones { set; get; }
        public string NumeroTelefono { set; get; }
        public string NombrePersona { set; get; }
    }
}