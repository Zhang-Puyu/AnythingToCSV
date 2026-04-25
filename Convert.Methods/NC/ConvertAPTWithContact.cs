using MachKit.NC.Toolpath;

namespace Convert.Methods.NC
{
    public class ConvertAPTWithContact : AbstractToolpathConverter
    {
        public override string FileFilter => "带切触点CLS刀轨 |*.cls;";

        private readonly APTWithContactReader reader = new APTWithContactReader();
        protected override AbstractToolpathReader Reader => reader;
    }
}
