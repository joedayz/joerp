
namespace JOERP.DataAccess.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Business.Entity;
    using Business.Entity.DTO;

    public interface IAjusteInventarioRepository : IRepository<Transaccion>
    {
        bool Insertar(Transaccion transaccion);

        bool Actualizar(Transaccion transaccion);

        bool Eliminar(int idTransaccion, int idUsuario, int estado, DateTime fechaRegistro);
    }
}
