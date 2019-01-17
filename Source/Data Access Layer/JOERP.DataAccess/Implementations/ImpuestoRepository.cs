
namespace JOERP.DataAccess.Implementations
{
    using Business.Entity;
    using Interfaces;

    public class ImpuestoRepository : Repository<Impuesto>, IImpuestoRepository
    {
        public bool ExisteImpuesto(int id)
        {
            return false;
        }
    }
}
