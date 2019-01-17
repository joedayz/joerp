
namespace JOERP.Business.Logic
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Implementations;
    using DataAccess.Interfaces;
    using Entity;
    using Helpers;

    public class ProductoBL : Singleton<ProductoBL>, IPaged<Producto>
    {
        private readonly IProductoRepository repository = new ProductoRepository();

        public int Count(params object[] parameters)
        {
            try
            {
                return repository.Count(parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Producto> GetPaged(params object[] parameters)
        {
            try
            {
                return repository.GetPaged(parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Producto> GetByTipoProducto(int idEmpresa, int tipoProducto)
        {
            try
            {
                return repository.GetByTipoProducto(idEmpresa, tipoProducto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool ExisteCodigo(string codigo, int id, int empresaId)
        {
            try
            {
                return repository.ExisteCodigo(codigo, id, empresaId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Producto> GetComponentes(string codigo)
        {
            try
            {
                return repository.GetComponentes(codigo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Producto> GetProductosByCodigoAndTipo(string codigo, int tipo)
        {
            try
            {
                return repository.GetProductosByCodigoAndTipo(codigo, tipo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Producto> GetByCodigo(int idEmpresa, string codigo)
        {
            try
            {
                return repository.GetByCodigo(idEmpresa, codigo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public IList<Producto> GetByCodigoAlterno(int idEmpresa, string codigo)
        {
            try
            {
                return repository.GetByCodigoAlterno(idEmpresa, codigo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Producto> GetProductoByFiltro(int idEmpresa, string filtro)
        {
            try
            {
                return repository.GetProductoByFiltro(idEmpresa, filtro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Producto GetByCodigo(int idEmpresa, int tipoProducto, string codigo)
        {
            try
            {
                return repository.GetByCodigo(idEmpresa, tipoProducto, codigo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int MaxId()
        {
            try
            {
                return repository.MaxId();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Producto Add(Producto entity)
        {
            try
            {
                return repository.Add(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Producto Update(Producto entity)
        {
            try
            {
                return repository.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void GuardarComponentes(List<ProductoComponente> componentes)
        {
            try
            {
                repository.GuardarComponentes(componentes);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int id)
        {
            try
            {
                 repository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Inventario> GetInventario(int idAlmacen)
        {
            try
            {
                return repository.GetInventario(idAlmacen);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Inventario> SelectInventarioByParameters(int idEmpresa, int idSucursal, int idAlmacen, int idProveedor, int tipo, string texto, string estructura)
        {
            try
            {
                return repository.SelectInventarioByParameters(idEmpresa, idSucursal, idAlmacen, idProveedor, tipo, texto, estructura);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Producto> GetAll()
        {
            try
            {
                return repository.Get();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Producto Single(int? id)
        {
            try
            {
                return repository.Single(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<Producto> GetByNombre(string nombre)
        {
            try
            {
                return repository.GetByNombre(nombre);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<ProductoStock> GetLotesProductoByAlmacen(int idPresentacion, int idAlmacen)
        {
            try
            {
                return repository.GetLotesProductoByAlmacen(idPresentacion, idAlmacen);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
