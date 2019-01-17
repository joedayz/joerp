
namespace JOERP.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using JqGrid;
    using Newtonsoft.Json;

    public static class Utils
    {
        #region Guardar Archivos

        public static bool GuardarArchivo(byte[] buffer, string pathDirectrio, string nombreArchivo)
        {
            if (!Directory.Exists(pathDirectrio))
                Directory.CreateDirectory(pathDirectrio);

            string pathArchivo = Path.Combine(pathDirectrio, nombreArchivo);

            try
            {
                var fs = new FileStream(pathArchivo, FileMode.Create, FileAccess.ReadWrite);
                var bw = new BinaryWriter(fs);
                bw.Write(buffer);
                bw.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Guardar Archivos

        #region Formatear datos

        /// <summary>
        /// Retira todos los caracteres especiales de un texto especificado
        /// </summary>
        /// <param name="text">texto a formatear</param>
        /// <returns>retorna un texto sin caracteres especiales</returns>
        public static string RemoveIllegalCharacters(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var regex = new Regex(@"(\s|-)+", RegexOptions.Compiled);

            text = text.Replace(":", string.Empty);
            text = text.Replace("/", string.Empty);
            text = text.Replace("?", string.Empty);
            text = text.Replace("#", string.Empty);
            text = text.Replace("[", string.Empty);
            text = text.Replace("]", string.Empty);
            text = text.Replace("@", string.Empty);
            text = text.Replace("*", string.Empty);
            text = text.Replace(",", string.Empty);
            text = text.Replace(".", string.Empty);
            text = text.Replace(";", string.Empty);
            text = text.Replace("\"", string.Empty);
            text = text.Replace("&", string.Empty);
            text = text.Replace("'", string.Empty);
            text = regex.Replace(text, "-");
            text = RemoveCharacters(text);

            return text;
        }

        /// <summary>
        /// Remover caracteres especiales
        /// </summary>
        /// <param name="text">texto a analizar</param>
        /// <returns></returns>
        private static string RemoveCharacters(string text)
        {
            String normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            for (int i = 0; i < normalized.Length; i++)
            {
                Char c = normalized[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return HttpUtility.UrlEncode(sb.ToString()).Replace("%", string.Empty);
            ;
        }

        /// <summary>
        /// Retira todas las etiquetas html del string especificado
        /// </summary>
        /// <param name="html">el estring que cntiene el html</param>
        /// <returns>un string sin etiquetas html</returns>
        public static string StripHtml(string html)
        {
            var strip_HTML = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            return strip_HTML.Replace(html, string.Empty);
        }

        #endregion Formatear datos

        #region Manejo de URLs

        private static string relativeWebRoot;

        /// <summary>
        /// Retorna la ruta relativa al sitio
        /// </summary>
        public static string RelativeWebRoot
        {
            get
            {
                if (relativeWebRoot == null)
                    relativeWebRoot = VirtualPathUtility.ToAbsolute("~/");
                return relativeWebRoot;
            }
        }

        /// <summary>
        /// Retorna la ruta absoluta al sitio
        /// </summary>
        public static Uri AbsoluteWebRoot
        {
            get
            {
                var context = HttpContext.Current;
                if (context == null)
                    throw new WebException("El actual HttpContext es nulo");

                if (context.Items["absoluteurl"] == null)
                    context.Items["absoluteurl"] =
                        new Uri(context.Request.Url.GetLeftPart(UriPartial.Authority) + RelativeWebRoot);

                return context.Items["absoluteurl"] as Uri;
            }
        }

        #endregion Manejo de URLs

        #region Emails

        public static bool SendEmail(string email, string asunto, string mensaje)
        {
            var message = new MailMessage();
            message.To.Add(email);
            message.Subject = asunto;
            message.IsBodyHtml = true;
            message.Body = mensaje;
            message.BodyEncoding = Encoding.UTF8;

            try
            {
                var smtp = new SmtpClient();
                smtp.Send(message);
                return true;
            }
            catch (SmtpException)
            {
                return false;
            }
            finally
            {
                message.Dispose();
            }
        }

        #endregion Emails

        #region Manejo de datos

        public static List<SelectListItem> ConvertToListItem<T>(IList<T> list, string value, string text, bool agregarOpcionSeleccion = true)
        {
            var listItems = (from entity in list
                             let propiedad1 = entity.GetType().GetProperty(value)
                             where propiedad1 != null
                             let valor1 = propiedad1.GetValue(entity, null)
                             where valor1 != null
                             let propiedad2 = entity.GetType().GetProperty(text)
                             where propiedad2 != null
                             let valor2 = propiedad2.GetValue(entity, null)
                             where valor2 != null
                             select new SelectListItem
                             {
                                 Value = valor1.ToString(),
                                 Text = valor2.ToString()
                             })
                .OrderBy(p => p.Text)
                .ToList();

            if (agregarOpcionSeleccion)
            {
                listItems.Insert(0, new SelectListItem { Text = "-- Seleccionar --", Value = "0" });   
            }
            return listItems;
        }

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

        public static List<Comun> ConvertToComunList<T>(IList<T> list, string value, string text)
        {
            var listItems = (from entity in list
                             let propiedad1 = entity.GetType().GetProperty(value)
                             where propiedad1 != null
                             let valor1 = propiedad1.GetValue(entity, null)
                             where valor1 != null
                             let propiedad2 = entity.GetType().GetProperty(text)
                             where propiedad2 != null
                             let valor2 = propiedad2.GetValue(entity, null)
                             where valor2 != null
                             select new Comun
                             {
                                 Id = Convert.ToInt32(valor1),
                                 Nombre = valor2.ToString()
                             })
                .OrderBy(p => p.Nombre)
                .ToList();
            listItems.Insert(0, new Comun { Nombre = "-- Seleccionar --", Id = 0 });
            return listItems;
        }

        #endregion Manejo de datos

        #region SQL

        public static string ConvertToSql(JqGrid.Filter filtros)
        {
            var whereFiltro = string.Empty;

            foreach (var filtro in filtros.rules)
            {
                var filterFormatString = string.Empty;

                switch (filtro.op)
                {
                    case "bw":
                        filterFormatString = " {0} {1} like '{2}%'";
                        break; // TODO: might not be correct. Was : Exit Select

                    //equal ==
                    case "eq":
                        filterFormatString = " {0} {1} = {2}";
                        break; // TODO: might not be correct. Was : Exit Select

                    //not equal !=
                    case "ne":
                        filterFormatString = " {0} {1} <> {2}";
                        break; // TODO: might not be correct. Was : Exit Select

                    //string.Contains()
                    case "cn":
                        filterFormatString = " {0} {1} like '%{2}%'";
                        break; // TODO: might not be correct. Was : Exit Select
                    case "dt":

                        filterFormatString = " {0} {1} = DATETIME'{2}'";
                        filtro.data = Convert.ToDateTime(filtro.data).ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                        break; // TODO: might not be correct. Was : Exit Select
                }

                whereFiltro += string.Format(filterFormatString, filtros.groupOp, filtro.field, filtro.data);
            }

            return filtros.rules.Count() == 0 ? string.Empty : whereFiltro.Substring(4);
        }

        public static string GetWhere(string filters, List<Rule> rules)
        {
            var where = string.Empty;
            var filtro = (string.IsNullOrEmpty(filters)) ? null : JsonConvert.DeserializeObject<JqGrid.Filter>(filters);

            if ((filtro != null))
            {
                where = ConvertToSql(filtro);
            }

            if ((rules != null))
            {
                foreach (var regla in rules.Where(regla => (!(string.IsNullOrEmpty(regla.data)) & regla.data != "0")))
                {
                    if (string.IsNullOrEmpty(regla.op))
                    {
                        regla.op = "=";
                    }
                    where += " and " + regla.field + regla.op + regla.data;
                }
            }

            return string.IsNullOrEmpty(@where) ? where : (where.StartsWith(" and ") ? where.Substring(4) : where);
        }

        public static string GetWhere(string columna, string operacion, string valor)
        {
            var opciones = new Dictionary<string, string>
                               {
                                   {"eq", "="},
                                   {"ne", "<>"},
                                   {"lt", "<"},
                                   {"le", "<="},
                                   {"gt", ">"},
                                   {"ge", ">="},
                                   {"bw", "LIKE"},
                                   {"bn", "NOT LIKE"},
                                   {"in", "LIKE"},
                                   {"ni", "NOT LIKE"},
                                   {"ew", "LIKE"},
                                   {"en", "NOT LIKE"},
                                   {"cn", "LIKE"},
                                   {"nc", "NOT LIKE"}
                               };

            if (string.IsNullOrEmpty(operacion))
            {
                return string.Empty;
            }
            else
            {
                if (operacion.Equals("bw") || operacion.Equals("bn"))
                {
                    valor = valor + "%";
                }
                if (operacion.Equals("ew") || operacion.Equals("en"))
                {
                    valor = "%" + valor;
                }
                if (operacion.Equals("cn") || operacion.Equals("nc") || operacion.Equals("in") || operacion.Equals("ni"))
                {
                    valor = "%" + valor + "%";
                }
                if (opciones.Take(6).ToDictionary(p => p.Key, p => p.Value).ContainsKey(operacion))
                {
                    return string.IsNullOrEmpty(valor) ? string.Empty : (columna + " ") + opciones[operacion] + " " + valor;
                }

                return columna + " " + opciones[operacion] + " '" + valor + "'";
            }
        }

        #endregion SQL
    }
}
