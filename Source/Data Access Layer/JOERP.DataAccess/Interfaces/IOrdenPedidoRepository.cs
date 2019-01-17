
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity.DTO;

    public interface IOrdenPedidoRepository : IRepository<OrdenPedido>
    {
        bool Insertar(OrdenPedido transaccion);

        bool Actualizar(OrdenPedido transaccion);

        bool Eliminar(int idTransaccion, int idUsuarioModificacion, int estado);

        bool Aprobar(int idTransaccion, int idUsuarioModificacion, int estado);

        IList<OrdenPedido> BuscarOrden(int idOperacion, int idProveedor, string desde, string hasta, string documento);
    }
}
