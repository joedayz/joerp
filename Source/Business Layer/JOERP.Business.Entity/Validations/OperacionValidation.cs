
namespace JOERP.Business.Entity.Validations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class OperacionValidation
    {
        [DisplayName("Código :")]
        public int Codigo { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "Los Nombres no debe ser más de 50 caracteres.", MinimumLength = 5)]
        [Required(ErrorMessage = "Nombre requerido")]
        [DisplayName("Nombre :")]
        public string Nombre { get; set; }

        [DisplayName("Descripción :")]
        public string Descripcion { get; set; }

        [DisplayName("Documento Interno :")]
        public int TipoDocumentoInterno { set; get; }

        [DisplayName("Signo Stock :")]
        public int SignoStock { set; get; }

        [DisplayName("Signo Caja :")]
        public int SignoCaja { set; get; }

        [DisplayName("Signo Contable :")]
        public int SignoContable { set; get; }

        [DisplayName("Signo Cartera :")]
        public int SignoCartera { set; get; }

        [DisplayName("Realiza Asiento :")]
        public bool RealizaAsiento { set; get; }

        [DisplayName("Estado :")]
        public int Estado { set; get; }
    }
}
