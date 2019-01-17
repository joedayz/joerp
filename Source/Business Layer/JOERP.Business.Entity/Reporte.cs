
namespace JOERP.Business.Entity
{
    using System;

    public class Reporte
    {
        public int IdReporte { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdModulo { get; set; }
        public int? IdCategoria { get; set; }
        public string FileName { get; set; }
        public int Parametros { get; set; }
        public string TypedDataset { get; set; }
        public string ReportDataSet { get; set; }
        public string DataTable { get; set; }
        public string TableAdapter { get; set; }
        public string MethodFill { get; set; }
        public int Estado { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}