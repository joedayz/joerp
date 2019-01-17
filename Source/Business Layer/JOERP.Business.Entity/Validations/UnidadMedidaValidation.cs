
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class UnidadMedidaValidation
    {
        [DataType(DataType.Text)]
        [DisplayName("Nombre :")]
        [Required(ErrorMessage = "Nombre requerido")]
        public string Nombre { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Abreviatura:")]
        [Required(ErrorMessage = "Abreviatura requerida")]
        public string Abreviatura { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Empresa :")]
        [Required(ErrorMessage = "Empresa requerida")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione la Empresa.")]
        public string IdEmpresa { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Descripción :")]
        public string Descripcion { get; set; }
    }
}
