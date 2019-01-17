
namespace JOERP.Business.Entity
{
    public class DatoEstructuraProducto
    {
        public int IdProducto { get; set; }

        public int IdDatoEstructura { get; set; }

        public DatoEstructura DatoEstructura { get; set; }
    }
}
