
namespace JOERP.Business.Entity
{
    using System;

    public class EmpleadoListaPrecio
    {
        public int IdEmpleadoListaPrecio { get; set; }
        public int IdEmpleado { get; set; }
        public int IdListaPrecio { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Empleado Empleado { get; set; }
        public ListaPrecio ListaPrecio { get; set; }
    }
}