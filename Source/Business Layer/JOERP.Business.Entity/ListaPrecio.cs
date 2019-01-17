
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;

    public class ListaPrecio
    {
        public ListaPrecio()
        {
            EmpleadoListaPrecios = new List<EmpleadoListaPrecio>();
            ProductoPrecios = new List<ProductoPrecio>();
        }

        public int IdListaPrecio { get; set; }
        public int IdEmpresa { get; set; }
        public int IdMoneda { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public bool EsListaBase { get; set; }
        public bool AutoActualizar { get; set; }
        public string Descripcion { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public IList<EmpleadoListaPrecio> EmpleadoListaPrecios { get; set; }
        public Empresa Empresa { get; set; }
        public Moneda Moneda { get; set; }
        public IList<ProductoPrecio> ProductoPrecios { get; set; }
    }
}