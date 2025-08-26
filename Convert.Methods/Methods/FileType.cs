using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Convert.Methods.NC;

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
        SLK,
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
        /// 刀轨程序G代码
        /// </summary>
        GCode,
        /// <summary>
        /// 刀轨程序Apt代码
        /// </summary>
        APT,
        /// <summary>
        /// MaxPac生成带前倾角cls代码
        /// </summary>
        APTWithDrag,
        /// <summary>
        /// UG导出带接触信息的cls刀轨
        /// </summary>
        APTWithContact,
        /// <summary>
        /// 合并csv文件
        /// </summary>
        MergeCsv
    }

    /// <summary>
    /// 文件类型-筛选器-转换器对应关系
    /// </summary>
    internal struct InforPair
    {
        public FileType FileType { get; set; }
        public string FileFilter { get; set; }
        public AbstractConverter Converter { get; set; }
    }

    public static class FileTypeExtensions
    {
        #region [文件类型-筛选器-转换器]对应关系
        internal static readonly InforPair[] InforPairs = new InforPair[]
        {
            new InforPair { FileType = FileType.SLK,        
                            FileFilter = "SLK矩阵文件 |*.slk;",            
                            Converter = ConvertSLK.Instance },
            new InforPair { FileType = FileType.JDReport,   
                            FileFilter = "精雕在机测量报告 |*_Export.txt;", 
                            Converter = ConvertJDReport.Instance },
            new InforPair { FileType = FileType.CMMReport,
                            FileFilter = "三坐标测量报告 |*.txt;",
                            Converter = ConvertCMMReport.Instance },
            new InforPair { FileType = FileType.DWData,
                            FileFilter = "Dewesoft采集数据 |*.dxd;",
                            Converter = ConvertDWData.Instance },
            new InforPair { FileType = FileType.GCode,
                            FileFilter = "普通G代码刀轨 |*.nc;*.mpf;",
                            Converter = ConvertGCode.Instance },
            new InforPair { FileType = FileType.APT,
                            FileFilter = "普通CLS刀轨 |*.cls;",
                            Converter = ConvertAPT.Instance },
            new InforPair { FileType = FileType.APTWithDrag,
                            FileFilter = "带前倾角CLS刀轨 |*.cls;",
                            Converter = ConvertAPTWithDarg.Instance },
            new InforPair { FileType = FileType.APTWithContact,
                            FileFilter = "带切触点CLS刀轨 |*.cls;",
                            Converter = ConvertAPTWithContact.Instance },
            new InforPair { FileType = FileType.Scan3DData,
                            FileFilter = "scanCONTROL三维扫描数据 |*.csv;",
                            Converter = ConvertScan3DData.Instance },
            new InforPair { FileType = FileType.MergeCsv,
                            FileFilter = "csv文件 |*.csv;",
                            Converter = ConvertMergeCSV.Instance },

        };
        #endregion

        #region 获取文件类型对应的信息

        /// <summary>
        /// 文件类型对应的文件过滤器
        /// </summary>
        public static string GetFileFilter(this FileType fileType)
        {
            return InforPairs.FirstOrDefault(p => p.FileType == fileType).FileFilter;
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
            return fileType.GetFileFilter().Split('|').Last().Replace("*.", ".").Split(';');

        }
        /// <summary>
        /// 根据文件类型选择对应的处理器
        /// </summary>
        public static AbstractConverter GetConverter(this FileType fileType)
        {
            return InforPairs.FirstOrDefault(p => p.FileType == fileType).Converter;
        }

        #endregion

        #region 打开文件对话框

        private static string LastOpenPath = string.Empty;
        /// <summary>
        /// 打开文件选择对话框
        /// </summary>
        public static OpenFileDialog OpenFileDialog(this FileType fileType, string initialDir = "") =>
             new OpenFileDialog()
             {
                 Title = "请选择要添加的数据文件",
                 Filter = fileType.GetFileFilter(),
                 Multiselect = !fileType.GetConverter().CanSingleToMulti,
                 InitialDirectory = string.IsNullOrEmpty(LastOpenPath) ? 
                                    Directory.GetCurrentDirectory() : 
                                    LastOpenPath,
             };

        private static string LastSavePath = string.Empty;
        /// <summary>
        /// 打开文件保存对话框
        /// </summary>
        public static SaveFileDialog SaveFileDialog(this FileType fileType) =>
             new SaveFileDialog()
             {
                 Title = "请选择要保存的文件",
                 Filter = "csv文件 |*.csv;",
             };
        #endregion
    }
}