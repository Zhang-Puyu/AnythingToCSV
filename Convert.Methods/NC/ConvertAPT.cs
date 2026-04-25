using MachKit.NC.Toolpath;

namespace Convert.Methods.NC
{
    public class ConvertAPT : AbstractToolpathConverter
    {
        public override string FileFilter => "普通CLS刀轨 |*.cls;";

        private readonly APTReader reader = new APTReader();
        protected override AbstractToolpathReader Reader => reader;
    }
}
