
namespace JOERP.Business.Entity
{
    public class SaldoProducto
    {
        public int IdEmpresa { get; set; }
        public int IdSucursal { get; set; }
        public int IdProducto { get; set; }
        public int IdPresentacion { get; set; }
        public int IdAlmacen { get; set; }
        public string LoteSerie { get; set; }
        public string Periodo { get; set; }
        public decimal Stock { get; set; }
        public decimal Costo { get; set; }
    }
}