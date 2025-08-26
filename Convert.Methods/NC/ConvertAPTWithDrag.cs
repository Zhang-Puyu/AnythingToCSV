using System.Collections.Generic;
using System.Text.RegularExpressions;
using Convert.NC;
using NCToolBox.Toolpath;

namespace Convert.Methods.NC
{
    /// <summary>
    /// 处理MaxPac生成的带前倾角的cls刀轨文件
    /// </summary>
    public class ConvertAPTWithDarg : ConvertNC
    {
        #region 单例模式
        private static ConvertAPTWithDarg instance = null;
        protected static readonly object padlock = new object();
        public static ConvertAPTWithDarg Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ConvertAPTWithDarg();
                    return instance;
                }
            }
        }

        private ConvertAPTWithDarg() { }
        #endregion

        private readonly NCToolBox.Toolpath.APT.ReadAPTWithDrag reader 
            = new NCToolBox.Toolpath.APT.ReadAPTWithDrag();
        protected override AbsractReader Reader => reader;
    }
}
