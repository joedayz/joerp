
namespace JOERP.Business.Entity.DTO
{
    using System;

    public class IngresoAjusteInventarioDTO
    {
        public int IdTransaccion { get; set; }

        public DateTime Fecha { get; set; }

        public string Documento { get; set; }

        public int IdEstado { get; set; }

        public string Estado { get; set; }

        public int IdConcepto { get; set; }

        public string Concepto { get; set; }
    }
}
