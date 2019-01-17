
namespace JOERP.Business.Entity
{
    using System;

    public class MovimientoProductoStock
    {
        public int IdMovimientoProductoStock { get; set; }
        public int IdMovimientoProducto { get; set; }
        public int Secuencia { get; set; }
        public int IdAlmacen { get; set; }
        public string NombreAlmacen { get; set; }
        public string LoteSerie { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string FechaVencimientoFormato { get; set; }
        public decimal Cantidad { get; set; }
        public decimal StockActual { get; set; }
        public int TipoClasificacion { get; set; }
        public int IdPresentacion { get; set; }
        public int SignoStock { get; set; }
        public int UsuarioCreacion { get; set; }
        public int IdAlmacenOrigen { get; set; }
        public decimal Costo { get; set; }
        public decimal Saldo { get; set; }
    }
}