
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using Helpers;

    public class OperacionImpuesto
    {
        public int IdOperacion { get; set; }

        public int IdImpuesto { get; set; }

        public int Orden { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public int UsuarioCreacion { get; set; }

        public int UsuarioModificacion { get; set; }

        public Impuesto Impuesto { get; set; }

        public Operacion Operacion { get; set; }

        public IList<Comun> TipoImpuestos { set; get; }

        public IList<OperacionImpuesto> ImpuestosOperacion { set; get; }

        public bool Seleccionado { set; get; }
    }
}