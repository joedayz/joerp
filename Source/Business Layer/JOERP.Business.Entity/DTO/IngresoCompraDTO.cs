
namespace JOERP.Business.Entity.DTO
{
    using System;

    public class IngresoCompraDTO : Transaccion
    {
        public int IdIngreso;

        public int IdOperacionCompra;

        public int TipoComprobante;

        public string SerieComprobante;

        public string NumeroComprobate;

        public DateTime FechaComprobante;
        
    }
}
