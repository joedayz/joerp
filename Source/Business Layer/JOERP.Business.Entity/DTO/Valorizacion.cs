
namespace JOERP.Business.Entity.DTO
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Validations;

    [MetadataType(typeof(ValorizacionValidation))]
    public class Valorizacion : Transaccion
    {
        public IList<TransaccionDocumento> DocumentosRelacionados { set; get; }
    }
}
