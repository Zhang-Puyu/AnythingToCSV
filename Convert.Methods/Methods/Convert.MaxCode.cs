using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Convert.Methods
{
    /// <summary>
    /// 带前倾角刀位点
    /// </summary>
    internal struct MaxPoint
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }
        /// <summary>
        /// 进给
        /// </summary>
        public float F { get; set; }
        /// <summary>
        /// 转速
        /// </summary>
        public float S { get; set; }

        /// <summary>
        /// 前倾角
        /// </summary>
        public float DRAG { get; set; }
        /// <summary>
        /// 无效的前倾角
        /// </summary>
        public static readonly float InvalidDrag = 181;

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        public MaxPoint(in MaxPoint point)
        {
            X = point.X; Y = point.Y; Z = point.Z;
            A = point.A; B = point.B; C = point.C;
            F = point.F; S = point.S;
            DRAG = point.DRAG;
        }
        /// <summary>
        /// 随机生成一个刀位点，进给和转速为0
        /// </summary>
        public static MaxPoint RandomPostionPoint()
        {
            Random random = new Random();
            return new MaxPoint
            {
                X = (float)random.NextDouble() * 550 - 100,
                Y = (float)random.NextDouble() * 550 - 100,
                Z = (float)random.NextDouble() * 550 - 100,
                A = (float)random.NextDouble() * 550 - 100,
                B = (float)random.NextDouble() * 550 - 100,
                C = (float)random.NextDouble() * 550 - 100,
                F = 0,
                S = 0,
                DRAG = InvalidDrag,
            };
        }
        /// <summary>
        /// 原点刀位点，进给和转速为0
        /// </summary>
        public static MaxPoint OrignalPostionPoint()
        {
            return new MaxPoint
            {
                X = 0, Y = 0, Z = 0,
                A = 0, B = 0, C = 0,
                F = 0, S = 0,
                DRAG = InvalidDrag
            };
        }

        public override string ToString()
        {
            return $"{X},{Y},{Z},{A},{B},{C},{F},{S},{DRAG}";
        }

        public static bool operator ==(in MaxPoint left, in MaxPoint right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z &&
                    left.A == right.A && left.B == right.B && left.C == right.C &&
                    left.DRAG == right.DRAG;
        }
        public static bool operator !=(in MaxPoint left, in MaxPoint right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// 处理MaxPac生成的cls刀轨文件
    /// </summary>
    public class ConvertMaxCode : AbstractConverter
    {
        #region 单例模式
        private static ConvertMaxCode instance = null;
        private static readonly object padlock = new object();
        public static ConvertMaxCode Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertMaxCode();
                    return instance;
                }
            }
        }

        private ConvertMaxCode() { }
        #endregion

        /// <summary>
        /// 快速进给速度
        /// </summary>
        public float RapidFeed { get; set; } = 6000;

        public override void SingleToSingle(string oriFile, string tarFile)
        {
            using (StreamReader reader = new StreamReader(oriFile, ReadEncoding))
            {
                using (StreamWriter writer = new StreamWriter(tarFile, false, WriteEncoding))
                {
                    writer.WriteLine("x,y,z,i,j,k,f,s,drag");

                    string line = null;
                    MaxPoint lastPoint = new MaxPoint();
                    while ((line = reader.ReadLine()) != null)
                    {
                        MaxPoint currPoint = new MaxPoint(lastPoint);
                        ParaseAptLine(line, ref currPoint);
                        if (currPoint != lastPoint)
                            writer.WriteLine(currPoint.ToString());
                        lastPoint = new MaxPoint(currPoint);
                    }
                    writer.Close();
                }
                reader.Close();
            }
        }

        private readonly string dragPattern = @"DRAG=\s*(-?\d+(\.\d+)?)";
        private Match dragMatch;
        private void ParaseAptLine(in string line, ref MaxPoint point)
        {
            if (line.StartsWith("RAPID"))
            {
                point.F = RapidFeed;
                return;
            }
            // GOTO/108.4843,-280.0770,-19.5434,0.380709,-0.921682,0.074586 $$PT2 DRAG=-90.0
            if (line.StartsWith("GOTO/"))
            {
                string[] s = line.Replace("GOTO/", "").Split(',');
                try
                {
                    // 读取坐标
                    point.X = float.Parse(s[0]);
                    point.Y = float.Parse(s[1]);
                    point.Z = float.Parse(s[2]);
                    point.A = float.Parse(s[3]);
                    point.B = float.Parse(s[4]);
                    point.C = float.Parse(s[5].Split(' ')[0]);

                    // 读取DRAG
                    dragMatch = Regex.Match(line, dragPattern);
                    point.DRAG = dragMatch.Success ? float.Parse(dragMatch.Groups[1].Value) : MaxPoint.InvalidDrag;
                }
                catch { }
                return;
            }
            if (line.StartsWith("FEDRAT/MMPM"))
            {
                point.F = float.Parse(line.Split('/').Last());
                return;
            }
            // FEDRAT/6000.0000
            if (line.StartsWith("FEDRAT/"))
            {
                point.F = float.Parse(line.Split('/').Last());
                return;
            }
            // SPINDL/ 2300, CLW
            if (line.StartsWith("SPINDL/"))
            {
                point.S = float.Parse(Regex.Replace(line.Split('/').Last(), @",.*", "").Trim());
                return;
            }
        }
    }
}
