using MachKit.NC.Toolpath;

namespace Convert.Methods.NC
{
    public class ConvertGCode : AbstractToolpathConverter
    {
        public override string FileFilter => "普通G代码刀轨 |*.nc;*.mpf;";

        private readonly GCodeReader reader = new GCodeReader();
        protected override AbstractToolpathReader Reader => reader;
    }
}
