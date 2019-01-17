
namespace JOERP.Business.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Validations;

    [MetadataType(typeof(CargoValidation))]
    public class Cargo
    {
        public int IdCargo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdEmpresa { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public Empresa Empresa { get; set; }
        public string NombreEmpresa { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}