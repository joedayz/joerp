
namespace JOERP.WebSite.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    public static class TreeViewHtmlHelper
    {
        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        public static MvcHtmlString TreeView<T>(this HtmlHelper html, string treeId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> parentContent, Func<T, string> itemContent)
        {
            return html.TreeView(treeId, string.Empty, rootItems, childrenProperty, parentContent, itemContent, true, string.Empty, null);
        }

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        /// <param name="includeJavaScript">If true, output will automatically render the JavaScript to turn the ul into the treeview</param>
        public static MvcHtmlString TreeView<T>(this HtmlHelper html, string treeId, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> parentContent, Func<T, string> itemContent, bool includeJavaScript)
        {
            return html.TreeView(treeId, string.Empty, rootItems, childrenProperty, parentContent, itemContent, includeJavaScript, string.Empty, null);
        }

        /// <summary>
        /// Create a TreeView of nodes starting from a root element
        /// </summary>
        /// <param name="treeId">The ID that will be used when the ul is created</param>
        /// <param name="rootItems">The root nodes to create</param>
        /// <param name="childrenProperty">A lambda expression that returns the children nodes</param>
        /// <param name="itemContent">A lambda expression defining the content in each tree node</param>
        /// <param name="includeJavaScript">If true, output will automatically render the JavaScript to turn the ul into the treeview</param>
        /// <param name="emptyContent">Content to be rendered when the tree is empty</param>
        /// <param name="includeJavaScript">If true, output will automatically into the JavaScript to turn the ul into the treeview</param>
        public static MvcHtmlString TreeView<T>(this HtmlHelper html, string treeId, string label, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> parentContent, Func<T, string> itemContent, bool includeJavaScript, string classUL, string emptyContent)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("<ul id='{0}' class='{1}'>\r\n", treeId, classUL);

            if (!string.IsNullOrEmpty(label))
            {
                sb.AppendFormat("<li><span style='border-top: 0px !important;'>{0}</span>", label);
                sb.AppendFormat("<ul>\r\n");
            }

            if (rootItems.Count() == 0)
            {
                sb.AppendFormat("<li>{0}</li>", emptyContent);
            }

            foreach (T item in rootItems)
            {
                var children = childrenProperty(item);

                RenderLi(sb, item, children.Count() > 0 ? parentContent : itemContent);
                AppendChildren(sb, item, childrenProperty, parentContent, itemContent);
            }

            sb.AppendLine(!string.IsNullOrEmpty(label) ? "</li></ul>" : "</ul>");

            if (includeJavaScript)
            {
                sb.AppendFormat(
                    @"<script type='text/javascript'>
                    $(document).ready(function() {{
                        $('#{0}').treeview({{ animated: 'fast' }});
                    }});
                </script>", treeId);
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        private static void AppendChildren<T>(StringBuilder sb, T root, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> parentContent, Func<T, string> itemContent)
        {
            var children = childrenProperty(root);
            if (children.Count() == 0)
            {
                sb.AppendLine("</li>");
                return;
            }

            sb.AppendLine("\r\n<ul>");
            foreach (T item in children)
            {
                children = childrenProperty(item);

                RenderLi(sb, item, children.Count() > 0 ? parentContent : itemContent);
                AppendChildren(sb, item, childrenProperty, parentContent, itemContent);
            }

            sb.AppendLine("</ul></li>");
        }

        private static void RenderLi<T>(StringBuilder sb, T item, Func<T, string> itemContent)
        {
            sb.AppendFormat("<li>{0}", itemContent(item));
        }
    }
}