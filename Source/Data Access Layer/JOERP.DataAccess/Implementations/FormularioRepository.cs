
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using Business.Entity;
    using Interfaces;

    public class FormularioRepository : Repository<Formulario>, IFormularioRepository
    {
        public List<Formulario> Formularios(int idUsuario, int idModulo, int? idParent)
        {
            return (List<Formulario>)Get("usp_Select_Formulario_ByUsuarioModuloParent", idUsuario, idModulo, idParent);
        }

        public List<Formulario> Formularios(int idModulo, int? idParent)
        {
            return (List<Formulario>)Get("usp_Select_Formulario_ByModuloParent", idModulo, idParent);
        }

        public IList<Formulario> Modulos(int idUsuario)
        {
            return Get("usp_Select_Modulos_ByUsuario", idUsuario);
        }
    }
}