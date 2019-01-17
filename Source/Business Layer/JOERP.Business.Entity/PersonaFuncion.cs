
namespace JOERP.Business.Entity
{
    using System;
    using Helpers.Enums;

    public class PersonaFuncion
    {
        public int IdFuncion { get; set; }
        public int IdPersona { get; set; }
        public Persona Persona { get; set; }
        public bool Seleccionado { get; set; }

        public string NombreFuncion
        {
            get { return Enum.GetName(typeof(TipoFuncion), IdFuncion); }
        }
    }
}