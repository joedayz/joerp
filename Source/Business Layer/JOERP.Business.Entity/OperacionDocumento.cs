
namespace JOERP.Business.Entity
{
    using System.Collections.Generic;
    using Helpers;
    using Helpers.Enums;

    public class OperacionDocumento
    {
        public int IdOperacion { get; set; }
        public int TipoDocumento { get; set; }
        public int Orden { get; set; }
        public int Posicion { get; set; }
        public int Estado { get; set; }
       
        public IList<Comun> TipoDocumentos { get; set; }
        public string NombreEstado
        {
            get { return (Estado == (int)TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }
    }
}