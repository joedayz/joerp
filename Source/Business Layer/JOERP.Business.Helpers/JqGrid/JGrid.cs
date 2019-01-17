
namespace JOERP.Helpers.JqGrid
{
    public class JGrid
    {
        public int total { get; set; }

        public int page { get; set; }

        public int records { get; set; }

        public int start { get; set; }

        public JRow[] rows { get; set; }
    }
}