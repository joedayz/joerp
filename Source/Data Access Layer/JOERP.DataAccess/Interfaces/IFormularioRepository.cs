
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IFormularioRepository : IRepository<Formulario>
    {
        List<Formulario> Formularios(int idUsuario, int idModulo, int? idParent);

        List<Formulario> Formularios(int idModulo, int? idParent);

        IList<Formulario> Modulos(int idUsuario);
    }
}
