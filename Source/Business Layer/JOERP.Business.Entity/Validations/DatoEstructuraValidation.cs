
namespace JOERP.Business.Entity.Validations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class DatoEstructuraValidation
    {
        [DataType(DataType.Text)]
        [DisplayName("Nombre :")]
        [Required(ErrorMessage = "Nombre requerido")]
        public string Nombre { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Descripción :")]
        public string Descripcion { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Linea :")]
        public int IdLinea { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Sub Linea :")]
        public int IdParent { get; set; }

    }
}
