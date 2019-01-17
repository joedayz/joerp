
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity.DTO;

    public interface IOrdenCompraRepository : IRepository<OrdenCompra>
    {
        bool Insertar(OrdenCompra transaccion);

        bool Actualizar(OrdenCompra transaccion);

        bool Eliminar(int idTransaccion, int idUsuarioModificacion, int estado);

        bool Aprobar(int idTransaccion, int idUsuarioModificacion, int estado);

        IList<OrdenCompra> BuscarOrden(int idOperacion,int idProveedor, string desde, string hasta, string documento);
    }
}
