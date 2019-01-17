
namespace JOERP.Business.Entity.Validations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class ItemTablaValidation
    {
        [DataType(DataType.Text)]
        [DisplayName("Código :")]
        public string Codigo { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Nombre :")]
        [Required(ErrorMessage = "Nombre requerido")]
        public string Nombre { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Descripción :")]
        public string Descripcion { get; set; }

        [DisplayName("Estado :")]
        public int Estado { get; set; }
    }
}
