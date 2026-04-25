using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MachKit.Common;

namespace Convert.Methods.Converters
{
    /// <summary>
    /// Dewesoft采集数据处理器
    /// </summary>
    public class ConvertDWData : AbstractConverter
    {
        public override string FileFilter => "Dewesoft采集数据 |*.dxd;";

        public override void ConvertSingleToSingle(string oriFile, string tarFile)
        {
            DWDataReader.DWDataReader.DXDToCSV(oriFile, tarFile.RenameIfExist());
        }

        public override Task ConvertEachToSingle(in IEnumerable<string> oriFiles, string tarFolder)
        {
            foreach (string file in oriFiles)
            {
                if (file.ContainsChinese())
                {
                    ErrorMsgEvent?.Invoke(".dxd文件名及其路径中不能包含中文字符");
                    return Task.CompletedTask;
                }
            }
            return base.ConvertEachToSingle(oriFiles, tarFolder);
        }
    }
}
