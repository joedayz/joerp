
namespace JOERP.Business.Entity
{
    public class Ubigeo
    {
        public int IdUbigeo { get; set; }
        public int IdDepartamento { get; set; }
        public int IdProvincia { get; set; }
        public int IdDistrito { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public string Direccion
        {
            get { return string.Format("{0} - {1} - {2}", Distrito, Provincia, Departamento); }
        }
    }
}