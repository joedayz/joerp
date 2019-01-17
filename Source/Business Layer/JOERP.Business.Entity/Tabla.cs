
namespace JOERP.Business.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(TablaValidation))]
    public class Tabla
    {
        public Tabla()
        {
            ItemTablas = new List<ItemTabla>();
        }

        public int IdTabla { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Estado { get; set; }
        public string NombreIngles { get; set; }
        public string DescripcionIngles { get; set; }
        public IList<ItemTabla> ItemTablas { get; set; }

        public string NombreEstado
        {
            get { return (Estado == (int)TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }

        public List<Comun> Estados { get; set; }
    }
}