
namespace JOERP.Business.Entity.DTO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Validations;
    using Helpers;

    [MetadataType(typeof(OrdenCompraValidation))]
    public class OrdenCompra : Transaccion
    {
        public string FechaDocumentoFormat { get; set; }

        [Required(ErrorMessage = "Tipo de Compra requerido")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Por favor seleccione el tipo de Compra.")]
        [DisplayName("Tipo Compra :")]
        public int EsLocal { set; get; }

        public IList<Comun> TiposCompra { set; get; }
    }
}
