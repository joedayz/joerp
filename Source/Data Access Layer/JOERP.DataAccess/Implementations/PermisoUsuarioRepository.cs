
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using Business.Entity;
    using Interfaces;

    public class PermisoUsuarioRepository : Repository<PermisoUsuario>, IPermisoUsuarioRepository 
    {
        public void Guardar(int idFormulario, List<PermisoUsuario> lista)
        {
            var permisos = Get("usp_Select_PermisoUsuario_ByIdFormulario", idFormulario);//
            foreach (var permiso in permisos)
            {
                if (lista.Any(x => x.IdEmpleado == permiso.IdEmpleado && x.IdTipoPermiso == permiso.IdTipoPermiso))
                    continue;
                Delete(permiso.IdPermisoUsuario);
            }
            var temporal = new List<PermisoUsuario>();
            temporal.AddRange(lista);
            for (var i = 0; i < temporal.Count; i++)
            {
                var permiso = temporal.ElementAt(i);
                if (!permisos.Any(x => x.IdEmpleado == permiso.IdEmpleado && x.IdTipoPermiso == permiso.IdTipoPermiso))
                    Add(permiso);   
            }
        }

        public IList<PermisoUsuario> GetFiltered(int idUsuario, int idFormulario)
        {
            return Get("usp_Select_GetFiltered2", idUsuario, idFormulario);
        }

        public IList<PermisoUsuario> GetFiltered(int idUsuario, int idFormulario, int idRol)
        {
            return Get("usp_Select_GetFiltered3", idUsuario, idFormulario, idRol);
        }
    }
}
