
namespace JOERP.Business.Entity.Validations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class ProductoValidation
    {
        [DisplayName("Código :")]
        public string Codigo { get; set; }

        [StringLength(100, ErrorMessage = "El Nombre de Producto no debe ser más de 100 caracteres.", MinimumLength = 2)]
        [Required(ErrorMessage = "Nombre requerido")]
        [DisplayName("Nombre :")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Descripción requerida")]
        [DisplayName("Descripción :")]
        public string Descripcion { get; set; }

        [DisplayName("Código de Barras :")]
        public string CodigoBarra { get; set; }

        [DisplayName("Estado :")]
        public int Estado { get; set; }

        [Required(ErrorMessage = "Stock máximo requerido")]
        [DisplayName("Stock Máximo :")]
        public decimal StockMaximo { get; set; }

        [Required(ErrorMessage = "Stock mínimo requerido")]
        [DisplayName("Stock Mínimo :")]
        public decimal StockMinimo { get; set; }

        [DisplayName("Código Alterno :")]
        public string CodigoAlterno { get; set; }

        [DisplayName("Exonerado I.G.V. :")]
        public bool EsExonerado { get; set; }

        [DisplayName("Tipo Producto :")]
        public int TipoProducto { get; set; }

        [DisplayName("Descripción Larga :")]
        public string DescripcionLarga { get; set; }

        [DisplayName("Tipo Clasificación :")]
        public int TipoClasificacion { get; set; }

        [DisplayName("Categoría :")]
        public int IdDatoEstructura { get; set; }

        [DisplayName("Sub Línea :")]
        public int IdSubLinea { get; set; }

        [DisplayName("Línea :")]
        public int IdLinea { get; set; }
    }
}
