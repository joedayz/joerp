
namespace JOERP.Business.Entity.DTO
{
    using System;
    using Helpers.Enums;

    public class TransaccionMovimientoDTO
    {
        public int IdTransaccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Documento { get; set; }
        public string DocumentoRelacionado { get; set; }
        public string NombreEmpresa { get; set; }
        public string NombreSucursal { get; set; }
        public string NombreSucursal1 { get; set; }
        public string AlmacenOrigen { get; set; }
        public string AlmacenDestino { get; set; }
        public string NombreOperacion { get; set; }
        public string NombreEmpleado { get; set; }
        public string NombreEmpresaTransporte { get; set; }
        public string NombreMoneda { get; set; }
        public decimal MontoTipoCambio { get; set; }
        public string Proveedor { get; set; }
        public string PersonaConductor { get; set; }
        public string Direccion { get; set; }
        public int Estado { get; set; }

        public string EstadoDoc
        {
            get { return Enum.GetName(typeof (TipoEstadoTransaccion), Estado); }
        }
    }
}
