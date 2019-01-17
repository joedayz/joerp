
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IOperacionImpuestoRepository : IRepository<OperacionImpuesto>
    {
        void Guardar(int idOperacion, List<OperacionImpuesto> lista);

        IList<OperacionImpuesto> GetByOperacion(int idOperacion);

        IList<OperacionImpuesto> GetFiltered(int idImpuesto, int idOperacion);
    }
}
