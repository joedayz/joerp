
namespace JOERP.Business.Entity.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Validations;

    [MetadataType(typeof(TransferenciaValidation))] 
    public class DistribucionMercaderia : Transaccion
    {
        public int IdTipoDocumentoRef { get; set; }

        public string SerieDocumentoRef { get; set; }

        public string NumeroDocumentoRef { get; set; }

        public DateTime FechaDocumentoRef { get; set; }
    }
}
