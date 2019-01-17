
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Validations;

    [MetadataType(typeof(PresentacionValidation))]
    public class Presentacion
    {
        public int IdPresentacion { get; set; }
        public int IdProducto { get; set; }
        public int IdUnidadMedida { get; set; }
        public string Nombre { get; set; }
        public decimal Peso { get; set; }
        public decimal Equivalencia { get; set; }
        public bool EsBase { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Producto Producto { get; set; }
        public UnidadMedida UnidadMedida { get; set; }
        public IList<UnidadMedida> TiposUnidad { set; get; } 
    }
}