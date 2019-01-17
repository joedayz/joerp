
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using Helpers;
    using Helpers.Enums;

    public class Transaccion
    {
        public int IdTransaccion { get; set; }
        public int IdEmpresa { get; set; }
        public int IdSucursal { get; set; }
        public int IdOperacion { get; set; }
        public int IdEmpleado { get; set; }
        public int IdSerieDocumento { get; set; }
        public int IdTipoDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime FechaDocumento { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? Concepto { get; set; }
        public int? CondicionPago { get; set; }
        public int? FormaPago { get; set; }
        public int? IdMoneda { get; set; }
        public int? IdTipoCambio { get; set; }
        public decimal MontoTipoCambio { get; set; }
        public decimal MontoNeto { get; set; }
        public decimal Saldo { get; set; }
        public int Estado { get; set; }
        public int? IdTransaccionReferencia { get; set; }
        public int? IdPersona { get; set; }
        public int? IdDireccionFacturacion { get; set; }
        public int? IdDireccionEnvio { get; set; }
        public int? IdDireccionCobranza { get; set; }
        public string Direccion { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public bool? AplicarDetraccion { get; set; }
        public int? IdEmpresaTransporte { get; set; }
        public int? IdUnidadTransporte { get; set; }
        public int? IdConductor { get; set; }
        public int? IdAlmacen { get; set; }
        public int ConDespacho { get; set; }
        public string Glosa { get; set; }
        public int? IdSucursalAlterno { get; set; }
        public int? IdAlmacenAlterno { get; set; }
        public string NombrePC { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public IList<TransaccionImpuesto> TransaccionImpuesto { get; set; }
        public IList<MovimientoProducto> MovimientoProducto { get; set; }
        public Operacion Operacion { get; set; }

        public string NumeroDoc
        {
            get { return "S: " + SerieDocumento + " - N: " + NumeroDocumento; }
        }

        public string EstadoOrden
        {
            get { return Enum.GetName(typeof(TipoEstadoDocumento), Estado); }
        }

        public string Documento { get; set; }

        public string DocumentoRelacionado { get; set; }

        public string NombreEstado
        {
            get { return Enum.GetName(typeof(TipoEstadoTransaccion), Estado); }
        }

        public bool ActulizarNumeroDocReferencia { get; set; }

        public List<Inventario> Inventarios { get; set; }

        public string Proveedor { get; set; }

        public string RazonSocial { set; get; }

        public IList<Comun> Documentos { set; get; }

        public IList<Comun> CondicionesPago { set; get; }

        public IList<Moneda> Monedas { set; get; }

        public IList<Comun> Direcciones { set; get; }

        public IList<Almacen> Almacenes { set; get; }

        public IList<Almacen> AlmacenesAlt { set; get; }

        public IList<Sucursal> SucursalesAlt { set; get; }

        public int TipoVenta { get; set; }
    }
}