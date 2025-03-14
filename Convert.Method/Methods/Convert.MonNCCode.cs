using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Convert.Methods
{
    public class ConvertMonNCCode : AbstractConverter
    {
        #region 单例模式
        private static ConvertMonNCCode instance = null;
        private static readonly object padlock = new object();
        public static ConvertMonNCCode Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertMonNCCode();
                    return instance;
                }
            }
        }

        private ConvertMonNCCode() { }
        #endregion

        public int Rapid { set; get; } = 6000;

        private readonly Regex NCCodeRegex = new Regex(@"(([A-Z])(-?\d+(\.\d+)?|\.\d+))");
        public override void SingleToSingle(string ncFile, string csvFile)
        {
            using (var reader = new System.IO.StreamReader(ncFile, ReadEncoding))
            {
                if (reader != null)
                {
                    using (var writer = new System.IO.StreamWriter(csvFile, false, WriteEncoding))
                    {
                        // 逐行读取文本文件
                        float x = 0, y = 0, z = 0, a = 0, c = 0, s = 0, f = 0;
                        writer.WriteLine("X,Y,Z,A,C,S,F");

                        string line = null;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Dictionary<char, float> map = new Dictionary<char, float>();
                            MatchCollection matches = NCCodeRegex.Matches(line);

                            foreach (Match match in matches)
                            {
                                if (match.Groups.Count > 2)
                                {
                                    char letter = match.Groups[2].Value[0];
                                    string number = match.Groups[3].Value;

                                    if (letter == 'G' && float.Parse(number) == 0)
                                        map['F'] = Rapid;
                                    else
                                        map[letter] = float.Parse(number);
                                }
                            }
                            float value;
                            if (map.TryGetValue('X', out value)) x = value;
                            if (map.TryGetValue('Y', out value)) y = value;
                            if (map.TryGetValue('Z', out value)) z = value;
                            if (map.TryGetValue('A', out value)) a = value;
                            if (map.TryGetValue('C', out value)) c = value;
                            if (map.TryGetValue('S', out value)) s = value;
                            if (map.TryGetValue('F', out value)) f = value;

                            writer.WriteLine($"{x},{y},{z},{a},{c},{s},{f}");
                        }
                        writer.Close();
                    }
                    reader.Close();
                }
            }
        }
    }
}
