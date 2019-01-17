
namespace JOERP.Business.Entity
{
    using System;

    public class PersonaContacto
    {
        public int IdPersonaContacto { get; set; }
        public int IdPersona { get; set; }
        public int IdContacto { get; set; }
        public string Cargo { get; set; }
        public int TipoContacto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Persona Persona { get; set; }
        public Persona Persona1 { get; set; }
    }
}