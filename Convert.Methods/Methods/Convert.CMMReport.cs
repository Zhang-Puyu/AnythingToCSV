using System.IO;
using System.Text;

namespace Convert.Methods
{
    /// <summary>
    /// 三坐标检测报告处理器
    /// </summary>
    public class ConvertCMMReport : AbstractConverter
    {
        #region 单例模式
        private static ConvertCMMReport instance = null;
        private static readonly object padlock = new object();
        public static ConvertCMMReport Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertCMMReport();
                    return instance;
                }
            }
        }
        private ConvertCMMReport() { }

        #endregion

        public override void SingleToSingle(string oriFile, string tarFile)
        {
            using (var reader = new StreamReader(oriFile, ReadEncoding))
            {
                using (var writer = new StreamWriter(tarFile, false, WriteEncoding))
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


        public override void SingleToMulti(string oriFile, string folder)
        {
            using (StreamReader reader = new StreamReader(oriFile, ReadEncoding))
            {
                StreamWriter writer = null;
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("扫描"))
                    {
                        writer?.Close();
                        string name = line.Substring(0, line.IndexOf(' '));
                        var tarFile = Path.Combine(folder, name + ".csv").RenameIfFileExists();
                        writer = new StreamWriter(tarFile, false, WriteEncoding);
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

