
namespace JOERP.Business.Entity
{
    using System;

    public class PersonaEntidadFinanciera
    {
        public int IdEntidadFinanciera { get; set; }
        public int IdPersona { get; set; }
        public int IdBanco { get; set; }
        public int IdMoneda { get; set; }
        public string NumeroCuenta { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public EntidadFinanciera EntidadFinanciera { get; set; }
        public Moneda Moneda { get; set; }
        public Persona Persona { get; set; }
    }
}