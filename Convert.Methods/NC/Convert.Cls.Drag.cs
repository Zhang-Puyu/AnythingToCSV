using System.Text.RegularExpressions;

namespace Convert.Methods.NC
{
    /// <summary>
    /// 处理MaxPac生成的带前倾角的cls刀轨文件
    /// </summary>
    public class ConvertClsDrag : ConvertClsStd
    {
        #region 单例模式
        private static ConvertClsDrag instance = null;
        public static new ConvertClsDrag Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertClsDrag();
                    return instance;
                }
            }
        }

        private ConvertClsDrag() { }
        #endregion

        protected override string Header => "x,y,z,i,j,k,f,s,drag";

        protected override int Dimension => 9;

        private readonly int DRAG = 8;

        private readonly string dragPattern = @"DRAG=\s*(-?\d+(\.\d+)?)";
        private Match dragMatch;

        protected override bool ParasePosition(in string line, ref float[] point)
        {
            // GOTO/108.4843,-280.0770,-19.5434,0.380709,-0.921682,0.074586 $$PT2 DRAG=-90.0
            if (line.StartsWith("GOTO/") && line.Contains("DRAG"))
            {
                string[] s = line.Replace("GOTO/", "").Split(',');
                try
                {
                    // 读取坐标
                    point[X] = float.Parse(s[0]);
                    point[Y] = float.Parse(s[1]);
                    point[Z] = float.Parse(s[2]);
                    point[I] = float.Parse(s[3]);
                    point[J] = float.Parse(s[4]);
                    point[K] = float.Parse(s[5].Split(' ')[0]);

                    // 读取DRAG
                    dragMatch = Regex.Match(line, dragPattern);
                    point[DRAG] = dragMatch.Success ? float.Parse(dragMatch.Groups[1].Value) : 360;
                    return true;
                }
                catch { return true; }
            }

            if (base.ParasePosition(line, ref point))
            {
                // 如果是不含前倾角的GOTO/行，调用父类方法处理
                return true;
            }
            return false;
        }
    }
}
