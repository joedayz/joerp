
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Validations;

    [MetadataType(typeof(MovimientoProductoValidation))]
    public class MovimientoProducto
    {
        public int IdMovimientoProducto { get; set; }
        public int IdTransaccion { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int IdProducto { get; set; }
        public int IdPresentacion { get; set; }
        public int? IdAlmacen { get; set; }
        public int Secuencia { get; set; }
        public int TipoProducto { get; set; }
        public int TipoClasificacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadDocumento { get; set; }        
        public decimal PrecioBase { get; set; }
        public int? IdListaPrecio { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal PrecioNeto { get; set; }
        public decimal PorcentajeImpuesto { get; set; }
        public decimal MontoImpuesto { get; set; }
        public decimal SubTotal { get; set; }
        public int SignoStock { get; set; }
        public decimal Costo { get; set; }
        public decimal Saldo { get; set; }
        public decimal? Peso { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public string NombreProducto { get; set; }
        public string CodigoProducto { get; set; }
        public string CodigoAlternoProducto { set; get; }
        public string NombrePresentacion { get; set; }
        public string NombreAlmacen { get; set; }
        public string UsuarioCreacionNombre { get; set; }
        public string UsuarioModificacionNombre { get; set; }
        public decimal MontoAgregado { get; set; }
        public IList<MovimientoProductoStock> MovimientoProductoStock { get; set; }
        public decimal PorcentajeDescuento2 { get; set; }
        public decimal MontoDescuento2 { get; set; }

        public string RazonSocial { get; set; }
        public string TipoNumeroDocumento { get; set; }
        public string FechaDocumento { get; set; }
        public string Ruc { get; set; }
        public string Lote { get; set; }
        public string Moneda { get; set; }
        public decimal TipoCambio { get; set; }
        public string Glosa { get; set; }

        public decimal Igv { set; get; }
        public decimal Descuento { set; get; }
        public decimal Total { set; get; }
        public decimal TotalParcial { set; get; }
    }
}