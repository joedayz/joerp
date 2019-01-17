
namespace JOERP.Business.Entity
{
    using System;

    public class UnidadTransporte
    {
        public int IdUnidadTransporte { get; set; }
        public int IdEmpresa { get; set; }
        public int IdTransporte { get; set; }
        public int IdConductor { get; set; }
        public string Descripcion { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public string Configuracion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int Estado { get; set; }
        public Empresa Empresa { get; set; }
        public Persona Persona { get; set; }
    }
}