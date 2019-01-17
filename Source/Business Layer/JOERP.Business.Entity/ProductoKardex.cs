
namespace JOERP.Business.Entity
{
    public class ProductoKardex
    {
        public int IdProductoKardex { get; set; }

        public string Periodo { get; set; }

        public int IdProducto { get; set; }

        public int IdPresentacion { get; set; }

        public int IdAlmacen { get; set; }

        public string LoteSerie { get; set; }

        public string FechaVencimiento { get; set; }

        public decimal Costo { get; set; }

        public decimal Cantidad { get; set; }

        public decimal Saldo { get; set; }

        public int Signo { get; set; }

        public int IdIngreso { get; set; }

        public int IdTransaccion { get; set; }
    }
}
