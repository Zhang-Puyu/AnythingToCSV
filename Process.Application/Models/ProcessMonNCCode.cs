using System.Text.RegularExpressions;

namespace Process.Application.Models
{
    public class ProMonNCCode : AbstractProcesser
    {
        #region 单例模式
        private static ProMonNCCode? instance = null;
        private static readonly object padlock = new object();
        public static ProMonNCCode Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new ProMonNCCode();
                    return instance;
                }
            }
        }
        #endregion

        private ProMonNCCode() 
        {
            FileType = FileType.MonNCCode;
            FileFilter = "在线监测NC代码|*.mon";

            CanOneToMulti = false;
            CanOneToOne = true;
        }

        public int Rapid { set; get; } = 6000;

        private readonly Regex NCCodeRegex = new Regex(@"(([A-Z])(-?\d+(\.\d+)?|\.\d+))");
        protected override void OneToOne(string ncFile, string csvFile)
        {
            // 逐行读取文本文件
            string? line = string.Empty;
            float x = 0, y = 0, z = 0, a = 0, c = 0, s = 0, f = 0;

            using var reader = new System.IO.StreamReader(ncFile);
            if (reader == null) return;

            using var writer = new System.IO.StreamWriter(csvFile);
            writer.WriteLine("X,Y,Z,A,C,S,F");

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
        }
    }
}
