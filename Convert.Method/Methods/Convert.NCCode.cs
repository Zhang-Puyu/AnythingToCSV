using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Convert.Methods
{
    /// <summary>
    /// 刀位点
    /// </summary>
    internal struct Point
    {
        #region 字段
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
        #endregion

        #region 初始化
        public Point(in Point point)
        {
            X = point.X; Y = point.Y; Z = point.Z; 
            A = point.A; B = point.B; C = point.C; 
            F = point.F; S = point.S; 
        }

        /// <summary>
        /// 随机生成一个刀位点，进给和转速为0
        /// </summary>
        public static Point RandomPostionPoint()
        {
            Random random = new Random();
            return new Point
            {
                X = (float)random.NextDouble() * 550 - 100,
                Y = (float)random.NextDouble() * 550 - 100,
                Z = (float)random.NextDouble() * 550 - 100,
                A = (float)random.NextDouble() * 550 - 100,
                B = (float)random.NextDouble() * 550 - 100,
                C = (float)random.NextDouble() * 550 - 100,
                F = 0,
                S = 0,
            };
        }
        /// <summary>
        /// 原点刀位点，进给和转速为0
        /// </summary>
        public static Point OrignalPostionPoint()
        {
            return new Point
            {
                X = 0, Y = 0, Z = 0,
                A = 0, B = 0, C = 0,
                F = 0, S = 0,
            };
        }
        #endregion

        public override string ToString()
        {
            // 遍历结构体的所有字段
            // return string.Join(",", this.GetType().GetFields().Select(f => f.GetValue(this)));
            return $"{X},{Y},{Z},{A},{B},{C},{F},{S}";
        }

        public static bool operator ==(in Point left, in Point right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z &&
                    left.A == right.A && left.B == right.B && left.C == right.C;
        }
        public static bool operator !=(in Point left, in Point right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// 解析一行代码委托
    /// </summary>
    /// <param name="line">一行代码</param>
    /// <param name="point">解析得到的刀位点</param>
    internal delegate void ParaseLineDelegate(in string line, ref Point point);

    public class ConvertNCCode : AbstractConverter
    {
        #region 单例模式
        private static ConvertNCCode instance = null;
        private static readonly object padlock = new object();
        public static ConvertNCCode Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertNCCode();
                    return instance;
                }
            }
        }

        private ConvertNCCode() {}
        #endregion

        /// <summary>
        /// 快速进给速度
        /// </summary>
        public float RapidFeed { get; set; } = 6000;

        /// <summary>
        /// 解析一行代码委托
        /// </summary>
        private ParaseLineDelegate ParaseLine;

        public override void SingleToSingle(string oriFile, string tarFile)
        {
            string extension = Path.GetExtension(oriFile).ToLower();
            string head      = string.Empty;
            if (extension == ".nc" || extension == ".mpf")
            {
                head = "x,y,z,a,b,c,f,s";
                ParaseLine = ParaseGCodeLine;
            }
            if (extension == ".cls")
            {
                head = "x,y,z,i,j,k,f,s";
                ParaseLine = ParaseAptLine;
            }
            else
            {
                ErrorMsgEvent?.Invoke("不支持的文件格式");
                return;
            }

            using (StreamReader reader = new StreamReader(oriFile, ReadEncoding))
            {
                using (StreamWriter writer = new StreamWriter(tarFile, false, WriteEncoding))
                {
                    writer.WriteLine(head);

                    string line = null;
                    Point lastPoint = new Point();
                    while ((line = reader.ReadLine()) != null)
                    {
                        Point currPoint = new Point(lastPoint);
                        ParaseLine(line, ref currPoint);
                        if (currPoint != lastPoint)
                            writer.WriteLine(currPoint.ToString());
                        lastPoint = new Point(currPoint);
                    }
                    writer.Close();
                }
                reader.Close();
            }
        }

        #region G代码(.nc/.mpf)解析
        private readonly Regex GCodeRegex = new Regex(@"(([A-Z])(-?\d+(\.\d+)?|\.\d+))");
        internal void ParaseGCodeLine(in string line, ref Point point)
        {
            Dictionary<char, float> map = new Dictionary<char, float>();
            MatchCollection matches = GCodeRegex.Matches(line);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 2)
                {
                    char letter = match.Groups[2].Value[0];
                    string number = match.Groups[3].Value;

                    if (letter == 'G' && float.Parse(number) == 0)
                        map['F'] = RapidFeed;
                    else
                        map[letter] = float.Parse(number);
                }
            }

            if (map.ContainsKey('X')) point.X = map['X'];
            if (map.ContainsKey('A')) point.A = map['A'];
            if (map.ContainsKey('Y')) point.Y = map['Y'];
            if (map.ContainsKey('B')) point.B = map['B'];
            if (map.ContainsKey('Z')) point.Z = map['Z'];
            if (map.ContainsKey('C')) point.C = map['C'];
            if (map.ContainsKey('F')) point.F = map['F'];
            if (map.ContainsKey('S')) point.S = map['S'];

            //if (map.StartsWithKey('T')) toolNo = (uint)map['T'];
            //if (map.StartsWithKey('H')) toolOffset = (uint)map['H'];
        }
        #endregion

        #region apt语言(.cls)解析
        private void ParaseAptLine(in string line, ref Point point)
        {
            if (line.StartsWith("RAPID"))
            {
                point.F = RapidFeed;
                return;
            }
            // GOTO/108.4843,-280.0770,-19.5434,0.380709,-0.921682,0.074586 $$PT2
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
        #endregion
    }
}
