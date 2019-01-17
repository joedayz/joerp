
namespace JOERP.Helpers
{
    using System.Collections.Generic;

    public interface IPaged<T> where T : class
    {
        int Count(params object[] parameters);

        IList<T> GetPaged(params object[] parameters);
    }
}
