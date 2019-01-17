
namespace JOERP.DataAccess.Interfaces
{
    using System.Collections.Generic;
    using System.Data.Common;
    using Business.Entity;

    public interface ISerieDocumentoRepository : IRepository<SerieDocumento>
    {
        IList<SerieDocumento> GetByTipoDocumento(int idSucursal, int tipoDocumento);

        void UpdateSerieDocumento(int idSerieDocumento, int idSucursal, int idUsuario, DbTransaction transaction = null);
    }
}
