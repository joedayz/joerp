
namespace JOERP.Helpers
{
    using System;

    public static class Extensiones
    {
        /// <summary>
        /// Redondear un numero a Sesion.DecimalesRedondeo 
        /// </summary>
        /// <param name="value">Valor a redondear</param>
        /// <returns>El valor redondeado</returns>
        public static decimal Redondear(this decimal value)
        {
            return Math.Round(value, Constantes.DecimalesRedondeo, MidpointRounding.AwayFromZero);
        }

        public static decimal Redondear(this decimal value, int decimales)
        {
            return Math.Round(value, decimales, MidpointRounding.AwayFromZero);
        }

    }
}
