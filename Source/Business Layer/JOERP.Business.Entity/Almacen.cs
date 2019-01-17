
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(AlmacenValidation))]
    public class Almacen
    {
        public int IdAlmacen { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Estado { get; set; }
        public int IdSucursal { get; set; }
        public string Abreviatura { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public Sucursal Sucursal { get; set; }
        public List<Comun> Sucursales { set; get; }
        public List<Comun> Estados { set; get; }
        public string NombreEstado
        {
            get { return (Estado == (int)TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }

        public string NombreSucursal { set; get; }
    }
}