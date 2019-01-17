
namespace JOERP.Business.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Enums;
    using Validations;

    [MetadataType(typeof(FormularioValidation))]
    public class Formulario
    {
        public int IdFormulario { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Assembly { get; set; }
        public string Direccion { get; set; }
        public int? IdParent { get; set; }
        public int Nivel { get; set; }
        public int IdModulo { get; set; }
        public int Estado { get; set; }
        public int? IdOperacion { get; set; }
        public int Orden { get; set; }

        public List<Formulario> Formularios { get; set; }
        public IList<Comun> Estados { get; set; }

        public string NombreEstado
        {
            get { return (Estado == (int)TipoEstado.Activo) ? "Activo" : "Inactivo"; }
        }

        public string AccionAlterna { set; get; }
        public IList<Comun> Modulos { get; set; }
        public IList<Comun> Operaciones { get; set; }
        public IList<Comun> Parents { get; set; }
        public IList<Comun> Roles { set; get; }
        public IList<PermisoRol> PermisosRoles { set; get; }
    }
}