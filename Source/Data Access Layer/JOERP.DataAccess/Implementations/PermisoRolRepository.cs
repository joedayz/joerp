
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using Business.Entity;
    using Interfaces;

    public class PermisoRolRepository : Repository<PermisoRol>, IPermisoRolRepository
    {
        public void Guardar(int idFormulario, List<PermisoRol> lista)
        {
            var permisos = Get("usp_Select_PermisoRol_ByIdFormulario", idFormulario);
            foreach (var permiso in permisos)
            {
                if (lista.Any(x => x.IdRol == permiso.IdRol && x.IdTipoPermiso == permiso.IdTipoPermiso))
                    continue;
                Delete(permiso.IdPermisoRol);
            }

            var temporal = new List<PermisoRol>();
            temporal.AddRange(lista);
            for (var i = 0; i < temporal.Count; i++)
            {
                var permiso = temporal.ElementAt(i);
                if (!permisos.Any(x => x.IdRol == permiso.IdRol && x.IdTipoPermiso == permiso.IdTipoPermiso))
                    Add(permiso);
            }
        }

        public IList<PermisoRol> GetFiltered(int idUsuario, int idFormulario)
        {
            return Get("usp_Select_PermisoRol_GetFiltered2", idUsuario, idFormulario);
        }
    }
}
