
namespace JOERP.DataAccess.Interfaces
{
    using Business.Entity;

    public interface IOperacionRepository : IRepository<Operacion>
    {
        int MaxId();
        bool ExisteCodigo(string codigo,int id);
    }
}
