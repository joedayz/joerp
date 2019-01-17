
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Validations;
    using Helpers;

    [MetadataType(typeof(UnidadMedidaValidation))]
    public class UnidadMedida
    {
        public int IdUnidadMedida { get; set; }
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Empresa Empresa { get; set; }

        public string NombreEmpresa
        {
            get { return Empresa == null ? string.Empty : Empresa.RazonSocial; }
        }

        public List<Comun> Empresas { get; set; }
    }
}