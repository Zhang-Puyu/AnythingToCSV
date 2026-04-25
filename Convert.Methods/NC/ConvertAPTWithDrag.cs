using MachKit.NC.Toolpath;

namespace Convert.Methods.NC
{
    /// <summary>
    /// 处理MaxPac生成的带前倾角的cls刀轨文件
    /// </summary>
    public class ConvertAPTWithDarg : AbstractToolpathConverter
    {
        public override string FileFilter => "带前倾角CLS刀轨 |*.cls;";

        private readonly APTWithDragReader reader = new APTWithDragReader();
        protected override AbstractToolpathReader Reader => reader;
    }
}
