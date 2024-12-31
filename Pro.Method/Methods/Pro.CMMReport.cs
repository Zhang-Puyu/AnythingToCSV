using System.IO;
using System.Text;

namespace Processor.Methods
{
    /// <summary>
    /// 三坐标检测报告处理器
    /// </summary>
    public sealed class ProCMMReport : AbstractProcessor
    {
        #region 单例模式
        private static ProCMMReport instance = null;
        private static readonly object padlock = new object();
        public static ProCMMReport Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ProCMMReport();
                    return instance;
                }
            }
        }

        private ProCMMReport() { }
        #endregion

        public override void SingleToSingle(string orifile, string csvfile)
        {
            using (var reader = new StreamReader(orifile, Encoding.GetEncoding("GBK")))
            {
                using (var writer = new StreamWriter(csvfile))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Trim().StartsWith("触测/矢量"))
                        {
                            // 取第一个逗号和最后一个逗号之间的字符串
                            string data = line.Substring(line.IndexOf(',') + 1, line.LastIndexOf(',') - line.IndexOf(',') - 1);
                            writer?.WriteLine(data);
                        }
                    }
                    writer.Close();
                }
                reader.Close();
            }
        }

        public override void SingleToMulti(string orifile, string folder)
        {
            using (StreamReader reader = new StreamReader(orifile, Encoding.GetEncoding("GBK")))
            {
                StreamWriter writer = null;
                string       line   = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("扫描"))
                    {
                        writer?.Close();
                        string name = line.Substring(0, line.IndexOf(' '));
                        writer = new StreamWriter(folder + "\\" + name + ".csv");
                    }
                    if (line.Trim().StartsWith("触测/矢量"))
                    {
                        // 取第一个逗号和最后一个逗号之间的字符串
                        writer?.WriteLine(line.Substring(line.IndexOf(',') + 1, line.LastIndexOf(',') - line.IndexOf(',') - 1));
                    }
                }
                writer?.Close();
                writer?.Dispose();

                reader.Close();
            }
        }
    }
}
