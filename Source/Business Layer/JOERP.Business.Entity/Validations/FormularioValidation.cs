
namespace JOERP.Business.Entity.Validations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class FormularioValidation
    {
        [DataType(DataType.Text)]
        [DisplayName("Nombre :")]
        [Required(ErrorMessage = "Nombre requerido")]
        public string Nombre { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Descripción :")]
        public string Descripcion { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Controlador :")]
        public string Assembly { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Ruta :")]
        public string Direccion { get; set; }

        [DisplayName("Formulario Padre :")]
        public int IdParent { get; set; }

        [Required(ErrorMessage = "Nivel requerido")]
        [DisplayName("Nivel :")]
        [Range(0, 1000, ErrorMessage = "El nivel debe ser entero positivo")]
        public string Nivel { get; set; }

        [DisplayName("Módulo :")]
        public int IdModulo { get; set; }

        [DisplayName("Estado :")]
        public int Estado { get; set; }

        [DisplayName("Operación :")]
        public int IdOperacion { get; set; }

        [DisplayName("Orden :")]
        public int Orden { get; set; }
    }
}
