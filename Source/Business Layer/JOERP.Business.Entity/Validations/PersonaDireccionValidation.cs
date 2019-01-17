
namespace JOERP.Business.Entity.Validations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class PersonaDireccionValidation
    {
        [StringLength(100, ErrorMessage = "No debe exeder los 100 caracteres.", MinimumLength = 2)]
        [DisplayName("Referencia :")]
        public string Referencia { set; get; }

        [DisplayName("Tipo Vía :")]
        public int TipoVia { set; get; }

        [StringLength(30, ErrorMessage = "No debe exeder los 30 caracteres.", MinimumLength = 1)]
        [DisplayName("Nombre Vía :")]
        public string NombreVia { set; get; }

        [StringLength(20, ErrorMessage = "No debe exeder los 20 caracteres.", MinimumLength = 1)]
        [DisplayName("Numero :")]
        public int Numero { set; get; }

        [StringLength(20, ErrorMessage = "No debe exeder los 20 caracteres.", MinimumLength = 1)]
        [DisplayName("Interior :")]
        public int Interior { set; get; }

        [DisplayName("Tipo Zona :")]
        public int TipoZona { get; set; }

        [StringLength(20, ErrorMessage = "No debe exeder los 20 caracteres.", MinimumLength = 1)]
        [DisplayName("Nombre Zona :")]
        public string NombreZona { set; get; }
    }
}
