
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using Business.Entity;

    public interface IProductoRepository : IRepository<Producto>
    {
        int MaxId();
        bool ExisteCodigo(string codigo, int id, int empresaId);
        List<Inventario> GetInventario(int idAlmacen);
        List<Inventario> SelectInventarioByParameters(int idEmpresa, int idSucursal, int idAlmacen, int idProveedor, int tipo, string texto, string estructura);
        IList<Producto> GetByTipoProducto(int idEmpresa, int tipoProducto);
        IList<Producto> GetComponentes(string codigo);
        IList<Producto> GetProductosByCodigoAndTipo(string codigo, int tipo);
        IList<Producto> GetByCodigoAlterno(int idEmpresa, string codigo);
        IList<Producto> GetByCodigo(int idEmpresa, string codigo);
        IList<Producto> GetProductoByFiltro(int idEmpresa, string filtro);
        Producto GetByCodigo(int idEmpresa, int tipoProducto, string codigo);
        IList<Producto> GetByNombre(string nombre);
        void GuardarComponentes(List<ProductoComponente> componentes);
        IList<ProductoStock> GetLotesProductoByAlmacen(int idPresentacion, int idAlmacen);
    }
}
