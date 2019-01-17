
namespace JOERP.Business.Entity.Validations
{
    using System.ComponentModel;

    public class EmpleadoValidation
    {
        [DisplayName("Cargo :")]
        public int IdCargo { set; get; }
    }
}
