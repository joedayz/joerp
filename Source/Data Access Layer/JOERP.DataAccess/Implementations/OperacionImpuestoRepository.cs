
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using Business.Entity;
    using Interfaces;

    public class OperacionImpuestoRepository : Repository<OperacionImpuesto>, IOperacionImpuestoRepository
    {
        public void Guardar(int idOperacion, List<OperacionImpuesto> lista)
        {
            var operacionImpuestos = Get("usp_Select_OperacionImpuesto_ByIdOperacion", idOperacion);
            foreach (var item in operacionImpuestos)
            {
                Delete("usp_Delete_OperacionImpuesto",item.IdOperacion, item.IdImpuesto);
            }
            var temporal = new List<OperacionImpuesto>();
            temporal.AddRange(lista);
            for (var i = 0; i < temporal.Count; i++)
            {
                var opimp = temporal.ElementAt(i);
                Add(opimp);
            }
        }

        public IList<OperacionImpuesto> GetByOperacion(int idOperacion)
        {
            return Get("usp_Select_OperacionImpuesto_ByIdOperacion", idOperacion);
        }

         public IList<OperacionImpuesto> GetFiltered(int idImpuesto, int idOperacion)
         {
             return Get("usp_Select_OperacionImpuesto_GetFiltered", idImpuesto, idOperacion);
         }
    }
}
