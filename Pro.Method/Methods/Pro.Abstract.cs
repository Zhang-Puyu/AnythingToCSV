using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Processor.Methods
{
    public abstract class AbstractProcessor
    {
        /// <summary>
        /// 一个源数据文件开始处理时触发的事件
        /// </summary>
        public Action<string> ProcessStartedEvent { get; set; } = null;
        /// <summary>
        /// 一个源数据文件处理完成时触发的事件
        /// </summary>
        public Action<string> ProcessFinishedEvent { get; set; } = null;

        public Action<string> ErrorMsgEvent { get; set; } = null;
        public Action<string> InfoMsgEvent { get; set; } = null;

        /// <summary>
        /// 判断方法是否被重写
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <returns>是否被重写</returns>
        private bool IsMethodOverridden(in string methodName)
        {
            var baseMethod = typeof(AbstractProcessor).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            var subMethod = this.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            return baseMethod.DeclaringType != subMethod.DeclaringType;
        }

        /// <summary>
        /// 单个文件能否输出为多个csv
        /// </summary>
        public bool CanSingleToMulti => IsMethodOverridden(nameof(SingleToMulti));
        /// <summary>
        /// 单个文件能否输出为一个csv
        /// </summary>
        public bool CanSingleToSingle => IsMethodOverridden(nameof(SingleToSingle));

        /// <summary>
        /// 合并多个csv文件
        /// </summary>
        /// <param name="orifiles">源文件列表</param>
        /// <param name="tarfile">目标文件</param>
        /// <param name="skipFirstRow">合并时是否跳过第一行</param>
        public static void MergeCsv(in IEnumerable<string> orifiles, string tarfile, bool skipFirstRow = false)
        {
            using (StreamWriter writer = new StreamWriter(tarfile))
            {
                if (skipFirstRow)
                {
                    using (StreamReader reader = new StreamReader(orifiles.First()))
                    {
                        writer.WriteLine(reader.ReadLine());
                        reader.Close();
                    }
                }
                string line = null;
                foreach (string file in orifiles)
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

        /// <summary>
        /// 将一个源数据文件转换为一个csv文件
        /// </summary>
        /// <param name="orifile">源文件</param>
        /// <param name="csvfile">目标文件</param>
        public virtual void SingleToSingle(string orifile, string csvfile) 
        { 
            Debug.WriteLine("单个源数据文件转单个csv文件方法未实现");
        }

        /// <summary>
        /// 将一个源数据文件转换拆分为多个csv文件
        /// </summary>
        /// <param name="orifile">源文件</param>
        /// <param name="folder">目标文件存放路径</param>
        public virtual void SingleToMulti(string orifile, string folder)
        {
            Debug.WriteLine("单个源数据文件转多个csv文件方法未实现");
        }

        /// <summary>
        /// 将每个源数据文件转换为一个csv文件
        /// </summary>
        /// <param name="orifiles"></param>
        /// <param name="folder"></param>
        public virtual Task EachToEach(in IEnumerable<string> orifiles, string folder)
        {
            Parallel.ForEach(orifiles, file =>
            {
                ProcessStartedEvent?.Invoke(file);
                SingleToSingle(file, Path.Combine(folder, Path.GetFileNameWithoutExtension(file) + ".csv"));
                ProcessFinishedEvent?.Invoke(file);
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// 将每个源数据文件转换拆分为多个csv文件
        /// </summary>
        /// <param name="orifiles"></param>
        /// <param name="tarfolder"></param>
        public virtual Task EachToMulti(in IEnumerable<string> orifiles, string tarfolder)
        {
            Parallel.ForEach(orifiles, file =>
            {
                ProcessStartedEvent?.Invoke(file);
                SingleToMulti(file, tarfolder);
                ProcessFinishedEvent?.Invoke(file);
            });
            return Task.CompletedTask;
        }
    }
}
