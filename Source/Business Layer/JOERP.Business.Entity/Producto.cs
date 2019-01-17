
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(ProductoValidation))]
    public class Producto
    {
        public Producto()
        {
            Presentacion = new List<Presentacion>();
            ProductoComponente = new List<ProductoComponente>();
        }

        public int IdProducto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string CodigoBarra { get; set; }
        public int Estado { get; set; }
        public int IdEmpresa { get; set; }
        public decimal? StockMaximo { get; set; }
        public decimal? StockMinimo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public bool EsAfecto { get; set; }
        public string CodigoAlterno { get; set; }
        public bool EsExonerado { get; set; }
        public int TipoProducto { get; set; }
        public string DescripcionLarga { get; set; }
        public int TipoClasificacion { get; set; }
        public byte[] Imagen { get; set; }
        public Empresa Empresa { get; set; }
        public List<Presentacion> Presentacion { get; set; }
        public List<ProductoComponente> ProductoComponente { get; set; }

        public IList<Comun> TiposProducto { get; set; }

        public IList<Comun> TiposClasificacion { get; set; }

        public string NombreTipoProducto
        {
            get { return Enum.Parse(typeof(TipoProducto), TipoProducto.ToString()).ToString(); }
        }

        public List<Comun> Estados { set; get; }

        public string NombreEstado
        {
            get { return (Estado == (int)TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }

        public int IdLinea { set; get; }
        public int IdSubLinea { set; get; }
        public int IdDatoEstructura { set; get; }

        public IList<DatoEstructura> Lineas { set; get; }

        public IList<DatoEstructura> SubLineas { set; get; }

        public IList<DatoEstructura> Categorias { set; get; }

        public IList<DatoEstructuraProducto> DatoEstructuraProducto { set; get; }
    }
}