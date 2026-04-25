using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using MachKit.Common;

namespace Convert.Methods.Converters
{
    public class ConvertJDReport : AbstractConverter
    {
        public override string FileFilter => "精雕在机测量报告 |*_Export.txt;";

        private readonly char[] TrimChars = new char[] { '[', ']', '\n', '\r' };
        public override void ConvertSingleToSingle(string oriFile, string csvFile)
        {
            using (StreamReader reader = new StreamReader(oriFile, ReadEncoding))
            {
                string[] text = reader.ReadToEnd().Split('\n');
                var reportIndexes = text.Select((line, index) => new { line, index })
                    .Where(x => x.line.Contains("版本号:"))
                    .Select(x => x.index);
                using (StreamWriter writer = new StreamWriter(csvFile.RenameIfExist(), false, WriteEncoding))
                {
                    string[] head = text[reportIndexes.First() + 11].Trim(TrimChars).Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
                    writer.WriteLine(string.Join(",", head));

                    List<string> reportNames = new List<string>();

                    foreach (int reportIndex in reportIndexes)
                    {
                        // string verion = text[reportIndex].Split(' ')[1].TrimEnd(TrimChars);
                        // string time   = text[reportIndex + 1].Split(' ')[1].TrimEnd(TrimChars);
                        string reportName = text[reportIndex + 2].Split(' ')[1].TrimEnd(TrimChars);
                        reportNames.Add(reportName);

                        int lineIndex = reportIndex + 11;
                        while (!string.IsNullOrWhiteSpace(text[++lineIndex]))
                        {
                            string[] cells = text[lineIndex].Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
                            writer.WriteLine(string.Join(",", cells));
                        }
                    }
                    writer.Close();
                    InfoMsgEvent?.Invoke($"在{Path.GetFileName(oriFile)}读取到了{reportIndexes.Count()}份报告\n" +
                        string.Join("\n", reportNames));
                }
                reader.Close();
            }
        }

        public override void ConvertSingleToMulti(string oriFile, string folder)
        {
            using (StreamReader reader = new StreamReader(oriFile, ReadEncoding))
            {
                string[] text = reader.ReadToEnd().Split('\n');
                var reportIndexes = text.Select((line, index) => new { line, index })
                    .Where(x => x.line.Contains("版本号:"))
                    .Select(x => x.index);
                List<string> reportNames = new List<string>();

                Parallel.ForEach(reportIndexes, reportIndex =>
                {
                    // string verion = text[reportIndex].Split(' ')[1].TrimEnd(TrimChars);
                    // string time   = text[reportIndex + 1].Split(' ')[1].TrimEnd(TrimChars);
                    string reportName = text[reportIndex + 2].Split(' ')[1].TrimEnd(TrimChars);
                    reportNames.Add(reportName);

                    string tarFile = Path.Combine(folder, reportName + ".csv");
                    using (StreamWriter writer = new StreamWriter(tarFile.RenameIfExist(), false, WriteEncoding))
                    {
                        string[] head = text[reportIndex + 11].Trim(TrimChars).Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
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
                InfoMsgEvent?.Invoke($"在{Path.GetFileName(oriFile)}读取到了{reportIndexes.Count()}份报告\n" + 
                    string.Join("\n", reportNames));
            }
        }
    }
}
