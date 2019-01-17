
namespace JOERP.Business.Entity.Validations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RolValidation
    {
        [DataType(DataType.Text)]
        [DisplayName("Nombre :")]
        [Required(ErrorMessage = "Razon Social requerida")]
        public string Nombre { get; set; }

        [DisplayName("Estado  :")]
        public int Estado { get; set; }
    }
}
