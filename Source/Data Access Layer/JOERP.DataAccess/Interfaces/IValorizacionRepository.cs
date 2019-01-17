
namespace JOERP.DataAccess.Interfaces
{
    using System.Data.Common;
    using Business.Entity;
    using Business.Entity.DTO;

    public interface IValorizacionRepository : IRepository<Valorizacion>
    {
        Valorizacion Obtener(int idValorizacion);

        Valorizacion Insertar(Valorizacion valorizacion);

        void Actualizar(Valorizacion valorizacion);

        void Valorizar(int idValorizacion);

        void Costear(Transaccion transaccion, DbTransaction transaction);
    }
}
