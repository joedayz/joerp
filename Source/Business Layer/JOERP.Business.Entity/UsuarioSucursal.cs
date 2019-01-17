
namespace JOERP.Business.Entity
{
    public class UsuarioSucursal
    {
        public int IdUsuario { get; set; }

        public int IdSucursal { get; set; }

        public bool Seleccionado { get; set; }

        public string NombreSucursal { get; set; }

        public int IdEmpleado { set; get; }
    }
}
