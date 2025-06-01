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
        GCodeMon,
        /// <summary>
        /// 刀轨程序G代码
        /// </summary>
        GCode,
        /// <summary>
        /// 刀轨程序Apt代码
        /// </summary>
        ClsStd,
        /// <summary>
        /// MaxPac生成带前倾角cls代码
        /// </summary>
        ClsDrag,
        /// <summary>
        /// UG导出带接触信息的cls刀轨
        /// </summary>
        ClsEngage,
        /// <summary>
        /// 合并csv文件
        /// </summary>
        MergeCsv,
        /// <summary>
        /// 不支持的文件类型
        /// </summary>
        Unsurpport,
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
            new InforPair { FileType = FileType.Slk,        
                            FileFilter = "SLK矩阵文件 |*.slk;",            
                            Converter = ConvertSlk.Instance },
            new InforPair { FileType = FileType.JDReport,   
                            FileFilter = "精雕在机测量报告 |*_Export.txt;", 
                            Converter = ConvertJDReport.Instance },
            new InforPair { FileType = FileType.CMMReport,
                            FileFilter = "三坐标测量报告 |*.txt;",
                            Converter = ConvertCMMReport.Instance },
            new InforPair { FileType = FileType.DWData,
                            FileFilter = "Dewesoft采集数据 |*.dxd;",
                            Converter = ConvertDWData.Instance },
            new InforPair { FileType = FileType.GCodeMon,
                            FileFilter = "在线监测G代码 |*.mon;",
                            Converter = ConvertGCodeMon.Instance },
            new InforPair { FileType = FileType.GCode,
                            FileFilter = "普通G代码刀轨 |*.nc;*.mpf;",
                            Converter = ConvertGCode.Instance },
            new InforPair { FileType = FileType.ClsStd,
                            FileFilter = "普通CLS刀轨 |*.cls;",
                            Converter = ConvertClsStd.Instance },
            new InforPair { FileType = FileType.ClsDrag,
                            FileFilter = "带前倾角CLS刀轨 |*.cls;",
                            Converter = ConvertClsDrag.Instance },
            new InforPair { FileType = FileType.ClsEngage,
                            FileFilter = "带切触点CLS刀轨 |*.cls;",
                            Converter = ConvertClsEngage.Instance },
            new InforPair { FileType = FileType.Scan3DData,
                            FileFilter = "scanCONTROL三维扫描数据 |*.csv;",
                            Converter = ConvertScan3DData.Instance },
            new InforPair { FileType = FileType.MergeCsv,
                            FileFilter = "csv文件 |*.csv;",
                            Converter = ConvertMergeCsv.Instance },
            new InforPair { FileType = FileType.Unsurpport,
                            FileFilter = "任意文件类型 |*.*;",
                            Converter = null },

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

        #region 打开/保存文件对话框
        /// <summary>
        /// 打开文件选择对话框
        /// </summary>
        public static OpenFileDialog OpenFileDialog(this FileType fileType) =>
             new OpenFileDialog()
             {
                 Title = "请选择要添加的数据文件",
                 Filter = fileType.GetFileFilter(),
                 Multiselect = !fileType.GetConverter().CanSingleToMulti,
             };
        /// <summary>
        /// 打开文件保存对话框
        /// </summary>
        public static SaveFileDialog SaveFileDialog(this FileType fileType) =>
            new SaveFileDialog()
            {
                Title = "请选择要保存的数据文件",
                Filter = fileType.GetFileFilter(),
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
        public static FileType FindFileType(this string file)
        {
            // 精雕在机测量报表实际文件后缀为.txt，和三坐标测量结果相同
            if(file.ToLower().EndsWith("_export.txt")) 
                return FileType.JDReport;

            string extension = Path.GetExtension(file).ToLower();
            foreach (var pair in FileTypeExtensions.InforPairs)
                if (pair.FileFilter.Contains(extension))
                    return pair.FileType;

            return FileType.Unsurpport;
        }
    }
}