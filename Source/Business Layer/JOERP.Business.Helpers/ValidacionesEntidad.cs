﻿
namespace JOERP.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Clase para validar las entidades.
    /// </summary>
    public static class ValidacionesEntidad
    {
        /// <summary>
        /// Verifica si la cadena no es un caracter null o un espacio en blanco.
        /// </summary>
        /// <param name="value">Cadena a validar.</param>
        /// <returns>Retorna true si la cadena es valida, en caso contrario false</returns>
        public static bool ValidarExiste(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length == 0)
                return false;
            else
                return true;
        }

        public static bool ValidarExisteNumero(decimal value)
        {
            if (value == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Valida la longitud maxima de una cadena.
        /// </summary>
        /// <param name="value">Cadena a validar.</param>
        /// <param name="longitud">Longitud maxima.</param>
        /// <returns>Retorna true si la cadena es valida, en caso contrario false.</returns>
        public static bool ValidarLongitudMaxima(string value, int longitud)
        {
            if (value != null && value.Length > longitud)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Valida la longitud minima de una cadena.
        /// </summary>
        /// <param name="value">Cadena a validar.</param>
        /// <param name="longitud">Longitud minima.</param>
        /// <returns>Retorna true si la cadena es valida, en caso contrario false.</returns>
        public static bool ValidarLongitudMinima(string value, int longitud)
        {
            if (value != null && value.Length < longitud)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Valida un email.
        /// <remarks>Retorna true si el email es valido, en caso contrario false.</remarks>
        /// </summary>
        /// <param name="email">Email a validar.</param>
        /// <returns>Retorna true si el email es valido, en caso contrario false.</returns>
        public static bool ValidarEmail(string email)
        {
            if (email == null)
                return false;

            if (Regex.IsMatch(email, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Valida direcciones URL sin (http://)...
        /// </summary>
        /// <param name="url">URL a validar</param>
        /// <returns>True si la url es correcta, en casoc ontrario false.</returns>
        public static bool ValidarURL(string url)
        {
            if (url == null)
                return false;
            //(ht|f)tp(s?)\:\/\/
            if (Regex.IsMatch(url, @"^[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)( [a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Valida un RUC
        /// </summary>
        /// <param name="RUC">RUC a validar</param>
        /// <returns>Retorna true si el RUC es valido, en caso contrario false.</returns>
        public static bool ValidarRUC(string RUC)
        {
            if (RUC == null) return false;
            if (RUC.Length != 11)
                return false;

            string prefijo = RUC.Substring(0, 2);
            if (prefijo != "10" && prefijo != "20")
                return false;

            return true;
        }

        public static bool ValidarDNI(string Dni)
        {
            if (Dni == null) return false;
            if (Dni.Length != 8)
                return false;

            if (Regex.IsMatch(Dni, "0*[1-9][0-9]*"))
                return true;
            else
                return false;
        }

        public static bool ValidarEstado(char estado)
        {
            if (estado != 'A' && estado != 'I' && estado != 'S')
                return false;

            return true;
        }

        public static bool ValidarTipoCliente(char tipoCliente)
        {
            if (tipoCliente != 'N' && tipoCliente != 'J')
                return false;

            return true;
        }

        public static bool ValidarMayoriaEdad(DateTime fechaNacimiento)
        {
            int age = new DateTime(DateTime.Now.Date.Subtract(fechaNacimiento).Ticks).Year - 1;
            if (age >= 18)
                return true;
            else
                return false;

        }

        public static bool ValidarID(int id)
        {
            if (id <= 0) //|| id==null)
                return false;

            return true;
        }

        internal static bool ValidarValorMaximo(int value, int maxValue)
        {
            if (value > maxValue)
                return false;
            else
                return true;
        }

        internal static bool ValidarValorMinimo(int value, int minValue)
        {
            if (value < minValue)
                return false;
            else
                return true;
        }

        internal static bool ValidarValorMaximo(decimal value, decimal maxValue)
        {
            if (value > maxValue)
                return false;
            else
                return true;
        }

        internal static bool ValidarValorMinimo(decimal value, decimal minValue)
        {
            if (value < minValue)
                return false;
            else
                return true;
        }


        /// <summary>
        /// Determina si el contenido de una propiedad es unico.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto</typeparam>
        /// <param name="items">Lista de items a examinar.</param>
        /// <param name="property">Propiedad a relacionar.</param>
        /// <param name="propertyValueAsString">Valor de la prpiedad a buscar.</param>
        /// <returns>True, la propiedad es unica, en caso contrario false.</returns>
        public static bool EsValorPropiedadUnico<T>(List<T> items, string property, string propertyValueAsString)
        {
            foreach (T item in items)
            {
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    if (propertyInfo.Name.Equals(property, StringComparison.OrdinalIgnoreCase))
                    {

                        var value = propertyInfo.GetValue(item, null).ToString();
                        value = RemoverEspaciosACadena(value);

                        if (Convert.ChangeType(propertyValueAsString, propertyInfo.PropertyType) is string)
                        {
                            string cadenaSinEpacios = RemoverEspaciosACadena(propertyValueAsString);

                            if (value.ToString().Equals(cadenaSinEpacios, StringComparison.OrdinalIgnoreCase))
                                return false;
                            else
                                break;
                        }
                    }
                }
            }
            return true;
        }

        public static string RemoverEspaciosACadena(string propertyValueAsString)
        {
            var valores = propertyValueAsString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string cadena = string.Empty;
            foreach (var item in valores)
            {
                cadena += item.Trim();
            }
            return cadena;
        }

        /// <summary>
        /// Determina si el objeto actual no tiene errores.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto a verificar.</typeparam>
        /// <param name="obj">El objeto en si.</param>
        /// <returns></returns>
        public static bool Validate<T>(T obj)
        {
            IDataErrorInfo entidad;
            entidad = obj as IDataErrorInfo;
            return (entidad != null && entidad.Error != null) ? false : true;
        }
    }
}
