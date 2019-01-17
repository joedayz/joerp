
namespace JOERP.Business.Entity.DTO
{
    using System.ComponentModel.DataAnnotations;
    using Validations;

    [MetadataType(typeof(TransferenciaValidation))]
    public class Transferencia : Transaccion
    {
        
    }
}
