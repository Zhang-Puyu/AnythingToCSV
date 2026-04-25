using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MachKit.Common;

namespace Convert.Methods.Converters
{
    public abstract class AbstractConverter
    {
        #region 获取所有继承自AbstractConverter的子类
        public static IEnumerable<AbstractConverter> GetAllConvertersInstances()
        {
            // 1. 获取当前类库所在的程序集（核心：只扫描本类库，避免扫描整个程序）
            Assembly assembly = Assembly.GetAssembly(typeof(AbstractConverter));

            if (assembly == null)
                yield break;

            // 2. 筛选条件：
            //    - 不是抽象类
            //    - 继承自 AbstractClass
            //    - 包含无参数构造函数
            var subclassTypes = assembly.GetTypes()
                .Where(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    typeof(AbstractConverter).IsAssignableFrom(type) &&
                    type.GetConstructor(Type.EmptyTypes) != null);

            // 3. 遍历并创建实例
            foreach (Type type in subclassTypes)
            {
                AbstractConverter instance = (AbstractConverter)Activator.CreateInstance(type);
                yield return instance;
            }
        }

        #endregion 

        #region 事件
        /// <summary>
        /// 一个源数据文件开始处理时触发的事件
        /// </summary>
        public Action<string> OneFileConvertStartedEvent { get; set; } = null;
        /// <summary>
        /// 一个源数据文件处理完成时触发的事件
        /// </summary>
        public Action<string> OneFileConvertFinishedEvent { get; set; } = null;
        /// <summary>
        /// 错误消息事件
        /// </summary>
        public Action<string> ErrorMsgEvent { get; set; } = null;
        /// <summary>
        /// 信息消息事件
        /// </summary>
        public Action<string> InfoMsgEvent { get; set; } = null;
        #endregion

        #region 处理的数据对象说明
        /// <summary>
        /// 文件筛选表达式
        /// </summary>
        public abstract string FileFilter { get; }
        /// <summary>
        /// 数据文件说明
        /// </summary>
        public string Description 
            => FileFilter.Split('|').First();
        
        /// <summary>
        /// 数据文件后缀
        /// </summary>
        public string[] FileExtensions 
             => FileFilter.Split('|').Last().Replace("*.", ".").Split(';'); 
        
        /// <summary>
        /// 打开文件选择对话框
        /// </summary>
        public OpenFileDialog OpenFileDialog() =>
             new OpenFileDialog()
             {
                 Title = "请选择要添加的数据文件",
                 Filter = FileFilter,
                 Multiselect = true,
                 // Multiselect = !fileType.GetConverter().CanConvertSingleToMulti,
             };

        /// <summary>
        /// 打开文件保存对话框
        /// </summary>
        public SaveFileDialog SaveFileDialog() =>
             new SaveFileDialog()
             {
                 Title = "请选择要保存的文件",
                 Filter = "csv文件 |*.csv;",
             };
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

        #region 根据方法是否被重写判断是否支持转换方式

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
        public bool CanConvertSingleToMulti => IsMethodOverridden(nameof(ConvertSingleToMulti));
        /// <summary>
        /// 单个文件能否转换为一个csv
        /// </summary>
        public bool CanConvertSingleToSingle => IsMethodOverridden(nameof(ConvertSingleToSingle));

        #endregion

        #region 文件转换方法

        /// <summary>
        /// 将一个源数据文件转换为一个csv文件
        /// </summary>
        /// <param name="oriFile">源文件</param>
        /// <param name="tarFile">目标文件</param>
        public virtual void ConvertSingleToSingle(string oriFile, string tarFile)
        {
            Debug.WriteLine("单个源数据文件转单个csv文件方法未实现");
        }

        /// <summary>
        /// 将一个源数据文件转换拆分为多个csv文件
        /// </summary>
        /// <param name="oriFile">源文件</param>
        /// <param name="tarFolder">目标路径</param>
        public virtual void ConvertSingleToMulti(string oriFile, string tarFolder)
        {
            Debug.WriteLine("单个源数据文件转多个csv文件方法未实现");
        }

        /// <summary>
        /// 将每个源数据文件转换为一个csv文件
        /// </summary>
        /// <param name="oriFiles">源文件</param>
        /// <param name="tarFolder">目标路径</param>
        public virtual Task ConvertEachToSingle(in IEnumerable<string> oriFiles, string tarFolder)
        {
            Parallel.ForEach(oriFiles, oriFile =>
            {
                OneFileConvertStartedEvent?.Invoke(oriFile);
                string tarFile = Path.Combine(tarFolder,
                    Path.GetFileNameWithoutExtension(oriFile) + ".csv").RenameIfExist();
                ConvertSingleToSingle(oriFile, tarFile);
                OneFileConvertFinishedEvent?.Invoke(oriFile);
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// 将每个源数据文件转换拆分为多个csv文件
        /// </summary>
        /// <param name="oriFiles">源文件</param>
        /// <param name="tarFolder">目标路径</param>
        public virtual Task ConvertEachToMulti(in IEnumerable<string> oriFiles, string tarFolder)
        {
            Parallel.ForEach(oriFiles, file =>
            {
                OneFileConvertStartedEvent?.Invoke(file);

                string baseName = Path.GetFileNameWithoutExtension(file);
                string childFolder = Path.Combine(tarFolder, baseName).RenameIfExist();
                Directory.CreateDirectory(childFolder);

                ConvertSingleToMulti(file, childFolder);
                OneFileConvertFinishedEvent?.Invoke(file);
            });
            return Task.CompletedTask;
        }

        #endregion
    }
}
