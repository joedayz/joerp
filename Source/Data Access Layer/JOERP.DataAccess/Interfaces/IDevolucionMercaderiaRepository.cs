
namespace JOERP.DataAccess.Interfaces
{
    using System;
    using Business.Entity;
    using Business.Entity.DTO;

    public interface IDevolucionMercaderiaRepository : IRepository<DevolucionMercaderia>
    {
        bool Insertar(Transaccion transaccion);

        bool Actualizar(Transaccion transaccion);

        bool Eliminar(int idTransaccion, int idUsuario, int estado, DateTime fechaRegistro);
    }
}
