using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JOERP.WebSite.Areas.Administracion.Models
{
    public class JsTreeModel
    {
        public string state = "open";
        public JsTreeNodeData data { get; set; }

        public JsTreeAttribute attr { get; set; }

        public List<JsTreeModel> children { get; set; }
    }

    public class JsTreeAttribute
    {
        public string id { get; set; }

        public string rel { get; set; }

        public string nodo { get; set; }
    }

    public class JsTreeNodeData
    {
        public string title { get; set; }

        public string icon { get; set; }
    }
}