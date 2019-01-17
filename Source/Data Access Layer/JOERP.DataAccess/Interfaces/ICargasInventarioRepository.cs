
namespace JOERP.DataAccess.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Business.Entity;
    using Business.Entity.DTO;

    public interface ICargasInventarioRepository : IRepository<Transaccion>
    {
        bool Insertar(Transaccion transaccion);

        List<CargasInventarioDTO> Buscar(int idAlmacen, DateTime? fechaInicio, DateTime? fechaFin, int idOperacion, int start, int rows, out int count);
    }
}
