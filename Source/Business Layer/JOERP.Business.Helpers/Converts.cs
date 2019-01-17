
namespace JOERP.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Converts
    {
        public static List<Comun> EnumToList<T>()
        {
            var enumType = typeof(T);

            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T debe ser de tipo System.Enum");

            var enumValArray = Enum.GetValues(enumType);
            var enumValList = (from object l in enumValArray
                               select new Comun
                               {
                                   Id = Convert.ToInt32(l),
                                   Nombre = Enum.GetName(enumType, l)
                               })
                .OrderBy(p => p.Nombre)
                .ToList();
            return enumValList;
        }
    }
}
