
namespace JOERP.Business.Entity
{
    using System;

    public class ReporteDetalle
    {
        public int IdReporte { get; set; }
        public int IdParametro { get; set; }
        public string Nombre { get; set; }
        public string Parametros { get; set; }
        public string Label { get; set; }
        public string TipoControl { get; set; }
        public string TipoDato { get; set; }
        public int TipoParametro { get; set; }
        public bool Visible { get; set; }
        public string ValorPorDefecto { get; set; }
        public int? IdParent { get; set; }
        public bool EsRequerido { get; set; }
        public int Cantidad { get; set; }
        public int Estado { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}