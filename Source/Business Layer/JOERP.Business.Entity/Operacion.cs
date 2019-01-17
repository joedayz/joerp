
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(OperacionValidation))]
    public class Operacion
    {
        public Operacion()
        {
            OperacionDocumentos = new List<OperacionDocumento>();
            OperacionImpuestos = new List<OperacionImpuesto>();
        }

        public int IdOperacion { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int SignoStock { get; set; }
        public int SignoCartera { get; set; }
        public int SignoCaja { get; set; }
        public int SignoContable { get; set; }
        public bool RealizaAsiento { get; set; }
        public int? TipoDocumentoInterno { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public IList<OperacionDocumento> OperacionDocumentos { get; set; }
        public IList<OperacionImpuesto> OperacionImpuestos { get; set; }

        public string NombreEstado
        {
            get { return (Estado == (int)TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }

        public List<Comun> Estados { get; set; }

        public List<Comun> Signos { get; set; } 
    }
}