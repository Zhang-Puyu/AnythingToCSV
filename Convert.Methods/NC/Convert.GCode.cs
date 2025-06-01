using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Convert.Methods.NC
{
    public class ConvertGCode : ConvertNC
    {
        #region 单例模式
        private static ConvertGCode instance = null;
        private static readonly object padlock = new object();
        public static ConvertGCode Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertGCode();
                    return instance;
                }
            }
        }

        private ConvertGCode() { }
        #endregion

        protected override string Header => "x,y,z,a,b,c,f,s";

        protected override int Dimension => 8;

        #region G代码解析

        public float RapidFeed { get; set; } = -1;

        private readonly int X = 0, Y = 1, Z = 2, A = 3, B = 4, C = 5, F = 6, S = 7;

        private readonly Regex GCodeRegex = new Regex(@"(([I-Z])(-?\d+(\.\d+)?|\.\d+))");

        protected override void ParaseLine(in string line, ref float[] point)
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

            if (map.ContainsKey('X')) point[X] = map['X'];
            if (map.ContainsKey('Y')) point[Y] = map['Y'];
            if (map.ContainsKey('Z')) point[Z] = map['Z'];
            if (map.ContainsKey('A')) point[A] = map['A'];
            if (map.ContainsKey('B')) point[B] = map['B'];
            if (map.ContainsKey('C')) point[C] = map['C'];
            if (map.ContainsKey('F')) point[F] = map['F'];
            if (map.ContainsKey('S')) point[S] = map['S'];

            //if (map.StartsWithKey('T')) toolNo = (uint)map['T'];
            //if (map.StartsWithKey('H')) toolOffset = (uint)map['H'];
        }
        #endregion
    }
}
