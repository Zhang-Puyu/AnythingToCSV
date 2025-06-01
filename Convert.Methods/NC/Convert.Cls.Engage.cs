using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Convert.Methods.NC
{
    public class ConvertClsEngage : ConvertClsStd
    {
        #region 单例模式
        private static ConvertClsEngage instance = null;
        public static new ConvertClsEngage Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertClsEngage();
                    return instance;
                }
            }
        }

        private ConvertClsEngage() { }
        #endregion

        #region apt语言解析

        protected override string Header => "x,y,z,i,j,k,f,s,engage_x,engage_y,engage_z";

        protected override int Dimension => 11;

        private readonly int ENGAGE_X = 8, ENGAGE_Y = 9, ENGAGE_Z = 10;

        protected override bool ParasePosition(in string line, ref float[] point)
        {
            // GOTO/ 5.5417,37.8811,-11.0780 $$ 5.0000,39.3745,-11.2777
            if (line.StartsWith("GOTO/") && line.Contains("$$"))
            {
                point[S] = SpindleSpeed;
                point[F] = RapidFlag ? RapidFeed : FeedRate;

                RapidFlag = RapidFlag ? false : RapidFlag;

                string[] subLines = line.Split(new[] { "$$" }, StringSplitOptions.RemoveEmptyEntries);
                string[] centerCells = subLines[0].Replace("GOTO/", "").Split(',');
                string[] engageCells = subLines[1].Split(',');
                try
                {
                    // 读取坐标
                    point[X] = float.Parse(centerCells[0]);
                    point[Y] = float.Parse(centerCells[1]);
                    point[Z] = float.Parse(centerCells[2]);

                    // 读取接触点坐标
                    point[ENGAGE_X] = float.Parse(engageCells[0]);
                    point[ENGAGE_Y] = float.Parse(engageCells[1]);
                    point[ENGAGE_Z] = float.Parse(engageCells[2]);

                    if (centerCells.Length > 3)
                    {
                        point[I] = float.Parse(centerCells[3]);
                        point[J] = float.Parse(centerCells[4]);
                        point[K] = float.Parse(centerCells[5].Split(' ')[0]);
                    }
                }
                catch { }
                return true;
            }

            if (base.ParasePosition(line, ref point))
            {
                // 如果是不带接触点的GOTO/行，调用父类方法处理
                return true;
            }

            return false;
        }
        #endregion
    }
}
