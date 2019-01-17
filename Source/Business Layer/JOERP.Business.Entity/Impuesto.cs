
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(ImpuestoValidation))]
    public class Impuesto
    {
        public int IdImpuesto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Abreviatura { get; set; }
        public int Signo { get; set; }
        public int EsEditable { get; set; }
        public decimal Monto { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }

        public string NombreEstado
        {
            get { return (Estado == (int)TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }

        public List<Comun> Estados { get; set; }

        public bool Editable {
            get
            {
                return (EsEditable == 1);
            }
        }

        public bool Demo { get; set; }

        public List<Comun> Signos { get; set; }

        public IList<OperacionImpuesto> OperacionImpuestos { get; set; }
    }
}