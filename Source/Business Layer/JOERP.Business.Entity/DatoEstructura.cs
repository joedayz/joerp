
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Validations;

    [MetadataType(typeof(DatoEstructuraValidation))] 
    public class DatoEstructura
    {
        public int IdDatoEstructura { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int? IdParent { get; set; }
        public int IdEstructuraProducto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public EstructuraProducto EstructuraProducto { get; set; }

        public string Jerarquia { get; set; }

        public string NombreEstructura
        {
            get { return EstructuraProducto == null ? string.Empty : EstructuraProducto.Nombre; }
        }

        public int IdLinea { get; set; }

        public IList<DatoEstructura> DatosEstructuras { set; get; }

        public IList<EstructuraProducto> EstructurasProducto { set; get; }
    }
}