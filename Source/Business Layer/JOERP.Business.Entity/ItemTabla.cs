
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(ItemTablaValidation))]
    public class ItemTabla
    {
        public int IdItemTabla { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Estado { get; set; }
        public string Valor { get; set; }
        public int IdTabla { get; set; }
        public string NombreIngles { get; set; }
        public string DescripcionIngles { get; set; }
        public Tabla Tabla { get; set; }

        public string NombreEstado
        {
            get { return Enum.Parse(typeof(TipoEstado), Estado.ToString()).ToString(); }
        }

        public List<Comun> Estados { get; set; }

        public string NombreTabla
        {
            get { return Tabla.Nombre; }
        }
    }
}