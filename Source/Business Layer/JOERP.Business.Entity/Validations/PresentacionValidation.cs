
namespace JOERP.Business.Entity.Validations
{
    using System.ComponentModel;

    public class PresentacionValidation
    {
        [DisplayName("Unidad Medida :")]
        public int IdUnidadMedida { get; set; }

        [DisplayName("Peso :")]
        public double Peso { get; set; }

        [DisplayName("Equivalencia :")]
        public double Equivalencia { set; get; }

        [DisplayName("Es Base :")]
        public bool EsBase { set; get; }
    }
}
