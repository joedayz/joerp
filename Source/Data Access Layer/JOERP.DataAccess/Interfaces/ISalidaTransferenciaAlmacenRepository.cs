
namespace JOERP.DataAccess.Interfaces
{
    using System;
    using Business.Entity;
    using Business.Entity.DTO;

    public interface ISalidaTransferenciaAlmacenRepository : IRepository<Transferencia>
    {
        bool Insertar(Transaccion transaccion);

        bool Eliminar(int idTransaccion, int idUsuario, int estado, DateTime fechaRegistro);
    }
}
