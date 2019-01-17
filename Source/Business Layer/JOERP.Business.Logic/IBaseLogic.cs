
namespace JOERP.Business.Logic
{
    using System.Collections.Generic;

    public interface IBaseLogic<T> where T : class
    {
        IList<T> Get(params object[] parameters);

        IList<T> GetPaged(string sordColumn, string sordDirection, int rowsCount, int page, int count,
                          params object[] parameters);

        T Single(params object[] parameters);

        int Count(params object[] parameters);

        T Add(T entity);

        T Update(T entity);

        void Delete(params object[] parameters);
    }
}
