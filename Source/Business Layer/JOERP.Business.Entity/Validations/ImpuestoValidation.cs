
namespace JOERP.Business.Entity.Validations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class ImpuestoValidation
    {
        [DataType(DataType.Text)]
        [DisplayName("Nombre :")]
        [Required(ErrorMessage = "Nombre requerido")]
        public string Nombre { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Descripcion :")]
        public string Descripcion { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Abreviatura :")]
        [Required(ErrorMessage = "Abreviatura requerido")]
        [StringLength(5, ErrorMessage = "Abreviatura debe tener como máximo 5 caracteres")]
        public string Abreviatura { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Signo :")]
        public int Demo { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Es editable :")]
        public int Editable { get; set; }

        [DataType(DataType.Currency)]
        [DisplayName("Monto :")]
        public double Monto { get; set; }

        [DisplayName("Estado :")]
        public int Estado { get; set; }
    }
}
