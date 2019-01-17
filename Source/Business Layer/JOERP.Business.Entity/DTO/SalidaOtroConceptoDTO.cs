
namespace JOERP.Business.Entity.DTO
{
    using System;

    public class SalidaOtroConceptoDTO : Transaccion
    {
        public DateTime Fecha { get; set; }

        public string Guia { get; set; }

        public string Transporte { get; set; }

        public int IdEstado { get; set; }

        public int IdConcepto { get; set; }

        public string NombreAlmacen { get; set; }
    }
}
