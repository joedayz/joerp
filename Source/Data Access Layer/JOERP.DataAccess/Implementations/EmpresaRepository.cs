
namespace JOERP.DataAccess.Implementations
{
    using Business.Entity;
    using Interfaces;

    public class EmpresaRepository : Repository<Empresa>, IEmpresaRepository
    {
        public Empresa SingleByRazonSocial(string razonSocial)
        {
            return Single("usp_Single_Empresa_ByRazonSocial", razonSocial);
        }
    }
}
