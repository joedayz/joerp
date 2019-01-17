
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class AlmacenValidation
    {
        [DataType(DataType.Text)]
        [DisplayName("Nombre :")]
        [Required(ErrorMessage = "Nombre requerido")]
        public string Nombre { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Descripción :")]
        public string Descripcion { get; set; }

        [DataType(DataType.Text)]
        [StringLength(4, ErrorMessage = "Abreviatura debe tener como máximo 4 caracteres")]
        [Required(ErrorMessage = "Abreviatura requerida")]
        [DisplayName("Abreviatura :")]
        public string Abreviatura { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Sucursal :")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione la Sucursal.")]
        public string IdSucursal { get; set; }

        [DisplayName("Estado :")]
        public int Estado { get; set; }
    }
}
