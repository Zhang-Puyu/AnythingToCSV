using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Convert.Methods
{
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

        #region 根据方法是否被重写判断支持的转换方式

        /// <summary>
        /// 判断方法是否被重写
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <returns>是否被重写</returns>
        private bool IsMethodOverridden(in string methodName)
        {
            var baseMethod = typeof(AbstractConverter).GetMethod(methodName);
            var subMethod = this.GetType().GetMethod(methodName);

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
