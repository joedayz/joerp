
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Validations;

    [MetadataType(typeof(SerieDocumentoValidation))]
    public class SerieDocumento
    {
        public int IdSerieDocumento { get; set; }
        public string Serie { get; set; }
        public string NumeroActual { get; set; }
        public string NumeroInicio { get; set; }
        public string NumeroFinal { get; set; }
        public int TipoDocumento { get; set; }
        public int IdSucursal { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Sucursal Sucursal { get; set; }
        public IList<Comun> TipoDocumentos { get; set; }
    }
}