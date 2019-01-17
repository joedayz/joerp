
namespace JOERP.Business.Entity
{
    public class ProductoProveedor
    {
        public int IdPersona { get; set; }
        public int IdPresentacion { get; set; }
        public decimal Precio { get; set; }
        public Persona Persona { get; set; }
        public Presentacion Presentacion { get; set; }
        public string Producto { set; get; }
        public string Proveedor { set; get; }
        public string Codigo { get; set; }
        public string CodigoAlterno { get; set; }
        public string Linea { get; set; }
        public string SubLinea { get; set; }
        public string Categoria { get; set; }
    }
}