using System.IO;
using System.Text;

namespace Process.Application.Models
{
    public sealed class ProCMMReport : AbstractProcesser
    {
        #region 单例模式
        private static ProCMMReport? instance = null;
        private static readonly object padlock = new object();
        public static ProCMMReport Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new ProCMMReport();
                    return instance;
                }
            }
        }
        #endregion

        private ProCMMReport()
        {
            FileType = FileType.CMMReport;
            FileFilter = "三坐标检测报告|*.txt;";

            CanOneToMulti = true;
            CanOneToOne = true;
        }

        protected override void OneToOne(string dataFile, string csvFile) 
        {
            string? line;
            StreamWriter writer = new StreamWriter(csvFile);
            using var reader = new StreamReader(dataFile, Encoding.GetEncoding("GBK"));

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim().StartsWith("触测/矢量"))
                {
                    // 取第一个逗号和最后一个逗号之间的字符串
                    string data = line.Substring(line.IndexOf(',') + 1, line.LastIndexOf(',') - line.IndexOf(',') - 1);
                    writer?.WriteLine(data);
                }
            }
        }

        protected override void OneToMulti(string dataFile, string folder)
        {
            string? line;
            StreamWriter? writer = null;
            using var reader = new StreamReader(dataFile, Encoding.GetEncoding("GBK"));

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("扫描"))
                {
                    writer?.Close();
                    string name = line[..line.IndexOf(' ')];
                    writer = new StreamWriter(folder + "\\" + name + ".csv");
                }
                if (line.Trim().StartsWith("触测/矢量"))
                {
                    // 取第一个逗号和最后一个逗号之间的字符串
                    writer?.WriteLine(line.Substring(line.IndexOf(',') + 1, line.LastIndexOf(',') - line.IndexOf(',') - 1));
                }
            }
        }
    }
}
