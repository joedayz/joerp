
namespace JOERP.DataAccess.Implementations
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Business.Entity;
    using Interfaces;

    public class SerieDocumentoRepository : Repository<SerieDocumento>, ISerieDocumentoRepository
    {
        public IList<SerieDocumento> GetByTipoDocumento(int idSucursal, int tipoDocumento)
        {
            return Get("usp_Select_SerieDocumento_ByTipoDocumento", idSucursal, tipoDocumento);
        }

        public void UpdateSerieDocumento(int idSerieDocumento, int idSucursal, int idUsuario, DbTransaction transaction = null)
        {
            var comandoSerieDocumento = Database.GetStoredProcCommand("UpdateSerieDocumento");
            Database.AddInParameter(comandoSerieDocumento, "pIdSerieDocumento", DbType.Int32, idSerieDocumento);
            Database.AddInParameter(comandoSerieDocumento, "pIdSucursal", DbType.Int32, idSucursal);
            Database.AddInParameter(comandoSerieDocumento, "pUsuarioModificacion", DbType.Int32, idUsuario);

            if (transaction == null)
            {
                Database.ExecuteNonQuery(comandoSerieDocumento);
            }
            else
            {
                Database.ExecuteNonQuery(comandoSerieDocumento, transaction);
            }
        }
    }
}
