
namespace JOERP.Business.Entity
{
    using System;

    public class ProductoPrecio
    {
        public int IdProductoPrecio { get; set; }
        public int IdSucursal { get; set; }
        public int IdPresentacion { get; set; }
        public int IdListaPrecio { get; set; }
        public decimal Costo { get; set; }
        public decimal PorcentajeGanancia { get; set; }
        public decimal Ganancia { get; set; }
        public decimal Valor { get; set; }
        public decimal PrecioVenta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public ListaPrecio ListaPrecio { get; set; }
        public Presentacion Presentacion { get; set; }
        public Sucursal Sucursal { get; set; }
    }
}