using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace Process.Application.Models
{
    public class ProcessDWData : AbstractProcesser
    {
        #region 单例模式
        private static ProcessDWData? instance = null;
        private static readonly object padlock = new object();
        public static ProcessDWData Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new ProcessDWData();
                    return instance;
                }
            }
        }
        #endregion

        private ProcessDWData()
        {
            FileType = FileType.DWData;
            FileFilter = "Dewesoft采集数据|*.dxd;";

            CanOneToMulti = false;
            CanOneToOne = true;
        }

        protected override void OneToOne(string dataFile, string csvFile)
        {
            DWDataReader.DWDataReader.DxdToCsv(dataFile, csvFile);
        }

        public override void EachOneToEachOne(in IEnumerable<string> oriFiles, string tarFolder)
        {
           Parallel.ForEach(oriFiles, (file)=> 
           {
                if (file.ContainsChinese())
                {
                    MessageBox.Show(".dxd文件名及其路径中不能包含中文字符", "错误",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string tarFile = Path.Combine(tarFolder, Path.GetFileNameWithoutExtension(file) + ".csv");
                OneToOne(file, tarFile);
           });
        }
    }

    public static class StringExtensions
    {
        private static Regex regex = new Regex("[\u4e00-\u9fa5]");
        public  static bool  ContainsChinese(this string input) => regex.IsMatch(input);
    }
}
