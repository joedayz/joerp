
namespace JOERP.Business.Entity.DTO
{
    using System;

    public class CargasInventarioDTO : Transaccion
    {
        public DateTime Fecha { get; set; }

        public string Almacen { get; set; }

        public int IdEstado { get; set; }

    }
}
