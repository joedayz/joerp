
namespace JOERP.Business.Entity
{
    using System;

    public class Inventario
    {
        public string Codigo { get; set; }
        public string CodigoBarra { get; set; }
        public string Producto { get; set; }
        public string Presentacion { get; set; }
        public string Lote { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Serie { get; set; }
        public decimal StockSistema { get; set; }
        public decimal CostoSistema { get; set; }
        public decimal StockReal { get; set; }
        public decimal CostoReal { get; set; }
        public int IdProductoStock { get; set; }
        public int IdProducto { get; set; }
        public int IdPresentacion { get; set; }
        public decimal Equivalencia { get; set; }
    }
}
