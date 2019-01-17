
namespace JOERP.Business.Entity.Validations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class MonedaValidation
    {
        [DataType(DataType.Text)]
        [DisplayName("Nombre :")]
        [Required(ErrorMessage = "Nombre requerido")]
        public string Nombre { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Abreviatura :")]
        [Required(ErrorMessage = "Abreviatura requerido")]
        public string Abreviatura { get; set; }

        [DisplayName("Simbolo :")]
        [Required(ErrorMessage = "Simbolo requerido")]
        public string Simbolo { get; set; }

        [DisplayName("Estado :")]
        public int Estado { get; set; }
    }
}
