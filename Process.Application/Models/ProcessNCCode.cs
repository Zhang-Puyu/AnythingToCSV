using System.IO;
using System.Text.RegularExpressions;

namespace Process.Application.Models
{
    internal struct Point : IEquatable<Point>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }
        public float F { get; set; }
        public float S { get; set; }

        public Point()
        {
            X = 0; Y = 0; Z = 0; A = 0; B = 0; C = 0; F = 0; S = 0;
        }
        public Point(in Point point)
        {
            X = point.X; Y = point.Y; Z = point.Z; 
            A = point.A; B = point.B; C = point.C; 
            F = point.F; S = point.S;
        }

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
                S = 0
            };
        }
        public static Point OrignalPostionPoint()
        {
            return new Point
            {
                X = 0, Y = 0, Z = 0,
                A = 0, B = 0, C = 0,
                F = 0, S = 0
            };
        }
        public static string Header()
        {
            return "x,y,z,a,b,c,f,s";
        }

        public override readonly string ToString()
        {
            return $"{X},{Y},{Z},{A},{B},{C},{F},{S}";
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Point point && Equals(point);
        }
        public override readonly int  GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, A, B, C, F, S);
        }
        public          readonly bool Equals(Point other)
        {
            return X == other.X && Y == other.Y && Z == other.Z &&
                   A == other.A && B == other.B && C == other.C &&
                   F == other.F && S == other.S;
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

    public class ProNCCode : AbstractProcesser
    {
        #region 单例模式
        private static ProNCCode? instance = null;
        private static readonly object padlock = new object();
        public static ProNCCode Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new ProNCCode();
                    return instance;
                }
            }
        }
        #endregion

        private ProNCCode() 
        {
            FileType = FileType.NCCode;
            FileFilter = "刀轨程序|*.nc;*.cls;*.mpf;";

            CanOneToMulti = false;
            CanOneToOne = true;
        }

        protected override void OneToOne(string dataFile, string csvFile)
        {
            string extension = Path.GetExtension(dataFile).ToLower();
            if (extension == ".nc" || extension == ".mpf")
                GCodeToCsv(dataFile, csvFile);
            if (extension == ".cls")
                AptToCsv(dataFile, csvFile);
        }

        public float RapidFeed { get; set; } = 6000;

        #region G代码.nc文件
        public void GCodeToCsv(in string ncFile, in string csvFile)
        {
            using StreamReader reader = new StreamReader(ncFile);
            using StreamWriter writer = new StreamWriter(csvFile);

            writer.WriteLine(Point.Header());

            string? line;
            Point lastPoint = new Point();
            while ((line = reader.ReadLine()) != null)
            {
                Point currPoint = new Point(lastPoint);
                ParaseGCodeLine(line, ref currPoint);
                if (currPoint != lastPoint)
                    writer.WriteLine(currPoint.ToString());
                lastPoint = new Point(currPoint);
            }

            reader.Close();
            writer.Close();
        }

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

        #region apt语言.cls文件
        public void AptToCsv(in string ncFile, in string csvFile)
        {
            using StreamReader reader = new StreamReader(ncFile);
            using StreamWriter writer = new StreamWriter(csvFile);

            writer.WriteLine(Point.Header());

            string? line;
            Point lastPoint = new Point();
            while ((line = reader.ReadLine()) != null)
            {
                Point currPoint = new Point(lastPoint);
                ParaseAptLine(line, ref currPoint);
                if (currPoint != lastPoint)
                    writer.WriteLine(currPoint.ToString());
                lastPoint = new Point(currPoint);
            }

            reader.Close();
            writer.Close();
        }

        private void ParaseAptLine(in string line, ref Point point)
        {
            if (line.StartsWith("RAPID"))
            {
                point.F = RapidFeed;
                return;
            }
            if (line.StartsWith("GOTO/"))
            {
                string[] s = line.Replace("GOTO/", "").Split(",");
                try
                {
                    point.X = float.Parse(s[0]);
                    point.Y = float.Parse(s[1]);
                    point.Z = float.Parse(s[2]);
                    point.A = float.Parse(s[3]);
                    point.C = float.Parse(s[5]);
                    point.B = float.Parse(s[4]);
                }
                catch { }
                return;
            }
            if (line.StartsWith("FEDRAT/MMPM"))
            {
                point.F = float.Parse(line.Split("/").Last());
                return;
            }
            if (line.StartsWith("FEDRAT/"))
            {
                point.F = float.Parse(line.Split("/").Last());
                return;
            }
        }
        #endregion

    }
}
