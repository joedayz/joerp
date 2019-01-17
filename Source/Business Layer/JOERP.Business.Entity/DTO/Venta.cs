
namespace JOERP.Business.Entity.DTO
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Validations;

    [MetadataType(typeof(VentaValidation))]
    public class Venta : Transaccion
    {
        public IList<Comun> TiposVenta { set; get; }
    }
}
