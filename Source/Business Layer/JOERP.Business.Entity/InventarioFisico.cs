
namespace JOERP.Business.Entity
{
    public class InventarioFisico
    {
        public string Codigo { set; get; }

        public string Producto { set; get; }

        public string Presentacion { set; get; }

        public string TipoClasificacion { set; get; }

        public string Lote { set; get; }

        public string FechaVencimiento { set; get; }

        public string Serie { set; get; }

        public decimal StockFisico { set; get; }

        public string Almacen { set; get; }

        public string Sucursal { set; get; }

        public int SubLinea { set; get; }

        public int Linea { set; get; }

        public int Categoria { set; get; }

        public int Proveedor { set; get; } //Id

        public int IdPresentacion { set; get; }

        public decimal Equivalencia { set; get; }

        public int IdAlmacen { set; get; }

        public int IdSucursal { set; get; }

        public int IdProducto { set; get; }

        public decimal Costo { set; get; }

        public string AnioMes { set; get; }

    }
}
