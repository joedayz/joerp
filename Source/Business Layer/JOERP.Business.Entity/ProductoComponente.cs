
namespace JOERP.Business.Entity
{
    using System;

    public class ProductoComponente
    {
        public int IdProductoComponente { get; set; }
        public int IdProducto { get; set; }
        public int IdPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Presentacion Presentacion { get; set; }
        public Producto Producto { get; set; }
    }
}