
namespace JOERP.Business.Entity.DTO
{
    using System;

    public class DespachoDTO
    {
        public int IdTransaccion { get; set; }

        public string Documento { get; set; }

        public DateTime Fecha { get; set; }

        public string DocumentoRelacionado { get; set; }

        public string Estado { get; set; }

        public int IdEstado { get; set; }
    }
}
