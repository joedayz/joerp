
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Validations;

    [MetadataType(typeof(TipoCambioValidation))]
    public class TipoCambio
    {
        public int IdTipoCambio { get; set; }
        public DateTime? Fecha { get; set; }
        public int IdMoneda { get; set; }
        public decimal ValorCompra { get; set; }
        public decimal ValorVenta { get; set; }
        public int TipoCalculo { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModifcacion { get; set; }
        public Empresa Empresa { get; set; }
        public Moneda Moneda { get; set; }
        public string Nombre { get; set; }
        public IList<Moneda> Monedas { set; get; }
        public IList<Comun> TiposCalculo { set; get; }
    }
}