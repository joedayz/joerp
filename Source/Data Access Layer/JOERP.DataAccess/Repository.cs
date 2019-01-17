using System.Configuration;

namespace JOERP.DataAccess
{
    public abstract class Repository<T> : BaseRepository<T>
        where T : class, new()
    {
        #region Propiedades

        protected string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; }
        }

        #endregion
    }
}
