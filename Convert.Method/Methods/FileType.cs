using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Convert.Methods
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// .slk矩阵文件
        /// </summary>
        Slk,
        /// <summary>
        /// 精雕在机测量报告
        /// </summary>
        JDReport,
        /// <summary>
        /// 三坐标测量报告
        /// </summary>
        CMMReport,
        /// <summary>
        /// Dewesoft采集数据
        /// </summary>
        DWData,
        /// <summary>
        /// scanCONTROL三维扫描数据
        /// </summary>
        Scan3DData,
        /// <summary>
        /// 在线监测G代码
        /// </summary>
        MonNCCode,
        /// <summary>
        /// 刀轨程序(.nc/.cls/.mpf)
        /// </summary>
        NCCode,
        /// <summary>
        /// MaxPac生成的cld代码
        /// </summary>
        MaxCode,
        /// <summary>
        /// 不支持的文件类型
        /// </summary>
        Unsurpport,
    }

    public static class FileTypeExtensions
    {
        #region 字典
        private static readonly Dictionary<FileType, string> FileFilterDic
            = new Dictionary<FileType, string>
            {
                { FileType.Slk,        "SLK矩阵文件 |*.slk;" },
                { FileType.JDReport,   "精雕在机测量报告 |*_Export.txt;" },
                { FileType.CMMReport,  "三坐标测量报告 |*.txt;" },
                { FileType.DWData,     "Dewesoft采集数据 |*.dxd;" },
                { FileType.MonNCCode,  "在线监测G代码 |*.mon;" },
                { FileType.NCCode,     "刀轨程序 .nc/.cls/.mpf |*.nc;*.cls;*.mpf;" },
                { FileType.MaxCode,    "MaxPac生成的带前倾角刀轨 |*.cls;" },
                { FileType.Scan3DData, "scanCONTROL三维扫描数据 |*.csv;" },
            };

        private static readonly Dictionary<FileType, AbstractConverter> ProcesserDic
            = new Dictionary<FileType, AbstractConverter>
            {
                { FileType.Slk,        ConvertSlk.Instance },
                { FileType.JDReport,   ConvertJDReport.Instance },
                { FileType.CMMReport,  ConvertCMMReport.Instance },
                { FileType.DWData,     ConvertDWData.Instance },
                { FileType.MonNCCode,  ConvertMonNCCode.Instance },
                { FileType.NCCode,     ConvertNCCode.Instance },
                { FileType.MaxCode,    ConvertMaxCode.Instance },
                { FileType.Scan3DData, ConvertScan3DData.Instance },
            };
        internal static readonly Dictionary<string, FileType> FileExtensionDic
            = new Dictionary<string, FileType>
            {
                { ".slk",        FileType.Slk },
                { "_export.txt", FileType.JDReport },
                { ".txt",        FileType.CMMReport },
                { ".dxd",        FileType.DWData },
                { ".mon",        FileType.MonNCCode },
                { ".nc",         FileType.NCCode },
                //{ ".cls",        FileType.NCCode },
                { ".cls",        FileType.MaxCode },
                { ".mpf",        FileType.NCCode },
                { ".csv",        FileType.Scan3DData },
            };
        #endregion

        public static string[] SurpportedFileExtensions
            => FileExtensionDic.Keys.ToArray();

        /// <summary>
        /// 文件类型对应的文件过滤器
        /// </summary>
        public static string GetFileFilter(this FileType fileType)
        {
            return FileFilterDic.TryGetValue(fileType, out var filter) ? filter : null;
        }
        /// <summary>
        /// 各文件类型的描述
        /// </summary>
        public static string GetDescription(this FileType fileType)
        {
            return fileType.GetFileFilter().Split('|').First();
        }
        /// <summary>
        /// 获取文件类型对应的文件后缀
        /// </summary>
        public static string[] GetFileExtensions(this FileType fileType)
        {
            // 遍历FileExtensionDic
            return FileExtensionDic.Where(kv => kv.Value == fileType).Select(kv => kv.Key).ToArray();

        }
        /// <summary>
        /// 根据文件类型选择对应的处理器
        /// </summary>
        public static AbstractConverter ChooseConverter(this FileType fileType)
        {
            return ProcesserDic.TryGetValue(fileType, out var processer) ? processer : null;
        }

        #region 文件对话框
        /// <summary>
        /// 打开文件选择对话框
        /// </summary>
        public static OpenFileDialog OpenFileDialog(this FileType fileType) =>
             new OpenFileDialog()
             {
                 Title = "请选择要添加的数据文件",
                 Filter = fileType.GetFileFilter(),
                 Multiselect = !fileType.ChooseConverter().CanSingleToMulti,
             };
        /// <summary>
        /// 打开文件保存对话框
        /// </summary>
        public static SaveFileDialog SaveFileDialog(this FileType fileType) =>
            new SaveFileDialog()
            {
                Title = "请选择要保存的数据文件",
                Filter = fileType.GetFileFilter(),
                CheckFileExists = true,
            };
        #endregion
    }

    public static class StringExtensions
    {
        /// <summary>
        /// 判断字符串中是否包含中文
        /// </summary>
        public static bool ContainsChinese(this string str)
        {
            foreach (char c in str)
                if (c >= 0x4e00 && c <= 0x9fbb)
                    return true;
            return false;
        }

        /// <summary>
        /// 获取根据文件后缀获取文件类型
        /// </summary>
        /// <param name="file">文件名</param>
        /// <returns>文件类型</returns>
        public static FileType GetFileType(this string file)
        {
            // 精雕在机测量报表实际文件后缀为.txt，和三坐标测量结果相同
            if(file.ToLower().EndsWith("_export.txt")) 
                return FileType.JDReport;

            return FileTypeExtensions.FileExtensionDic.TryGetValue(Path.GetExtension(file).ToLower(), out var fileType) ? 
                fileType :
                FileType.Unsurpport;
        }
    }
}