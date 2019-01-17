
namespace JOERP.Business.Entity.DTO
{
    using System;

    public class HistorialPrecioDTO
    {
        public int IdTransaccion { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string Documento { get; set; }

        public string Proveedor { get; set; }

        public decimal Cantidad { get; set; }

        public decimal Descuento { get; set; }

        public decimal Precio { get; set; }
    }
}
