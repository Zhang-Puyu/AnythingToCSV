using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MachKit.Common;

namespace Convert.Methods.Converters
{
    /// <summary>
    /// csv合并模式
    /// </summary>
    public enum MergeModel
    {
        /// <summary>
        /// 合并行
        /// </summary>
        MergeRows,
        /// <summary>
        /// 合并列
        /// </summary>
        MergeColumns
    }

    public class ConvertMergeCSV : AbstractConverter
    {
        public override string FileFilter => "csv文件 |*.csv;";

        #region 按行合并
        /// <summary>
        /// 按行合并多个csv文件
        /// </summary>
        /// <param name="oriFiles">源文件列表</param>
        /// <param name="tarFile">目标文件</param>
        /// <param name="skipFirstRow">源文件是否包含表头</param>
        public void MergeCSVAsRow(in IEnumerable<string> oriFiles, string tarFile, bool skipFirstRow = false)
        {
            OneFileConvertStartedEvent?.Invoke("开始合并文件");

            using (StreamWriter writer = new StreamWriter(tarFile.RenameIfExist(), false, WriteEncoding))
            {
                if (skipFirstRow)
                {
                    using (StreamReader reader = new StreamReader(oriFiles.First(), ReadEncoding))
                    {
                        writer.WriteLine(reader.ReadLine());
                        reader.Close();
                    }
                }
                string line = null;
                foreach (string oriFile in oriFiles)
                {
                    using (StreamReader reader = new StreamReader(oriFile, ReadEncoding))
                    {
                        if (skipFirstRow) reader.ReadLine();
                        while ((line = reader.ReadLine()) != null)
                            writer.WriteLine(line);
                        reader.Close();
                    }
                }
                writer.Close();
            }

            OneFileConvertFinishedEvent?.Invoke("文件合并完成");
        }
        #endregion

        #region 按列合并
        /// <summary>
        /// 按列合并多个csv文件
        /// </summary>
        /// <param name="oriFiles">源文件列表</param>
        /// <param name="tarFile">目标文件</param>
        /// <param name="skipFirstRow">源文件是否包含表头</param>
        public void MergeCSVAsColumn(in IEnumerable<string> oriFiles, string tarFile, bool skipFirstRow = false)
        {
            OneFileConvertStartedEvent?.Invoke("开始合并文件");

            StringBuilder head = new StringBuilder();
            List<StringBuilder> data = new List<StringBuilder>();
            foreach (string file in oriFiles)
            {
                using (StreamReader reader = new StreamReader(file, ReadEncoding))
                {
                    if (skipFirstRow)
                    {
                        head.Append(reader.ReadLine());
                        head.Append(",");
                    }

                    string[] lines = reader.ReadToEnd().
                        Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    for(int i = 0; i < lines.Length; i++)
                    {
                        if (data.Count <= i)
                            data.Add(new StringBuilder());
                        data[i].Append(lines[i]);
                        data[i].Append(",");
                    }

                    reader.Close();
                }
            }
            using (StreamWriter writer = new StreamWriter(tarFile.RenameIfExist(), false, WriteEncoding))
            {
                if (skipFirstRow)
                    // 删除head最后一个逗号
                    writer.WriteLine(head.ToString().Substring(0, head.Length - 1));

                for (int i = 0; i < data.Count; i++)
                {
                    writer.WriteLine(data[i].ToString().Substring(0, head.Length - 1));
                }
                writer.Close();
            }

            OneFileConvertFinishedEvent?.Invoke("文件合并完成");
        }
        #endregion
    }
}
