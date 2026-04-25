using System.IO;
using System.Text;
using MachKit.Common;

namespace Convert.Methods.Converters
{
    /// <summary>
    /// 三坐标检测报告处理器
    /// </summary>
    public class ConvertCMMReport : AbstractConverter
    {
        public override string FileFilter => "三坐标测量报告 |*.txt;";

        public override void ConvertSingleToSingle(string oriFile, string tarFile)
        {
            using (var reader = new StreamReader(oriFile, ReadEncoding))
            {
                using (var writer = new StreamWriter(tarFile.RenameIfExist(), false, WriteEncoding))
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

        public override void ConvertSingleToMulti(string oriFile, string folder)
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
                        var tarFile = Path.Combine(folder, name + ".csv");
                        writer = new StreamWriter(tarFile.RenameIfExist(), false, WriteEncoding);
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

