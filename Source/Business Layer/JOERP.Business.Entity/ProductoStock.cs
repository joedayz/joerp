
namespace JOERP.Business.Entity
{
    using System;

    public class ProductoStock
    {
        public int IdProductoStock { get; set; }
        public int IdPresentacion { get; set; }
        public int IdAlmacen { get; set; }
        public string Lote { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Serie { get; set; }
        public decimal StockLogico { get; set; }
        public decimal StockFisico { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Almacen Almacen { get; set; }
        public Presentacion Presentacion { get; set; }

        public string NombreAlmacen
        {
            get { return Almacen == null ? string.Empty : Almacen.Nombre; }
        }
    }
}