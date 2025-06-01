using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Convert.Methods.NC
{
    public class ConvertClsStd : ConvertNC
    {
        #region 单例模式
        private static ConvertClsStd instance = null;
        protected static readonly object padlock = new object();
        public static ConvertClsStd Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertClsStd();
                    return instance;
                }
            }
        }

        protected ConvertClsStd() { }
        #endregion

        protected override string Header => "x,y,z,i,j,k,f,s";

        protected override int Dimension => 8;

        #region apt语言解析

        protected bool  RapidFlag = false;
        public    float RapidFeed { get; set; } = -1;

        protected float FeedRate = 0;
        protected float SpindleSpeed = 0;

        protected readonly int X = 0, Y = 1, Z = 2, 
            A = 3, B = 4, C = 5,
            I = 3, J = 4, K = 5, 
            F = 6, S = 7;

        protected override void ParaseLine(in string line, ref float[] point)
        {
            if (ParasePosition(line, ref point)) return;
            if (ParaseFeedRate(line, point))     return;
            if (ParaseSpindlSpeed(line))         return;
        }

        protected virtual bool ParasePosition(in string line, ref float[] point)
        {
            // NREC: GOTO/108.4843,-280.0770,-19.5434,0.380709,-0.921682,0.074586 $$PT2
            // UG: GOTO/97.8011,83.8859,-11.4651
            if (line.StartsWith("GOTO/"))
            {
                point[S] = SpindleSpeed;
                point[F] = RapidFlag ? RapidFeed : FeedRate;

                RapidFlag = RapidFlag ? false : RapidFlag;

                string[] cells = line.Replace("GOTO/", "").Split(',');
                try
                {
                    // 读取坐标
                    point[X] = float.Parse(cells[0]);
                    point[Y] = float.Parse(cells[1]);
                    point[Z] = float.Parse(cells[2]);
                    if (cells.Length > 3)
                    {
                        point[I] = float.Parse(cells[3]);
                        point[J] = float.Parse(cells[4]);
                        point[K] = float.Parse(cells[5].Split(' ')[0]);
                    }
                }
                catch { }
                return true;
            }
            return false;
        }

        protected virtual bool ParaseFeedRate(in string line, in float[] point)
        {
            // FEDRAT/MMPM,250.0000
            if (line.StartsWith("FEDRAT/MMPM"))
            {
                FeedRate = float.Parse(line.Split(',').Last());
                return true;
            }
            // FEDRAT/6000.0000
            if (line.StartsWith("FEDRAT/"))
            {
                FeedRate = float.Parse(line.Split('/').Last());
                return true;
            }
            if (line.StartsWith("RAPID"))
            {
                RapidFlag = true;
                FeedRate = point[F];
                return true;
            }
            return false;
        }

        protected virtual bool ParaseSpindlSpeed(in string line)
        {
            // SPINDL/ 2300, CLW
            if (line.StartsWith("SPINDL/"))
            {
                SpindleSpeed = float.Parse(Regex.Replace(line.Split('/').Last(), @",.*", "").Trim());
                return true;
            }
            return false;
        }
        #endregion
    }
}
