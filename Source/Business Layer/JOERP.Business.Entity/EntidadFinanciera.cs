
namespace JOERP.Business.Entity
{
    using System;
    using Helpers.Enums;

    public class EntidadFinanciera
    {
        public int IdEntidadFinanciera { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int TipoEntidad { get; set; }
        public int IdEmpresa { get; set; }
        public int? IdSucursal { get; set; }
        public int? IdMoneda { get; set; }
        public string NumeroCuenta { get; set; }
        public bool EsPorDefecto { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Empresa Empresa { get; set; }
        public Moneda Moneda { get; set; }
        public Sucursal Sucursal { get; set; }

        public string NombreEstado
        {
            get { return Enum.GetName(typeof(TipoEstado), Estado); }
        }

        public string NombreTipoEntidad
        {
            get { return Enum.GetName(typeof(TipoEntidadFinanciera), TipoEntidad); }
        }

        public string NombreMoneda
        {
            get { return (Moneda != null) ? Moneda.Nombre : string.Empty; }
        }
    }
}