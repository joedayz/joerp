
namespace JOERP.Business.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Validations;

    [MetadataType(typeof(PersonaDireccionValidation))]
    public class PersonaDireccion
    {
        public int IdDireccion { get; set; }
        public int IdPersona { get; set; }
        public int IdUbigeo { get; set; }
        public string Referencia { get; set; }
        public int? TipoVia { get; set; }
        public string NombreVia { get; set; }
        public string Numero { get; set; }
        public string Interior { get; set; }
        public int? TipoDireccion { get; set; }
        public int? TipoZona { get; set; }
        public string NombreZona { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Persona Persona { get; set; }
        public Ubigeo Ubigeo { get; set; }
    }
}