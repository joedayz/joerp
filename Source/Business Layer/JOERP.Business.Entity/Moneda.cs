
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(MonedaValidation))]
    public class Moneda
    {
        public int IdMoneda { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public int Estado { get; set; }
        public bool EsLocal { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public int IdEmpresa { get; set; }
        public string Simbolo { get; set; }
        public Empresa Empresa { get; set; }

        public string NombreEstado
        {
            get { return Enum.GetName(typeof(TipoEstado), this.Estado); }
        }

        public List<Comun> Estados { get; set; }

        public List<Comun> Empresas { get; set; }
    }
}