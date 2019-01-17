
namespace JOERP.Business.Entity.Validations
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class MovimientoProductoValidation
    {
        [Required(ErrorMessage = "Código requerido.")]
        [DataType(DataType.Text)]
        [DisplayName("Producto :")]
        public string CodigoProducto { get; set; }

        [Required(ErrorMessage = "Código requerido.")]
        [DataType(DataType.Text)]
        [DisplayName("Cod. Alterno :")]
        public string CodigoAlternoProducto { get; set; }

        [Required(ErrorMessage = "Producto requerido.")]
        [DataType(DataType.Text)]
        [DisplayName("Producto :")]
        public string NombreProducto { get; set; }

        [Required(ErrorMessage = "Presentación requerido.")]
        [DataType(DataType.Text)]
        [Range(1, Int16.MaxValue, ErrorMessage = "Seleccione Presentación.")]
        [DisplayName("Presentación :")]
        public int IdPresentacion { get; set; }

        [Required(ErrorMessage = "Cantidad requerida.")]
        [DataType(DataType.Text)]
        [DisplayName("Cantidad :")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "Precio Base requerido.")]
        [DataType(DataType.Text)]
        [DisplayName("Precio Unit. :")]
        public decimal PrecioBase { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("% Descuento :")]
        public decimal PorcentajeDescuento { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Monto Descuento :")]
        public decimal MontoDescuento { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Precio Final :")]
        public decimal PrecioNeto { get; set; }

        [Required(ErrorMessage = "Cantidad factura requerida.")]
        [DataType(DataType.Text)]
        [DisplayName("Cantidad Factura:")]
        public decimal CantidadDocumento { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("% Descuento2 :")]
        public decimal PorcentajeDescuento2 { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Monto Descuento2 :")]
        public decimal MontoDescuento2 { get; set; }
    }
}
