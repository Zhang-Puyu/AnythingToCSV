using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Processor.Methods
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
        /// 不支持的文件类型
        /// </summary>
        Unsurpport,
    }

    public static class FileTypeExtensions
    {
        /// <summary>
        /// 文件类型对应的文件过滤器
        /// </summary>
        public static string GetFileFilter(this FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Slk:        return "SLK矩阵文件 |*.slk;";
                case FileType.JDReport:   return "精雕在机测量报告 |*_Export.txt;";
                case FileType.CMMReport:  return "三坐标测量报告 |*.txt;";
                case FileType.DWData:     return "Dewesoft采集数据 |*.dxd;";
                case FileType.MonNCCode:  return "在线监测G代码 |*.mon;";
                case FileType.NCCode:     return "刀轨程序 .nc/.cls/.mpf |*.nc;*.cls;*.mpf;";
                case FileType.Scan3DData: return "scanCONTROL三维扫描数据 |*.csv;";

                default: return null;
            }
        }
        /// <summary>
        /// 根据文件类型选择对应的处理器
        /// </summary>
        public static AbstractProcessor ChooseProcesser(this FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Slk:        return ProSlk.Instance;
                case FileType.JDReport:   return ProJDReport.Instance;
                case FileType.CMMReport:  return ProCMMReport.Instance;
                case FileType.DWData:     return ProDWData.Instance;
                case FileType.MonNCCode:  return ProMonNCCode.Instance;
                case FileType.NCCode:     return ProNCCode.Instance;
                case FileType.Scan3DData: return ProScan3DData.Instance;

                default: return null;
            }
        }
        /// <summary>
        /// 各文件类型的描述
        /// </summary>
        public static string GetDescription(this FileType fileType)
        {
            return fileType.GetFileFilter().Split('|').First();
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
                 Multiselect = !fileType.ChooseProcesser().CanSingleToMulti,
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
        /// <summary>
        /// 打开文件夹选择对话框
        /// </summary>
        public static CommonOpenFileDialog FolderDialog(this FileType fileType) =>
            new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Title = "请选择要保存路径",
            };
        #endregion
    }

    public static class StringExtensions
    {
        private static Dictionary<string, FileType> FileTypeDic
            = new Dictionary<string, FileType>
        {
            { ".slk",        FileType.Slk },
            { "_export.txt", FileType.JDReport },
            { ".txt",        FileType.CMMReport },
            { ".dxd",        FileType.DWData },
            { ".mon",        FileType.MonNCCode },
            { ".nc",         FileType.NCCode },
            { ".cls",        FileType.NCCode },
            { ".mpf",        FileType.NCCode },
            { ".csv",        FileType.Scan3DData },
        };

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

            return FileTypeDic.TryGetValue(Path.GetExtension(file).ToLower(), out var fileType) ? 
                fileType :
                FileType.Unsurpport;
        }
    }
}