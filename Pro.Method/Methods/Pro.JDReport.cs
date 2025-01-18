using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processor.Methods
{
    /// <summary>
    /// 精雕在机检测报告处理器
    /// </summary>
    public class ProJDReport : AbstractProcessor
    {
        #region 单例模式
        private static ProJDReport instance = null;
        private static readonly object padlock = new object();
        public static ProJDReport Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ProJDReport();
                    return instance;
                }
            }
        }

        private ProJDReport()
        {
            // 注册Nuget包System.Text.Encoding.CodePages中的编码到.NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        #endregion
        private readonly char[] trimChars = new char[] { '[', ']', '\n', '\r' };

        public override void SingleToSingle(string orifile, string csvfile)
        {
            string fileName = Path.GetFileNameWithoutExtension(orifile);

            using (StreamReader reader = new StreamReader(orifile, Encoding.GetEncoding("GB2312")))
            {
                string[] text = reader.ReadToEnd().Split('\n');
                var reportIndexes = text.Select((line, index) => new { line, index })
                    .Where(x => x.line.Contains("版本号:"))
                    .Select(x => x.index);
                using (StreamWriter writer = new StreamWriter(csvfile))
                {
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
                    InfoMsgEvent?.Invoke($"在{Path.GetFileName(orifile)}读取到了{reportIndexes.Count()}份报告\n" + string.Join("\n", reportNames));
                }
                reader.Close();
            }
        }

        public override void SingleToMulti(string orifile, string folder)
        {
            string fileName = Path.GetFileNameWithoutExtension(orifile);

            using (StreamReader reader = new StreamReader(orifile, Encoding.GetEncoding("GB2312")))
            {
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
                    using (StreamWriter writer = new StreamWriter(folder))
                    {
                        string[] head = text[reportIndex + 11].Trim(trimChars).Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
                        writer.WriteLine(string.Join(",", head));

                        int lineIndex = reportIndex + 11;
                        while (!string.IsNullOrWhiteSpace(text[++lineIndex]))
                        {
                            string[] cells = text[lineIndex].Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
                            writer.WriteLine(string.Join(",", cells));
                        }
                        writer.Close();
                    }
                });
                reader.Close();
                InfoMsgEvent?.Invoke($"在{Path.GetFileName(orifile)}读取到了{reportIndexes.Count()}份报告\n" + string.Join("\n", reportNames));
            }
        }
    }
}
