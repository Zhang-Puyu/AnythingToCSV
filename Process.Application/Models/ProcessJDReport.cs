using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Process.Application.Models
{
    public class ProJDReport : AbstractProcesser
    {
        #region 单例模式
        private static ProJDReport? instance = null;
        private static readonly object padlock = new object();
        public static ProJDReport Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new ProJDReport();
                    return instance;
                }
            }
        }
        #endregion

        private ProJDReport() 
        {
            FileType = FileType.JDReport;
            FileFilter = "在机检测报告|*_Export.txt";

            CanOneToMulti = true;
            CanOneToOne = true;

            // 注册Nuget包System.Text.Encoding.CodePages中的编码到.NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private readonly char[] trimChars = new char[] { '[', ']', '\n', '\r' };

        protected override void OneToOne(string dataFile, string csvFile)
        {
            string fileName = Path.GetFileNameWithoutExtension(dataFile);

            using StreamReader reader = new StreamReader(dataFile, Encoding.GetEncoding("GB2312"));
            string[] text = reader.ReadToEnd().Split('\n');
            var reportIndexes = text.Select((line, index) => new { line, index })
                .Where(x => x.line.Contains("版本号:"))
                .Select(x => x.index);

            using StreamWriter writer = new StreamWriter(csvFile);

            string[] head = text[reportIndexes.First() + 11].Trim(trimChars).Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
            writer.WriteLine(string.Join(",", head));

            List<string> reportNames = new List<string>();

            foreach (int reportIndex in reportIndexes)
            {
                // string verion = text[reportIndex].Split(' ')[1].TrimEnd(trimChars);
                // string time   = text[reportIndex + 1].Split(' ')[1].TrimEnd(trimChars);
                string reportName = text[reportIndex + 2].Split(' ')[1].TrimEnd(trimChars);
                reportNames.Add(reportName);

                int lineIndex = reportIndex + 11;
                while (!string.IsNullOrWhiteSpace(text[++lineIndex]))
                {
                    string[] cells = text[lineIndex].Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
                    writer.WriteLine(string.Join(",", cells));
                }
            }

            writer.Close();
            reader.Close();

            MessageBox.Show($"读取到了{reportIndexes.Count()}份报告\n" + string.Join('\n', reportNames));
        }

        protected override void OneToMulti(string dataFile, string folder)
        {
            string fileName = Path.GetFileNameWithoutExtension(dataFile);

            using StreamReader reader = new StreamReader(dataFile, Encoding.GetEncoding("GB2312"));
            string[] text = reader.ReadToEnd().Split('\n');
            var reportIndexes = text.Select((line, index) => new { line, index })
                .Where(x => x.line.Contains("版本号:"))
                .Select(x => x.index);

            List<string> reportNames = new List<string>();

            Parallel.ForEach(reportIndexes, reportIndex =>
            {
                // string verion = text[reportIndex].Split(' ')[1].TrimEnd(trimChars);
                // string time   = text[reportIndex + 1].Split(' ')[1].TrimEnd(trimChars);
                string reportName = text[reportIndex + 2].Split(' ')[1].TrimEnd(trimChars);
                reportNames.Add(reportName);

                string tarFile = Path.Combine(folder, reportName + ".csv");
                using StreamWriter writer = new StreamWriter(folder);

                string[] head = text[reportIndex + 11].Trim(trimChars).Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
                writer.WriteLine(string.Join(",", head));

                int lineIndex = reportIndex + 11;
                while (!string.IsNullOrWhiteSpace(text[++lineIndex]))
                {
                    string[] cells = text[lineIndex].Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
                    writer.WriteLine(string.Join(",", cells));
                }

                writer.Close();
            });

            reader.Close();

            MessageBox.Show($"读取到了{reportIndexes.Count()}份报告\n" + string.Join('\n', reportNames));
        }
    }
}
