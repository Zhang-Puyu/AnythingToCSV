using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convert.Methods
{
    #region 文件重命名
    internal static class StringExtension
    {
        /// <summary>
        /// 若文件已存在，则重命名
        /// </summary>
        /// <param name="oriFile">原文件</param>
        /// <returns>文件新名称</returns>
        public static string RenameIfFileExists(this string oriFile)
        {
            // 若文件已存在
            if (File.Exists(oriFile))
            {
                string fileName = Path.GetFileNameWithoutExtension(oriFile);
                string ext = Path.GetExtension(oriFile);
                string folder = Path.GetDirectoryName(oriFile);
                int i = 1;
                string newFile = Path.Combine(folder, $"{fileName}({i}){ext}");
                while (File.Exists(newFile))
                {
                    i++;
                    newFile = Path.Combine(folder, $"{fileName}({i}){ext}");
                }
                return newFile;
            }
            return oriFile;
        }
    }
    #endregion

    public abstract class AbstractConverter
    {
        #region 事件
        /// <summary>
        /// 一个源数据文件开始处理时触发的事件
        /// </summary>
        public Action<string> StartedEvent { get; set; } = null;
        /// <summary>
        /// 一个源数据文件处理完成时触发的事件
        /// </summary>
        public Action<string> FinishedEvent { get; set; } = null;
        /// <summary>
        /// 错误消息事件
        /// </summary>
        public Action<string> ErrorMsgEvent { get; set; } = null;
        /// <summary>
        /// 信息消息事件
        /// </summary>
        public Action<string> InfoMsgEvent { get; set; } = null;

        #endregion

        #region 编码格式

        /// <summary>
        /// 支持的编码格式
        /// </summary>
        public static readonly Encoding[] SurpportedEncodings
            = new Encoding[] 
            {
                    Encoding.UTF8,
                    Encoding.Unicode, // UTF-16
                    Encoding.UTF32,
                    Encoding.GetEncoding("GBK"),
            };

        /// <summary>
        /// 读取文件的编码格式
        /// </summary>
        public Encoding ReadEncoding = Encoding.GetEncoding("GBK");

        /// <summary>
        /// 写入文件的编码格式
        /// </summary>
        public Encoding WriteEncoding = Encoding.UTF8;

        #endregion

        #region 支持的转换方式

        /// <summary>
        /// 判断方法是否被重写
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <returns>是否被重写</returns>
        private bool IsMethodOverridden(in string methodName)
        {
            var baseMethod = typeof(AbstractConverter).GetMethod(methodName);
            var subMethod  = this.GetType().GetMethod(methodName);

            return baseMethod.DeclaringType != subMethod.DeclaringType;
        }

        /// <summary>
        /// 单个文件能否转换为多个csv
        /// </summary>
        public bool CanSingleToMulti => IsMethodOverridden(nameof(SingleToMulti));
        /// <summary>
        /// 单个文件能否转换为一个csv
        /// </summary>
        public bool CanSingleToSingle => IsMethodOverridden(nameof(SingleToSingle));

        #endregion

        #region 合并CSV
        /// <summary>
        /// 合并多个csv文件
        /// </summary>
        /// <param name="oriFiles">源文件列表</param>
        /// <param name="tarFile">目标文件</param>
        /// <param name="skipFirstRow">合并时是否跳过第一行</param>
        public static void MergeCsv(in IEnumerable<string> oriFiles, string tarFile, bool skipFirstRow = false)
        {
            using (StreamWriter writer = new StreamWriter(tarFile.RenameIfFileExists()))
            {
                if (skipFirstRow)
                {
                    using (StreamReader reader = new StreamReader(oriFiles.First()))
                    {
                        writer.WriteLine(reader.ReadLine());
                        reader.Close();
                    }
                }
                string line = null;
                foreach (string file in oriFiles)
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        if (skipFirstRow) reader.ReadLine();
                        while ((line = reader.ReadLine()) != null)
                            writer.WriteLine(line);
                        reader.Close();
                    }
                }
                writer.Close();
            }
        }
        #endregion

        #region 文件转换方法

        /// <summary>
        /// 将一个源数据文件转换为一个csv文件
        /// </summary>
        /// <param name="oriFile">源文件</param>
        /// <param name="tarFile">目标文件</param>
        public virtual void SingleToSingle(string oriFile, string tarFile) 
        { 
            Debug.WriteLine("单个源数据文件转单个csv文件方法未实现");
        }

        /// <summary>
        /// 将一个源数据文件转换拆分为多个csv文件
        /// </summary>
        /// <param name="oriFile">源文件</param>
        /// <param name="tarFolder">目标路径</param>
        public virtual void SingleToMulti(string oriFile, string tarFolder)
        {
            Debug.WriteLine("单个源数据文件转多个csv文件方法未实现");
        }

        /// <summary>
        /// 将每个源数据文件转换为一个csv文件
        /// </summary>
        /// <param name="oriFiles">源文件</param>
        /// <param name="tarFolder">目标路径</param>
        public virtual Task EachToEach(in IEnumerable<string> oriFiles, string tarFolder)
        {
            Parallel.ForEach(oriFiles, oriFile =>
            {
                StartedEvent?.Invoke(oriFile);
                string tarFile = Path.Combine(tarFolder,
                    Path.GetFileNameWithoutExtension(oriFile) + ".csv").RenameIfFileExists();
                SingleToSingle(oriFile, tarFile);
                FinishedEvent?.Invoke(oriFile);
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// 将每个源数据文件转换拆分为多个csv文件
        /// </summary>
        /// <param name="oriFiles">源文件</param>
        /// <param name="tarFolder">目标路径</param>
        public virtual Task EachToMulti(in IEnumerable<string> oriFiles, string tarFolder)
        {
            Parallel.ForEach(oriFiles, file =>
            {
                StartedEvent?.Invoke(file);
                SingleToMulti(file, tarFolder);
                FinishedEvent?.Invoke(file);
            });
            return Task.CompletedTask;
        }

        #endregion
    }
}
