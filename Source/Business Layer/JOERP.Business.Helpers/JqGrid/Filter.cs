
namespace JOERP.Helpers.JqGrid
{
    using System.Collections.Generic;

    public class Filter
    {
        public string groupOp { get; set; }

        public List<Rule> rules { get; set; }
    }
}