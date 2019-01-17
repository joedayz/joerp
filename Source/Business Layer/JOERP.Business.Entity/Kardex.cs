
namespace JOERP.Business.Entity
{
    using System;

    public class Kardex
    {
        public int IdTransaccion { get; set; }

        public int IdEmpresa { get; set; }

        public int IdSucursal { get; set; }

        public int IdOperacion { get; set; }

        public int IdTipoDocumento { get; set; }

        public string TipoDocumento { get; set; }

        public string SerieDocumento { get; set; }

        public string NumeroDocumento { get; set; }

        public DateTime FechaEmision { get; set; }

        public DateTime FechaProceso { get; set; }

        public string TipoOperacion { get; set; }

        public int SignoStock { get; set; }

        public int IdProducto { get; set; }

        public string CodigoProducto { get; set; }

        public int IdPresentacion { get; set; }

        public int IdAlmacen { get; set; }

        public decimal CantidadIngreso { get; set; }

        public decimal CostoUnitarioIngreso { get; set; }

        public decimal CostoTotalIngreso { get; set; }

        public decimal CantidadSalida { get; set; }

        public decimal CostoUnitarioSalida { get; set; }

        public decimal CostoTotalSalida { get; set; }

        public decimal CantidadSaldo { get; set; }

        public decimal CostoUnitarioSaldo { get; set; }

        public decimal CostoTotalSaldo { get; set; }

        public int? IdTipoDocumentoAlterno { get; set; }

        public string TipoDocumentoAlterno { get; set; }

        public string SerieDocumentoAlterno { get; set; }

        public string NumeroDocumentoAlterno { get; set; }

        public int IdEmpleado { get; set; }

        public int Estado { get; set; }
    }
}
