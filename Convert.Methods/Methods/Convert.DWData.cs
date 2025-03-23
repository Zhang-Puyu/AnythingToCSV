using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Convert.Methods
{
    /// <summary>
    /// Dewesoft采集数据处理器
    /// </summary>
    public class ConvertDWData : AbstractConverter
    {
        #region 单例模式
        private static ConvertDWData instance = null;
        private static readonly object padlock = new object();
        public static ConvertDWData Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null) 
                        instance = new ConvertDWData();
                    return instance;
                }
            }
        }

        private ConvertDWData() { }
        #endregion

        public override void SingleToSingle(string oriFile, string tarFile)
        {
            DWDataReader.DWDataReader.DxdToCsv(oriFile, tarFile);
        }

        public override Task EachToEach(in IEnumerable<string> oriFiles, string tarFolder)
        {
            Parallel.ForEach(oriFiles, file =>
            {
                if (file.ContainsChinese())
                {
                    ErrorMsgEvent?.Invoke(".dxd文件名及其路径中不能包含中文字符");
                    return;
                }
                StartedEvent?.Invoke(file);
                string tarFile = Path.Combine(tarFolder, Path.GetFileNameWithoutExtension(file) + ".csv");
                SingleToSingle(file, tarFile);
                FinishedEvent?.Invoke(file);
            });
            return Task.CompletedTask;
        }
    }
}
