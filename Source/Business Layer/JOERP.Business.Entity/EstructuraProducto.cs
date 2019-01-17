
namespace JOERP.Business.Entity
{
    using System;
    using System.Collections.Generic;
    using Helpers;

    public class EstructuraProducto
    {
        public int IdEstructuraProducto { get; set; }
        public string Nombre { get; set; }
        public int? IdParent { get; set; }
        public int Nivel { get; set; }
        public int Total { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModificacion { get; set; }
        public Empresa Empresa { get; set; }

        public List<EstructuraProducto> EstructurasProducto { get; set; }
        public string AccionAlterna { set; get; }

        public IList<Comun> Operaciones { get; set; }
        public IList<Comun> Parents { get; set; }
    }
}