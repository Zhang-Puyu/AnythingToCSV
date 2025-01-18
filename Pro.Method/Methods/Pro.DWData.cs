using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Processor.Method.DWDataReader;

namespace Processor.Methods
{
    /// <summary>
    /// Dewesoft采集数据处理器
    /// </summary>
    public class ProDWData : AbstractProcessor
    {
        #region 单例模式
        private static ProDWData instance = null;
        private static readonly object padlock = new object();
        public static ProDWData Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null) 
                        instance = new ProDWData();
                    return instance;
                }
            }
        }

        private ProDWData() { }
        #endregion

        public override void SingleToSingle(string orifile, string csvfile)
        {
            DWDataReader.DxdToCsv(orifile, csvfile);
        }

        public override Task EachToEach(in IEnumerable<string> orifiles, string tarfolder)
        {
            Parallel.ForEach(orifiles, file =>
            {
                if (file.ContainsChinese())
                {
                    ErrorMsgEvent?.Invoke(".dxd文件名及其路径中不能包含中文字符");
                    return;
                }
                ProcessStartedEvent?.Invoke(file);
                string tarfile = Path.Combine(tarfolder, Path.GetFileNameWithoutExtension(file) + ".csv");
                SingleToSingle(file, tarfile);
                ProcessFinishedEvent?.Invoke(file);
            });
            return Task.CompletedTask;
        }
    }
}
